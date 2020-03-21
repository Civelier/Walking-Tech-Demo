using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public class ListSelectionData : ICollectionData<object>, IData<object>
    {
        List<object> _list = new List<object>();
        public object this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                OnIndexValueChanged(index, value);
            }
        }

        public CollectionAddEvent<object> Added { get; set; }
        public CollectionRemoveEvent<object> Removed { get; set; }
        public CollectionIndexValueChangeEvent<object> IndexValueChanged { get; set; }

        public bool IsBinded => IndexValueChanged != null && DataChanged != null;

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        int _index = 0;

        public DataChangedEvent<object> DataChanged { get; set; }
        public object Data
        {
            get => _index < Count ? _list[_index] : null;
            set => Index = _list.IndexOf(value);
        }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnDataChanged();
            }
        }
        public void Add(object item)
        {
            _list.Add(item);
            OnAdded(new[] { item });
        }

        public void Clear()
        {
            var objs = _list.ToArray();
            _list.Clear();
            OnRemoved(objs);
        }

        public bool Contains(object item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public ICollectionUIData<object> GetCollectionUIData()
        {
            return IndexValueChanged?.Binder.UIData;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public IUIData<object> GetUIData()
        {
            return DataChanged?.Binder.UIData;
        }

        public void OnAdded(IEnumerable<object> items)
        {
            Added?.Invoke(items);
        }

        public void OnDataChanged()
        {
            DataChanged?.Invoke();
        }

        public void OnIndexValueChanged(int index, object item)
        {
            IndexValueChanged?.Invoke(index, item);
        }

        public void OnRemoved(IEnumerable<object> items)
        {
            Removed?.Invoke(items);
        }

        public bool Remove(object item)
        {
            if (_list.Remove(item))
            {
                OnRemoved(new[] { item });
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
