using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

[RequireComponent(typeof(RectTransform))]
public class ControlsDisplayer : MonoBehaviour
{
    public RectTransform RectTransform;
    public InputSettings Input;

    // Start is called before the first frame update
    void Start()
    {
        if (RectTransform == null) RectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
