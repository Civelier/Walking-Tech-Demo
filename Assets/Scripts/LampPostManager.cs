using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LampPostManager : MonoBehaviour
{
    public Material LightOn;
    public Material LightOff;
    public Light Light;
    public MeshRenderer Lamp;

    public bool _IsOn;
    public bool IsOn
    {
        get => _IsOn;
        set
        {
            _IsOn = value;
            if (_IsOn) TurnOn();
            else TurnOff();
        }
    }

    void TurnOn()
    {
        Light.enabled = true;
        if (Lamp != null) Lamp.material = LightOn;
    }

    void TurnOff()
    {
        Light.enabled = false;
        if (Lamp != null) Lamp.material = LightOff;
    }

    // Update is called once per frame

#if UNITY_EDITOR
    void Update()
    {
        IsOn = _IsOn;
    }
#endif
}
