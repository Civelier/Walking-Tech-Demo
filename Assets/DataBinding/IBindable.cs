using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.DataBinding
{
    /// <summary>
    /// Describes a data object that can be binded with an <see cref="IDataBinder"/>
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// Get if the <see cref="IBindable"/> is binded to a binder
        /// </summary>
        bool IsBinded { get; }
    }
    /// <summary>
    /// Describes a UI object that can be binded with an <see cref="IDataBinder"/>
    /// </summary>
    public interface IUIBindable : IBindable
    {
        /// <summary>
        /// The Unity <see cref="GameObject"/> of this UI object
        /// </summary>
        GameObject DisplayObject { get; }
    }


}
