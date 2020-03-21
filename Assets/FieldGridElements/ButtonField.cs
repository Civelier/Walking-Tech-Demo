using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.FieldGridElements
{
    public class ButtonField : MonoBehaviour, IDisplayableField
    {
        public Button ActionButton;
        public TextMeshProUGUI ActionText;

        public string UnityEditorText;
        
        public string Name
        {
            get => ActionText.text == "" ? UnityEditorText : ActionText.text;
            set => ActionText.text = value;
        }
        public GameObject DisplayObject => gameObject;

        public bool IsBinded => false;

        public void OnUIDataChanged()
        {
        }

        public void Refresh()
        {
        }

        private void Start()
        {
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                ActionText.text = UnityEditorText;
            }
        }
#endif
    }
}
