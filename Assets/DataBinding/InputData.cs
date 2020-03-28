using Assets.GameMenu;
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
        public InputDisplayer[] Displayers;
        public InputAction Action;
        public InputData(InputAction action, InputDisplayer[] displayers = null)
        {
            ControlName = action.name;
            KeyboardControl = "";
            XboxControl = "";
            Displayers = displayers ?? new[]
            {
                new InputDisplayer(action, 0),
                new InputDisplayer(action, 1)
            };
            Action = action;
            foreach (var c in action.controls)
            {
                if (c.device == InputSystem.GetDevice<UnityEngine.InputSystem.XInput.XInputController>())
                {
                    XboxControl = Displayers[1].Text;
                }

                if (c.device == InputSystem.GetDevice<Mouse>() || c.device == InputSystem.GetDevice<Keyboard>())
                {
                    KeyboardControl = Displayers[0].Text;
                }
            }
        }

        public void SetKeyboardDisplayer(InputDisplayer displayer)
        {
            Displayers[0] = displayer;
            foreach (var c in Action.controls)
            {
                if (c.device == InputSystem.GetDevice<Mouse>() || c.device == InputSystem.GetDevice<Keyboard>())
                {
                    KeyboardControl = Displayers[0].Text;
                }
            }
        }

        public void SetXboxDisplayer(InputDisplayer displayer)
        {
            Displayers[1] = displayer;
            foreach (var c in Action.controls)
            {
                if (c.device == InputSystem.GetDevice<UnityEngine.InputSystem.XInput.XInputController>())
                {
                    XboxControl = Displayers[1].Text;
                }
            }
        }
    }
}
