using Assets.DataBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class ListSelectionProperty : MonoBehaviour, IDisplayableProperty<object>, ICollectionUIData<object>, IUIData<object>
    {
        List<object> _list = new List<object>();

        public TextMeshProUGUI NameText;
        public TMP_Dropdown Values;

#if UNITY_EDITOR
        public string UnityEditorName;
#endif

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
                Values.SetValueWithoutNotify(value);
                OnUIDataChanged();
            }
        }

        public GameObject DisplayObject => gameObject;
        public string Name
        {
            get => NameText.text;
            set => NameText.text = value;
        }

        private void Start()
        {
            Values.onValueChanged.AddListener((i) => Index = i);
        }

        public void Add(object item)
        {
            _list.Add(item);
            Values.AddOptions(new List<string>() { item.ToString() });
            OnAdded(new[] { item });
        }

        public void Clear()
        {
            var objs = _list.ToArray();
            _list.Clear();
            Values.ClearOptions();
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

        public ICollectionData<object> GetCollectionData()
        {
            return IndexValueChanged?.Binder.Data;
        }

        public IData<object> GetData()
        {
            return DataChanged?.Binder.Data;
        }

        public IEnumerator<object> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void OnAdded(IEnumerable<object> items)
        {
            Added?.Invoke(items);
        }

        public void OnIndexValueChanged(int index, object item)
        {
            IndexValueChanged?.Invoke(index, item);
        }

        public void OnRemoved(IEnumerable<object> items)
        {
            Removed?.Invoke(items);
        }

        public void OnUIDataChanged()
        {
            DataChanged?.Invoke();
        }

        public bool Remove(object item)
        {
            if (_list.Remove(item))
            {
                OnRemoved(new[] { item });
                Values.options.RemoveAt(Values.options.FindIndex((p) => p.text == item.ToString()));
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Refresh()
        {
        }

#if UNITY_EDITOR
        void Update()
        {
            if (!EditorApplication.isPlaying)
            {
                Name = UnityEditorName;
            }
        }
#endif
    }
}
