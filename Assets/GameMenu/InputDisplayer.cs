using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Assets.GameMenu
{
    public enum DisplayType
    {
        Default,
        Image,
        OverrideText,
    }

    [Serializable]
    public class InputDisplayer
    {
        public InputActionReference Control;
        public DisplayType DisplayAs;
        public string Text;
        public Sprite Image;
    }
}
