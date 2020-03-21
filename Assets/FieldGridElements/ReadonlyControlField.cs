using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.DataBinding;
using UnityEngine.InputSystem;
using TMPro;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class ReadonlyControlField : MonoBehaviour, IUIData<InputData>
    {
        public TextMeshProUGUI ControlName;
        public TextMeshProUGUI KeyboardKey;
        public TextMeshProUGUI XboxControl;
        public InputActionReference Reference;
        public DataChangedEvent<InputData> DataChanged { get; set; }

        internal InputData _data;
        public InputData Data
        { 
            get => _data;
            set
            {
            }
        }

    public GameObject DisplayObject => gameObject;

        public bool IsBinded => DataChanged != null;

        public IData<InputData> GetData()
        {
            return DataChanged?.Binder.Data;
        }

        public void OnUIDataChanged()
        {
            ControlName.text = Data.ControlName;
            KeyboardKey.text = Data.KeyboardControl;
            XboxControl.text = Data.XboxControl;
        }

        // Start is called before the first frame update
        void Start()
        {
            _data = new InputData(Reference.action);
            OnUIDataChanged();
        }

#if UNITY_EDITOR
        // Update is called once per frame
        void Update()
        {
            _data = new InputData(Reference.action);
            OnUIDataChanged();
        }
#endif
    }
}
