using Assets.DataBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.FieldGridElements
{
    public interface IDisplayableField : IUIData
    {
        string Name { get; set; }
        void Refresh();
    }
}
