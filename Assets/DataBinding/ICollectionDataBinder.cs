using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public interface ICollectionDataBinder<T> : IDataBinder
    {
        /// <summary>
        /// The data representation of the binding
        /// </summary>
        ICollectionData<T> Data { get; }
        /// <summary>
        /// The UI representation of the binding
        /// </summary>
        ICollectionUIData<T> UIData { get; }
        /// <summary>
        /// Called when <see cref="ICollectionData{T}.Added"/> event is invoked
        /// </summary>
        /// <param name="items">Items added</param>
        void OnDataAdded(IEnumerable<T> items);
        /// <summary>
        /// Called when <see cref="ICollectionData{T}.Removed"/> event is invoked
        /// </summary>
        /// <param name="items">Items removed</param>
        void OnDataRemoved(IEnumerable<T> items);
        /// <summary>
        /// Called when <see cref="ICollectionData{T}.IndexValueChanged"/> event is invoked
        /// </summary>
        /// <param name="index">Index of the item changed</param>
        /// <param name="item">New item</param>
        void OnDataIndexValueChanged(int index, T item);
        /// <summary>
        /// Called when <see cref="ICollectionUIData{T}.Added"/> event is invoked
        /// </summary>
        /// <param name="items">Items added</param>
        void OnUIDataAdded(IEnumerable<T> items);
        /// <summary>
        /// Called when <see cref="ICollectionUIData{T}.Removed"/> event is invoked
        /// </summary>
        /// <param name="items">Items removed</param>
        void OnUIDataRemoved(IEnumerable<T> items);
        /// <summary>
        /// Called when <see cref="ICollectionUIData{T}.IndexValueChanged"/> event is invoked
        /// </summary>
        /// <param name="index">Index of the item changed</param>
        /// <param name="item">New item</param>
        void OnUIDataIndexValueChanged(int index, T item);
    }
}
