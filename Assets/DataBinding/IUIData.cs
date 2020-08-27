using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.DataBinding
{
    /// <summary>
    /// The UI representation of any type
    /// </summary>
    public interface IUIData : IUIBindable
    {
        void OnUIDataChanged();
    }

    /// <summary>
    /// The UI representation of a specific type
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    public interface IUIData<T> : IUIData
    {
        /// <summary>
        /// Event for data changes
        /// </summary>
        DataChangedEvent<T> DataChanged { get; set; }
        /// <summary>
        /// Stored data
        /// </summary>
        T Data { get; set; }
        /// <summary>
        /// Gets the <see cref="IData{T}"/> object of this bind
        /// </summary>
        /// <returns>The <see cref="IData{T}"/> of this bind if it exists, otherwise, returns null</returns>
        IData<T> GetData();
    }
}
