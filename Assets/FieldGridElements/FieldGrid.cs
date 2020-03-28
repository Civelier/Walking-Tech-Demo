using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Assets.DataBinding;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class FieldGrid : ContainerComponent, ICollectionUIData<IDisplayableField>
    {
        public ScrollRect ScrollRect;
        protected List<IDisplayableField> _fields = new List<IDisplayableField>();
        public TextMeshProUGUI TitleText;

        public override string Name
        {
            get => TitleText.text == "" ? UnityEditorName : TitleText.text;
            set => TitleText.text = value;
        }

        public string UnityEditorName;

        public GameObject DisplayObject => gameObject;

        public bool IsBinded => false;

        public override bool Visible
        {
            get => DisplayObject.activeSelf;
            set => DisplayObject.SetActive(value);
        }
        public CollectionAddEvent<IDisplayableField> Added { get; set; }
        public CollectionRemoveEvent<IDisplayableField> Removed { get; set; }
        public CollectionIndexValueChangeEvent<IDisplayableField> IndexValueChanged { get; set; }

        public int Count => _fields.Count;

        public bool IsReadOnly => false;

        public IDisplayableField this[int index]
        {
            get => _fields[index];
            set
            {
                _fields[index] = value;
                OnIndexValueChanged(index, value);
            }
        }

        private IEnumerable<float> GetHeights()
        {
            foreach (var field in _fields)
            {
                yield return field.DisplayObject.GetComponent<RectTransform>().rect.height;
            }
        }

        public void Clear()
        {
            var a = _fields.ToArray();
            //foreach (var field in _fields)
            //{
            //    //field.DisplayObject.SetActive(true);
            //    DestroyImmediate(field.DisplayObject.transform);
            //}
            for (int i = 0; i < ScrollRect.content.childCount; i++)
            {
                DestroyImmediate(ScrollRect.content.GetChild(i).gameObject);
            }
            _fields.Clear();
            OnRemoved(a);
            Refresh();
        }

        float NextPosY(float itemHeight)
        {
            float sum = 5;
            foreach (var height in GetHeights())
            {
                sum += height + 5;
            }
            return sum + itemHeight / 2;
        }

        void UpdateSize()
        {
            float sum = 5;
            foreach (var height in GetHeights())
            {
                sum += height + 5;
            }
            ScrollRect.content.sizeDelta = new Vector2(0, sum);
        }

        public void Add(IDisplayableField field)
        {
            var r = field.DisplayObject.GetComponent<RectTransform>();
            field.DisplayObject.transform.Translate(0, -NextPosY(r.rect.height), 0);
            _fields.Add(field);
            UpdateSize();
            Refresh();
            OnAdded(new[] { field });
        }

        public bool Remove(IDisplayableField field)
        {
            var f = _fields.IndexOf(field);
            var range = _fields.GetRange(f, _fields.Count - f);
            var height = field.DisplayObject.GetComponent<RectTransform>().rect.height;
            foreach (var item in range)
            {
                item.DisplayObject.transform.Translate(0, height + 5, 0);
            }
            Destroy(field.DisplayObject);
            if (_fields.Remove(field))
            {
                UpdateSize();
                OnRemoved(new[] { field });
                Refresh();
            }
            return false;
        }

        private void Start()
        {
            Name = UnityEditorName;
        }

        // Update is called once per frame
#if UNITY_EDITOR
        void Update()
        {
            if (!EditorApplication.isPlaying)
            {
                Name = UnityEditorName;
                Refresh();
            }
        }
#endif

        public override void Refresh()
        {
            foreach (var field in _fields)
            {
                field.Refresh();
            }
        }

        public void OnAdded(IEnumerable<IDisplayableField> items)
        {
            Added?.Invoke(items);
        }

        public void OnRemoved(IEnumerable<IDisplayableField> items)
        {
            Removed?.Invoke(items);
        }

        public void OnIndexValueChanged(int index, IDisplayableField item)
        {
            IndexValueChanged?.Invoke(index, item);
        }

        public ICollectionData<IDisplayableField> GetCollectionData()
        {
            return IndexValueChanged?.Binder.Data;
        }

        public bool Contains(IDisplayableField item)
        {
            return _fields.Contains(item);
        }

        public void CopyTo(IDisplayableField[] array, int arrayIndex)
        {
            _fields.CopyTo(array, arrayIndex);
        }

        public IEnumerator<IDisplayableField> GetEnumerator()
        {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}