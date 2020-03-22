using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.DataBinding;
using UnityEngine.InputSystem;
using TMPro;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class ReadonlyControlField : MonoBehaviour, IUIData<InputData>, IDisplayableField
    {
        public TextMeshProUGUI ControlName;
        public TextMeshProUGUI KeyboardKey;
        public TextMeshProUGUI XboxControl;
        public InputActionReference Reference;
        public string NameOverwrite = "";
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

        public string Name { get => _data.ControlName; set => _data.ControlName = value; }

        public IData<InputData> GetData()
        {
            return DataChanged?.Binder.Data;
        }

        public void OnUIDataChanged()
        {
            ControlName.text = NameOverwrite == "" ? Data.ControlName : NameOverwrite;
            KeyboardKey.text = Data.KeyboardControl;
            XboxControl.text = Data.XboxControl;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Reference != null) _data = new InputData(Reference.action);
            OnUIDataChanged();
        }

#if UNITY_EDITOR
        // Update is called once per frame
        void Update()
        {
            if (Reference != null) _data = new InputData(Reference.action);
            OnUIDataChanged();
        }

#endif
        public void Refresh()
        {
        }
    }
}
