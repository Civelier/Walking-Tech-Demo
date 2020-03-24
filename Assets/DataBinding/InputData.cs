using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

namespace Assets.DataBinding
{
    public struct InputData
    {
        public string ControlName;
        public string KeyboardControl;
        public string XboxControl;

        public InputData(InputAction action)
        {
            ControlName = action.name;
            KeyboardControl = "";
            XboxControl = "";
            foreach (var c in action.controls)
            {
                if (c.device == InputSystem.GetDevice<UnityEngine.InputSystem.XInput.XInputController>())
                {
                    XboxControl = c.name;
                }

                if (c.device == InputSystem.GetDevice<Mouse>() || c.device == InputSystem.GetDevice<Keyboard>())
                {
                    KeyboardControl = c.displayName;
                }
            }
        }
    }
}
