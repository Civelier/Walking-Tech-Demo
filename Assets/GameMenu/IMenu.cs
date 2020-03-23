using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GameMenu
{
    public interface IMenu
    {
        IMenu Current { get; set; }
        bool IsCurrent { get; set; }
        void Focus();
        void Show();
        void Hide();
    }
}
