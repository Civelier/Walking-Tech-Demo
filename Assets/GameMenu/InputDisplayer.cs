using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.GameMenu
{
    public enum DisplayType
    {
        Default,
        Image,
        OverrideText,
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InputDisplayer))]
    public class InputDisplayerDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property == null)
            {
                return EditorGUIUtility.singleLineHeight * 4 + 6;
            }
            switch ((DisplayType?)property.FindPropertyRelative("DisplayAs")?.enumValueIndex)
            {
                case DisplayType.Image:
                case DisplayType.OverrideText:
                    return EditorGUIUtility.singleLineHeight * 5 + 8;
                case DisplayType.Default:
                default:
                    return EditorGUIUtility.singleLineHeight * 4 + 6;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r0 = new Rect(position.x, position.y, position.width, 16);
            var r1 = new Rect(position.x, position.y + 18, position.width, 16);
            var r2 = new Rect(position.x, position.y + 36, position.width, 16);
            var r3 = new Rect(position.x, position.y + 54, position.width, 16);
            var r4 = new Rect(position.x, position.y + 72, position.width, 16);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.LabelField(r0, label);
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            var reference = property.FindPropertyRelative("Reference");
            EditorGUI.PropertyField(r1, reference);
            if (EditorGUI.EndChangeCheck())
            {
                InputDisplayer.Update(property);
                //((InputDisplayer)property.objectReferenceValue).Update();
            }
            if (reference?.objectReferenceValue != null)
            {
                var selection = property.FindPropertyRelative("Selection");
                EditorGUI.PropertyField(r2, selection);
                InputDisplayer.Update(property);
            }

            EditorGUI.PropertyField(r3, property.FindPropertyRelative("DisplayAs"));

            switch ((DisplayType)property.FindPropertyRelative("DisplayAs").enumValueIndex)
            {
                case DisplayType.Image:
                    EditorGUI.PropertyField(r4, property.FindPropertyRelative("Image"));
                    break;
                case DisplayType.OverrideText:
                    EditorGUI.PropertyField(r4, property.FindPropertyRelative("Text"));
                    break;
                case DisplayType.Default:
                default:
                    break;
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }
    }
#endif

    [Serializable]
    public class InputDisplayer
    {
        public InputActionReference Reference = null;
        public InputControl Control = null;
        public DisplayType DisplayAs = DisplayType.Default;
        public string Text = "";
        public Sprite Image = null;
        public SelectionList Selection = new SelectionList();
        public ReadOnlyArray<InputControl> Controls => Reference?.action.controls ?? new ReadOnlyArray<InputControl>();

        public InputDisplayer()
        {
        }

        public InputDisplayer(InputAction action, int index)
        {
            Reference = InputActionReference.Create(action);
            Update();
            Selection.SelectionIndex = index;
        }

#if UNITY_EDITOR
        public static void Update(SerializedProperty inputDisplayer)
        {
            var reference = (InputActionReference)inputDisplayer.FindPropertyRelative("Reference").objectReferenceValue;
            var controls = reference.asset[reference.name].bindings;
            var list = inputDisplayer.FindPropertyRelative("Selection").FindPropertyRelative("Array");
            list.arraySize = controls.Count;
            int ii = 0;
            for (int i = 0; i < controls.Count; i++)
            {
                var control = controls[i];
                if (!control.isPartOfComposite)
                {
                    if (controls[i].isComposite)
                    {
                        list.GetArrayElementAtIndex(ii).stringValue = controls[i].name;
                        ii++;
                    }
                    else
                    {
                        list.GetArrayElementAtIndex(ii).stringValue = controls[i].ToDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice);
                        ii++;
                    }
                }
                //if (controls[i].isPartOfComposite)
                //{
                //    readingComposite = true;
                //    sb.Append(controls[i].ToDisplayString(InputBinding.DisplayStringOptions.IgnoreBindingOverrides | InputBinding.DisplayStringOptions.DontIncludeInteractions));
                //    return;
                //}
                //if (readingComposite)
                //{
                //    list.GetArrayElementAtIndex(ii).stringValue = sb.ToString();
                //    sb.Clear();
                //    readingComposite = false;
                //    ii++;
                //}
            }
            list.arraySize = ii;
            if ((DisplayType)inputDisplayer.FindPropertyRelative("DisplayAs").enumValueIndex == DisplayType.Default)
            {
                inputDisplayer.FindPropertyRelative("Text").stringValue = list.GetArrayElementAtIndex(inputDisplayer.FindPropertyRelative("Selection").FindPropertyRelative("SelectionIndex").intValue).stringValue;
            }
        }
#endif

        IEnumerable<string> GetControls()
        {
            foreach (var c in Controls)
            {
                yield return c.displayName;
            }
        }

        public void Update()
        {
            Selection.Array = GetControls().ToArray();
            var controls = Reference.asset[Reference.action.name].bindings;
            var list = new List<string>();
            for (int i = 0; i < controls.Count; i++)
            {
                if (!controls[i].isPartOfComposite)
                {
                    if (controls[i].isComposite)
                    {
                        list.Add(controls[i].name);
                    }
                    else
                    {
                        list.Add(controls[i].ToDisplayString(InputBinding.DisplayStringOptions.DontOmitDevice));
                    }
                }
            }
            Selection.Array = list.ToArray();

            if (DisplayAs == DisplayType.Default)
            {
                Text = Selection.Item;
            }
        }
    }
}
