using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.GameMenu
{
    public interface IMenu
    {
        UnityEvent LostFocus { get; }
        UnityEvent GotFocus { get; }
        IMenu Current { get; set; }
        bool IsCurrent { get; set; }
        void Focus();
        void Show();
        void Hide();
    }
}
