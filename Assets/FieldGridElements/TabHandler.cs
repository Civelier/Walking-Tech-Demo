using Assets.DataBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.FieldGridElements
{
    public class TabHandler : MonoBehaviour, ICollectionUIData<ContainerComponent>
    {
        public GameObject[] ContainerComponents;
        public TMP_Dropdown TabDropDown;
        public RectTransform Tabs;

        List<ContainerComponent> _components = new List<ContainerComponent>();

        int _value = 0;
        public int Value
        {
            get => _value;
            set
            {
                if (Current != null) Current.Visible = false;
                _value = value;
                if (Current != null) Current.Visible = true;
            }
        }

        public ContainerComponent Current => Count <= _value ? null : _components[_value];

        public int Count => _components.Count;

        public bool IsReadOnly => false;

        public CollectionAddEvent<ContainerComponent> Added { get; set; }
        public CollectionRemoveEvent<ContainerComponent> Removed { get; set; }
        public CollectionIndexValueChangeEvent<ContainerComponent> IndexValueChanged { get; set; }

        public bool IsBinded => IndexValueChanged != null;

        public ContainerComponent this[int index]
        {
            get => _components[index];
            set
            {
                _components[index] = value;
                OnIndexValueChanged(index, value);
            }
        }

        private void Start()
        {
            TabDropDown.onValueChanged.AddListener((index) => Value = index);
            foreach (var obj in ContainerComponents)
            {
                if (obj.TryGetComponent(out ContainerComponent container))
                {
                    Add(container);
                }
            }
        }

        private void Update()
        {
            
        }

        public void Add(ContainerComponent item)
        {
            _components.Add(item);
            item.gameObject.transform.parent = Tabs;
            OnAdded(new[] { item });
            if (Current == null) Value = Count - 1;
            foreach (var component in _components)
            {
                component.Visible = false;
            }
            Current.Visible = true;
        }

        public void AddRange(IEnumerable<ContainerComponent> items)
        {
            foreach (var item in items)
            {
                _components.Add(item);
                item.gameObject.transform.parent = transform;
            }
            OnAdded(items);
            if (Current == null) Value = Count - 1;
        }

        public void InstantiateAndAdd(GameObject obj)
        {
            var instance = Instantiate(obj, Tabs);
            _components.Add(instance.GetComponent<ContainerComponent>());
            OnAdded(new[] { _components.LastOrDefault() });
            if (Current == null) Value = Count - 1;
        }

        public void Clear()
        {
            var a = _components.ToArray();
            _components.Clear();
            OnRemoved(a);
            foreach (var component in _components)
            {
                Destroy(component.gameObject);
            }
            Value = 0;
        }

        public bool Contains(ContainerComponent item)
        {
            return _components.Contains(item);
        }

        public void CopyTo(ContainerComponent[] array, int arrayIndex)
        {
            _components.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ContainerComponent> GetEnumerator()
        {
            return _components.GetEnumerator();
        }

        public bool Remove(ContainerComponent item)
        {
            if (_components.IndexOf(item) <= _value) Value = Count > 1 ? _value - 1 : 0;
            if (_components.Remove(item))
            {
                Destroy(item);
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void OnAdded(IEnumerable<ContainerComponent> items)
        {
            Added?.Invoke(items);

            var list = new List<string>();
            foreach (var item in items)
            {
                list.Add(item.Name);
            }

            TabDropDown.AddOptions(list);
        }

        public void OnRemoved(IEnumerable<ContainerComponent> items)
        {
            Removed?.Invoke(items);
            foreach (var item in items)
            {
                TabDropDown.options.RemoveAt(TabDropDown.options.FindIndex((data) => data.text == item.Name));
            }
        }

        public void OnIndexValueChanged(int index, ContainerComponent item)
        {
            IndexValueChanged?.Invoke(index, item);
            TabDropDown.options[index].text = item.Name;
        }

        public ICollectionData<ContainerComponent> GetCollectionData()
        {
            return IndexValueChanged?.Binder.Data;
        }
    }
}
