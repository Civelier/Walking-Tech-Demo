using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ValueSlider : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI ValueText;
    public int Decimals = 2;

    public float Value
    {
        get { return Slider.value; }
        set { Slider.value = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        Slider.onValueChanged.AddListener(ValueChanged);
        ValueChanged(Slider.value);
    }

    void ValueChanged(float val)
    {
        ValueText.text = Math.Round(val, Decimals).ToString();
    }
}
