using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.DataBinding
{
    /// <summary>
    /// Data binder for collections of type <typeparamref name="T"/>
    /// The bridge between <see cref="ICollectionData{T}"/> and <see cref="ICollectionUIData{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the data</typeparam>
    public class BasicCollectionDataBinder<T> : ICollectionDataBinder<T>, IRevertableBinder
    {
        private bool _isRevertable;

        public ICollectionData<T> Data { get; private set; }
        public ICollectionUIData<T> UIData { get; private set; }

        /// <summary>
        /// Returns if the bind is revertable
        /// If true, the data will not be updated until either <see cref="ApplyToData"/> or <see cref="ApplyToUI"/> are called
        /// </summary>
        public bool IsRevertable => _isRevertable;

        /// <summary>
        /// The queue of events comming from the UI changes if the bind is revertable
        /// Will be applied if <see cref="ApplyToData"/> is called
        /// </summary>
        Queue<UnityAction> _uiDataEventQueue = new Queue<UnityAction>();
        /// <summary>
        /// The queue of events comming from the data changes if the bind is revertable
        /// Will be applied if <see cref="ApplyToUI"/> is called
        /// </summary>
        Queue<UnityAction> _dataEventQueue = new Queue<UnityAction>();

        /// <summary>
        /// Constructor for the basic collection data binder
        /// </summary>
        /// <param name="data">The data representation (<see cref="ICollectionData{T}"/>) of the bind</param>
        /// <param name="uiData">The UI representation (<see cref="ICollectionUIData{T}"/>) of the bind</param>
        /// <param name="isRevertable">Whether the bind is revertable or not</param>
        public BasicCollectionDataBinder(ICollectionData<T> data, ICollectionUIData<T> uiData, bool isRevertable)
        {
            _isRevertable = isRevertable;
            Data = data;
            UIData = uiData;

            Data.IndexValueChanged = new CollectionIndexValueChangeEvent<T>(this);
            Data.Added = new CollectionAddEvent<T>(this);
            Data.Removed = new CollectionRemoveEvent<T>(this);
            UIData.IndexValueChanged = new CollectionIndexValueChangeEvent<T>(this);
            UIData.Added = new CollectionAddEvent<T>(this);
            UIData.Removed = new CollectionRemoveEvent<T>(this);

            Data.IndexValueChanged.AddListener(OnDataIndexValueChanged);
            Data.Added.AddListener(OnDataAdded);
            Data.Removed.AddListener(OnDataRemoved);
            UIData.IndexValueChanged.AddListener(OnUIDataIndexValueChanged);
            UIData.Added.AddListener(OnUIDataAdded);
            UIData.Removed.AddListener(OnUIDataRemoved);
        }

        public void OnDataAdded(IEnumerable<T> items)
        {
            if (IsRevertable)
            {
                _dataEventQueue.Enqueue(() => OnDataAddedApply(items));
            }
            else OnDataAddedApply(items);
        }
        void OnDataAddedApply(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!UIData.Contains(item)) UIData.Add(item);
            }
        }

        public void OnDataIndexValueChanged(int index, T item)
        {
            if (IsRevertable) _dataEventQueue.Enqueue(() => OnDataIndexValueChangedApply(index, item));
            else OnDataIndexValueChangedApply(index, item);
        }
        void OnDataIndexValueChangedApply(int index, T item)
        {
            if (!UIData[index].Equals(item)) UIData[index] = item;
        }

        public void OnDataRemoved(IEnumerable<T> items)
        {
            if (IsRevertable) _dataEventQueue.Enqueue(() => OnDataRemovedApply(items));
            else OnDataRemovedApply(items);
        }
        void OnDataRemovedApply(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                UIData.Remove(item);
            }
        }

        public void OnUIDataAdded(IEnumerable<T> items)
        {
            if (IsRevertable) _uiDataEventQueue.Enqueue(() => OnUIDataAddedApply(items));
            else OnUIDataAddedApply(items);
        }
        void OnUIDataAddedApply(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (!Data.Contains(item)) Data.Add(item);
            }
        }

        public void OnUIDataIndexValueChanged(int index, T item)
        {
            if (IsRevertable) _uiDataEventQueue.Enqueue(() => OnUIDataIndexValueChangedApply(index, item));
            else OnUIDataIndexValueChangedApply(index, item);
        }
        void OnUIDataIndexValueChangedApply(int index, T item)
        {
            if (!Data[index].Equals(item)) Data[index] = item;
        }

        public void OnUIDataRemoved(IEnumerable<T> items)
        {
            if (IsRevertable) _uiDataEventQueue.Enqueue(() => OnUIDataRemovedApply(items));
            else OnUIDataRemovedApply(items);
        }
        void OnUIDataRemovedApply(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Data.Remove(item);
            }
        }

        public void Unbind()
        {
            Data.Removed = null;
            Data.Added = null;
            Data.IndexValueChanged = null;
            UIData.Removed = null;
            UIData.Added = null;
            UIData.IndexValueChanged = null;
        }

        public void ApplyToUI()
        {
            while (_dataEventQueue.Count > 0)
            {
                _dataEventQueue.Dequeue().Invoke();
            }
            _uiDataEventQueue.Clear();
        }

        public void ApplyToData()
        {
            while (_uiDataEventQueue.Count > 0)
            {
                _uiDataEventQueue.Dequeue().Invoke();
            }
            _dataEventQueue.Clear();
        }
    }
}
