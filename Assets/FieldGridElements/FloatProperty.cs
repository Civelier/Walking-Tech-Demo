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
    public class FloatProperty : MonoBehaviour, IDisplayableProperty<float>, IUIData<float>
    {
        public Slider ValueSlider;
        public TextMeshProUGUI NameText;

#if UNITY_EDITOR
        public string UnityEditorName;
        public float UnityEditorMin;
        public int UnityEditorMax;
#endif
        private bool _pauseEvents = false;

        public float Min
        {
            get => ValueSlider.minValue;
            set => ValueSlider.minValue = value;
        }
        public float Max
        {
            get => ValueSlider.maxValue;
            set => ValueSlider.maxValue = value;
        }
        public float Data
        {
            get => ValueSlider.value;
            set
            {
                ValueSlider.value = value;
                OnUIDataChanged();
            }
        }
        public string Name
        {
            get => NameText.text;
            set => NameText.text = value;
        }

        public GameObject DisplayObject => gameObject;

        public bool IsBinded => DataChanged != null;

        public DataChangedEvent<float> DataChanged { get; set; }

        public IData<float> GetData()
        {
            return DataChanged?.Binder.Data;
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

        void Start()
        {
            ValueSlider.wholeNumbers = false;
            ValueSlider.onValueChanged.AddListener((f) => OnUIDataChanged());
        }

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