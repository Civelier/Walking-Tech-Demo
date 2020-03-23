using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.GameMenu
{
    [Serializable]
    public struct ButtonMenuBind : IDisposable
    {
        [SerializeField]
        private Button _button;
        public Button Button
        {
            get => _button;
            set
            {
                _button?.onClick.RemoveListener(SubMenu.Focus);
                _button = value;
                _button?.onClick.AddListener(SubMenu.Focus);
            }
        }

        public SubMenu SubMenu;
        public ButtonMenuBind(Button button, SubMenu subMenu)
        {
            _button = button;
            SubMenu = subMenu;
            _button.onClick.AddListener(SubMenu.Focus);
        }

        public void Initialize()
        {
            _button.onClick.AddListener(SubMenu.Focus);
        }

        public void Dispose()
        {
            Button.onClick.RemoveListener(SubMenu.Focus);
        }

        public void Hide()
        {
            SubMenu.Hide();
        }

        public void Show()
        {
            SubMenu.Show();
        }
    }
}
