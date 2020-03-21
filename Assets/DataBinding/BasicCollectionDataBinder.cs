using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.DataBinding
{
    public class BasicCollectionDataBinder<T> : ICollectionDataBinder<T>, IRevertableBinder
    {
        private bool _isRevertable;

        public ICollectionData<T> Data { get; private set; }

        public ICollectionUIData<T> UIData { get; private set; }

        public bool IsRevertable => _isRevertable;

        Queue<UnityAction> _uiDataEventQueue = new Queue<UnityAction>();
        Queue<UnityAction> _dataEventQueue = new Queue<UnityAction>();

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
