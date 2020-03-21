using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public interface IUIProperty
{
    object GetValue(string valName = "default");
    void SetValue(object o, string valName = "default");
}

public class SliderProperty : Slider, IUIProperty
{
    public object GetValue(string valName = "default")
    {
        switch (valName)
        {
            case "default":
                return value;
            default:
                return null;
        }
    }

    public void SetValue(object o, string valName = "default")
    {
        switch (valName)
        {
            case "default":
                int i;
                if (int.TryParse(o.ToString(), out i))
                {
                    value = i;
                }
                break;
            default:
                break;
        }
    }
}

public enum UIValueTypes
{
    UIInt,
    UIFloat,
    UIBool,
    UIString,
    UIVector2,
    UIVector3,
    UIVector4
}

public class UIValue
{
    public UIValueTypes type;

    [Header("Value")]
    public int[] iValue;
    [Header("Value")]
    public float[] fValue;
    [Header("Value")]
    public bool[] bValue;
    [Header("Value")]
    public string[] sValue;
    [Header("Value")]
    public Vector2[] v2Value;
    [Header("Value")]
    public Vector3[] v3Value;
    [Header("Value")]
    public Vector4[] v4Value;

    public object GetValue()
    {
        switch (type)
        {
            case UIValueTypes.UIInt:
                return iValue;
            case UIValueTypes.UIFloat:
                return fValue;
            case UIValueTypes.UIBool:
                return bValue;
            case UIValueTypes.UIString:
                return sValue;
            case UIValueTypes.UIVector2:
                return v2Value;
            case UIValueTypes.UIVector3:
                return v3Value;
            case UIValueTypes.UIVector4:
                return v4Value;
            default:
                return null;
        }
    }

    public void SetValue(object o)
    {
        switch (type)
        {
            case UIValueTypes.UIInt:
                iValue = (int[])o;
                break;
            case UIValueTypes.UIFloat:
                fValue = (float[])o;
                break;
            case UIValueTypes.UIBool:
                bValue = (bool[])o;
                break;
            case UIValueTypes.UIString:
                sValue = (string[])o;
                break;
            case UIValueTypes.UIVector2:
                v2Value = (Vector2[])o;
                break;
            case UIValueTypes.UIVector3:
                v3Value = (Vector3[])o;
                break;
            case UIValueTypes.UIVector4:
                v4Value = (Vector4[])o;
                break;
            default:
                break;
        }
    }
}

[ExecuteInEditMode]
public class PropertySetter : MonoBehaviour
{
    public Dropdown PresetValues;
    public Slider MySlider;

    [Min(1)]
    public int Size;
    public int Default;
    public List<string> Presets;
    public List<float> Values;
    bool isChangingValue = false;


    // Start is called before the first frame update
    void Start()
    {
        //e = Editor.CreateEditor(typeof(Types));

        if (PresetValues == null) PresetValues = GetComponent<Dropdown>();
        if (PresetValues == null) PresetValues = GetComponentInChildren<Dropdown>();
        if (PresetValues == null) PresetValues = GetComponentInParent<Dropdown>();
        if (PresetValues == null)
        {
            Debug.LogError("Could not find a dropdown");
            return;
        }
        if (MySlider == null) MySlider = GetComponent<Slider>();
        if (MySlider == null) MySlider = GetComponentInChildren<Slider>();
        if (MySlider == null) MySlider = GetComponentInParent<Slider>();
        if (MySlider == null)
        {
            Debug.LogError("Could not find a slider");
            return;
        }

        PresetValues.onValueChanged.AddListener(ValueChanged);
        MySlider.onValueChanged.AddListener((float f) =>
        {
            if (!isChangingValue)
            {
                Values[Values.Count - 1] = f;
                PresetValues.SetValueWithoutNotify(Presets.Count - 1);
            }
            isChangingValue = false;
        });
        PresetValues.options[Default].text += " (Default)";
        PresetValues.value = Default;
        PresetValues.onValueChanged.Invoke(Default);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (EditorApplication.isPlaying) return;
        if (Presets.Count == 0) Presets.Add("Custom");
        if (Values.Count == 0) Values.Add(0);

        Presets.RemoveAt(Presets.Count - 1);
        var v = Values[Values.Count - 1];
        Values.RemoveAt(Values.Count - 1);
        for (int i = Presets.Count; i < Size -1; i++)
        {
            Presets.Add("");
            Values.Add(0);
        }
        for (int i = Presets.Count - 1; i > Size - 1; i--)
        {
            Presets.RemoveAt(i);
            Values.RemoveAt(i);
        }
        Presets.Add("Custom");
        Values.Add(v);
        PresetValues.ClearOptions();
        PresetValues.AddOptions(Presets);
    }
#endif

    void ValueChanged(int index)
    {
        isChangingValue = true;
        MySlider.value = Values[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
