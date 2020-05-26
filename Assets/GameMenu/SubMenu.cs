using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Events;

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

        private UnityEvent _lostFocus = new UnityEvent();
        public UnityEvent LostFocus => _lostFocus;

        private UnityEvent _gotFocus = new UnityEvent();
        public UnityEvent GotFocus => _gotFocus;

        //void Start()
        //{
        //    Parent.Escape.action.performed += Back;
        //}

        public void Back(InputAction.CallbackContext obj)
        {
            if (IsCurrent)
            {
                Hide();
                Parent.Show();
                _lostFocus.Invoke();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _lostFocus.Invoke();
        }

        public void Show(bool tempFocus = true)
        {
            gameObject.SetActive(true);
            _gotFocus.Invoke();
        }

        public void Focus()
        {
            Current = this;
            _gotFocus.Invoke();
        }
    }
}
