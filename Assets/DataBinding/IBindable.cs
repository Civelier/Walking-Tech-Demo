using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.DataBinding
{
    public interface IBindable
    {
        bool IsBinded { get; }
    }

    public interface IUIBindable : IBindable
    {
        GameObject DisplayObject { get; }
    }


}
