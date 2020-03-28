using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using Assets.Scripts;
using System;
using UnityEngine.Events;

class EditorGUIPopup : EditorWindow
{
    public string[] options = { "Rigidbody", "Box Collider", "Sphere Collider" };
    public int index = 0;

    [MenuItem("Examples/Editor GUI Popup usage")]
    static void Init()
    {
        var window = GetWindow<EditorGUIPopup>();
        window.position = new Rect(0, 0, 180, 80);
        window.Show();
    }

    public OnSelectEvent OnSelect = new OnSelectEvent();

    void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        index = EditorGUI.Popup(
            new Rect(0, 0, position.width, 20),
            "Control:",
            index,
            options);
        if (EditorGUI.EndChangeCheck())
        {
            OnSelect.Invoke(index);
            OnSelect.RemoveAllListeners();
            Close();
        }
    }

}

[CustomPropertyDrawer(typeof(SelectionList))]
public class SelectionListDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return PropertyDrawerUtilities.GetHeight(1);
    }

    IEnumerable<string> GetContent(SerializedProperty property)
    {
        for (int i = 0; i < property.arraySize; i++)
        {
            yield return property.GetArrayElementAtIndex(i).stringValue;
        }
    }

    void OnIndexChanged(int index, SerializedProperty property)
    {
        property.FindPropertyRelative("SelectionIndex").intValue = index;
        property.serializedObject.ApplyModifiedProperties();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //EditorGUI.BeginProperty(position, label, property);

        if (EditorGUI.DropdownButton(position, new GUIContent(SelectionList.GetItem(property)), FocusType.Passive))
        {
            var content = GetContent(property.FindPropertyRelative("Array")).ToArray();
            var popup = new EditorGUIPopup() { options = content, index = property.FindPropertyRelative("SelectionIndex").intValue };
            popup.ShowAsDropDown(GUIUtility.GUIToScreenRect(position), new Vector2(500, PropertyDrawerUtilities.GetHeight(content.Length)));
            //popup.Show();
            popup.OnSelect.AddListener((index) => OnIndexChanged(index, property));
            //property.FindPropertyRelative("SelectionIndex").intValue = EditorGUI.Popup(new Rect(0, 0, position.width, 20), property.FindPropertyRelative("SelectionIndex").intValue, content);
        }
        //EditorGUI.EndProperty();
    }
}

[Serializable]
public class SelectionList
{
    public string[] Array = new string[0];
    public int SelectionIndex = 0;
    public string Item
    {
        get => Array?.ElementAtOrDefault(SelectionIndex) ?? "";
        set
        {
            if (Array?.Contains(value) ?? false)
            {
                SelectionIndex = Array.ToList().IndexOf(value);
            }
        }
    }

    public SelectionList()
    {
        SelectionIndex = 0;
    }

    public static string GetItem(SerializedProperty selectionList)
    {
        var array = selectionList.FindPropertyRelative("Array");
        int count = array.arraySize;
        int selectionIndex = selectionList.FindPropertyRelative("SelectionIndex").intValue;
        if (count > 0)
        {
            var output = array.GetArrayElementAtIndex(Mathf.Min(count - 1, selectionIndex)).stringValue;
            return output;
        }
        return "";
    }
}
