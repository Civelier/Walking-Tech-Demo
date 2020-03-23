using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Assets.GameMenu
{
    public class SubMenu : MonoBehaviour, IMenu
    {
        public GameMenuHandler Parent;

        public IMenu Current { get => Parent.Current; set => Parent.Current = value; }
        public bool IsCurrent
        {
            get => Current?.Equals(this) ?? false;
            set
            {
                if (value) Current = this;
                else Parent.Unfocus();
            }
        }

        void Start()
        {
            Parent.Escape.action.performed += Back;
        }

        private void Back(InputAction.CallbackContext obj)
        {
            if (IsCurrent) Current = Parent;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Focus()
        {
            Current = this;
        }
    }
}
