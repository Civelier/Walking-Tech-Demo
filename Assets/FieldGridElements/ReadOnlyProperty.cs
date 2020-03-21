using Assets.DataBinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class ReadOnlyProperty : MonoBehaviour, IDisplayableField, IUIData<object>
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI ValueText;

#if UNITY_EDITOR
        public string UnityEditorName;
#endif
        public string Name 
        {
            get => NameText.text;
            set => NameText.text = value;
        }

        object _data;
        public object Data
        {
            get => _data;
            set
            {
                _data = value;
                OnUIDataChanged();
            }
        }

        public GameObject DisplayObject => gameObject;

        public bool IsBinded => DataChanged != null;

        public DataChangedEvent<object> DataChanged { get; set; }

        // Start is called before the first frame update
        void Start()
        {

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
        public void Refresh()
        {
            ValueText.text = _data.ToString();
        }

        public void OnUIDataChanged()
        {
            DataChanged?.Invoke();
        }

        public IData<object> GetData()
        {
            return DataChanged?.Binder.Data;
        }

        public void SetValueWithoutNotify(object value)
        {
            _data = value;
            Refresh();
        }
    }
}