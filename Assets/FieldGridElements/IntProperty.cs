using Assets.DataBinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class IntProperty : MonoBehaviour, IDisplayableProperty<int>, IUIData<int>
    {
        public Slider ValueSlider;
        public TextMeshProUGUI NameText;

#if UNITY_EDITOR
        public string UnityEditorName;
        public int UnityEditorMin;
        public int UnityEditorMax;
#endif

        public int Min
        {
            get => Mathf.RoundToInt(ValueSlider.minValue);
            set => ValueSlider.minValue = value;
        }
        public int Max
        {
            get => Mathf.RoundToInt(ValueSlider.maxValue);
            set => ValueSlider.maxValue = value;
        }
        public int Data
        {
            get => Mathf.RoundToInt(ValueSlider.value);
            set
            {
                ValueSlider.value = value;
            }
        }
        public string Name
        {
            get => NameText.text;
            set => NameText.text = value;
        }

        bool _pauseEvents = false;

        public GameObject DisplayObject => gameObject;

        public DataChangedEvent<int> DataChanged { get; set; }

        public bool IsBinded => DataChanged != null;

        public IData<int> GetData()
        {
            return DataChanged.Binder.Data;
        }

        public void OnUIDataChanged()
        {
            if (!_pauseEvents) DataChanged?.Invoke();
        }

        public void Refresh()
        {
        }

        public void PauseEvents()
        {
            _pauseEvents = true;
        }

        public void ResumeEvents()
        {
            _pauseEvents = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            ValueSlider.wholeNumbers = true;
            ValueSlider.onValueChanged.AddListener((f) => OnUIDataChanged());
        }

        // Update is called once per frame
#if UNITY_EDITOR
        void Update()
        {
            if (!EditorApplication.isPlaying)
            {
                Name = UnityEditorName;
                Min = UnityEditorMin;
                Max = UnityEditorMax;
            }
        }
#endif
    }
}