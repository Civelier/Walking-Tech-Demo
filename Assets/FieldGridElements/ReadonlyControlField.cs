using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.DataBinding;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Assets.GameMenu;
using UnityEditor;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class ReadonlyControlField : MonoBehaviour, IUIData<InputData>, IDisplayableField
    {
        public TextMeshProUGUI ControlName;
        public TextMeshProUGUI KeyboardKey;
        public TextMeshProUGUI XboxControl;
        public Image KeyboardIcon;
        public Image XboxIcon;
        public InputActionReference Reference;
        public string NameOverwrite = "";
        public DataChangedEvent<InputData> DataChanged { get; set; }

        public InputData _data;
        public InputData Data
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

        public string Name { get => _data.ControlName; set => _data.ControlName = value; }

        public IData<InputData> GetData()
        {
            return DataChanged?.Binder.Data;
        }

        public void OnUIDataChanged()
        {
            ControlName.text = NameOverwrite == "" ? Data.ControlName : NameOverwrite;
            if (Data.Displayers[0].DisplayAs == DisplayType.Image)
            {
                KeyboardIcon.enabled = true;
                KeyboardKey.enabled = false;
                KeyboardIcon.sprite = Data.Displayers[0].Image;
            }
            else
            {
                KeyboardIcon.enabled = false;
                KeyboardKey.enabled = true;
                KeyboardKey.text = Data.KeyboardControl;
            }

            if (Data.Displayers[1].DisplayAs == DisplayType.Image)
            {
                XboxIcon.enabled = true;
                XboxControl.enabled = false;
                XboxIcon.sprite = Data.Displayers[1].Image;
            }
            else
            {
                XboxIcon.enabled = false;
                XboxControl.enabled = true;
                XboxControl.text = Data.XboxControl;
            }
            DataChanged?.Binder.OnUIDataChanged();
        }

        public void SetKeyboardDisplayer(InputDisplayer displayer)
        {
            _data.SetKeyboardDisplayer(displayer);
            OnUIDataChanged();
        }

        public void SetControllerDisplayer(InputDisplayer displayer)
        {
            _data.SetXboxDisplayer(displayer);
            OnUIDataChanged();
        }

        public void SetDisplayer(InputDisplayer displayer)
        {
            if (displayer.Selection.SelectionIndex == 0) _data.SetKeyboardDisplayer(displayer);
            else _data.SetXboxDisplayer(displayer);
            OnUIDataChanged();
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
            //if (Reference != null) _data = new InputData(Reference.action);
            if (!EditorApplication.isPlaying) OnUIDataChanged();
        }

#endif
        public void Refresh()
        {
        }
    }
}
