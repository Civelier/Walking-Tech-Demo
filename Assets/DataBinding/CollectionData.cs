using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public class CollectionData<T> : ICollectionData<T>
    {
        List<T> _list = new List<T>();
        public T this[int index]
        {
            get => _list[index];
            set
            {
                _list[index] = value;
                OnIndexValueChanged(index, value);
            }
        }

        public CollectionAddEvent<T> Added { get; set; }
        public CollectionRemoveEvent<T> Removed { get; set; }
        public CollectionIndexValueChangeEvent<T> IndexValueChanged { get; set; }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _list.Add(item);
            OnAdded(new[] { item });
        }

        public void Clear()
        {
            var a = _list.ToArray();
            _list.Clear();
            OnRemoved(a);
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public ICollectionUIData<T> GetCollectionUIData()
        {
            return IndexValueChanged?.Binder.UIData;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void OnAdded(IEnumerable<T> items)
        {
            Added?.Invoke(items);
        }

        public void OnIndexValueChanged(int index, T item)
        {
            IndexValueChanged?.Invoke(index, item);
        }

        public void OnRemoved(IEnumerable<T> items)
        {
            Removed?.Invoke(items);
        }

        public bool Remove(T item)
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
