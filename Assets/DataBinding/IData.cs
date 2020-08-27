using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    /// <summary>
    /// The data representation of any type
    /// </summary>
    public interface IData : IBindable
    {
        /// <summary>
        /// Called when the data has been changed
        /// </summary>
        void OnDataChanged();
    }
    /// <summary>
    /// The data representation of a specific type
    /// </summary>
    /// <typeparam name="T">Type of the data</typeparam>
    public interface IData<T> : IData
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
        /// Gets the <see cref="IUIData{T}"/> object of this bind
        /// </summary>
        /// <returns>The <see cref="IUIData{T}"/> of this bind if it exists, otherwise, returns null</returns>
        IUIData<T> GetUIData();
    }
}
