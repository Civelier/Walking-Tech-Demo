using Assets.DataBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.FieldGridElements
{
    [ExecuteInEditMode]
    public class EnumProperty : MonoBehaviour, IDisplayableProperty<int>, IUIData<int>
    {
#if UNITY_EDITOR
        public string UnityNameText;
#endif

        public TextMeshProUGUI NameText;
        public TMP_Dropdown EnumValue;
        private bool _pauseEvents = false;
        public Type EnumType;
        private Dictionary<string, int> _values = new Dictionary<string, int>();

        public int Data
        {
            get => _values[EnumValue.options[EnumValue.value].text];
            set
            {
                int i = 0;
                foreach (var keyValue in _values)
                {
                    if (keyValue.Value == value) break;
                    i++;
                }
                EnumValue.value = i;
                OnUIDataChanged();
            }
        }

        public string Name
        {
            get => NameText.text;
            set => NameText.text = value;
        }

        public bool IsBinded => DataChanged != null;

        public GameObject DisplayObject => gameObject;

        public DataChangedEvent<int> DataChanged { get; set; }

        public IData<int> GetData()
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

        IEnumerable<string> ConvertToDisplayText(IEnumerable<string> text)
        {
            foreach (var t in text)
            {
                var sb = new StringBuilder();
                foreach (var c in t)
                {
                    if (c == '_') sb.Append(' ');
                    else sb.Append(c);
                }
                yield return sb.ToString();
            }
        }

        private void Start()
        {
            EnumValue.options.Clear();
            var names = ConvertToDisplayText(Enum.GetNames(EnumType)).ToList();
            EnumValue.AddOptions(names.ToList());
            EnumValue.onValueChanged.AddListener((i) => OnUIDataChanged());

            for (int i = 0; i < names.Count; i++)
            {
                _values.Add(names[i], (int)Enum.GetValues(EnumType).GetValue(i));
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            Name = UnityNameText;
        }
#endif
    }
}
