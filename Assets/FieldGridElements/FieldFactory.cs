using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.FieldGridElements
{
    [DefaultExecutionOrder(1)]
    public class FieldFactory : MonoBehaviour
    {
        public static FieldFactory Instance;

        public GameObject IntPropertyPrefab;
        public GameObject FloatPropertyPrefab;
        public GameObject ReadonlyPropertyPrefab;
        public GameObject FieldGridPrefab;
        public GameObject SongFieldPrefab;
        public GameObject EnumPropertyPrefab;
        public GameObject ButtonFieldPrefab;
        public GameObject ListSelectionPrefab;
        public FieldFactory()
        {

        }

        public IntProperty InstantiateInt(FieldGrid grid, string name, int min, int max, int value = 0)
        {
            IntProperty obj = Instantiate(IntPropertyPrefab, grid.ScrollRect.content.transform).GetComponent<IntProperty>();
            obj.Name = name;
            obj.Min = min;
            obj.Max = max;
            obj.Data = value;
            return obj;
        }

        public FloatProperty InstantiateFloat(FieldGrid grid, string name, float min, float max, float value = 0)
        {
            FloatProperty obj = Instantiate(FloatPropertyPrefab, grid.ScrollRect.content.transform).GetComponent<FloatProperty>();
            obj.Name = name;
            obj.Min = min;
            obj.Max = max;
            obj.Data = value;
            return obj;
        }

        public ReadOnlyProperty InstantiateReadOnly(FieldGrid grid, string name, object value)
        {
            ReadOnlyProperty obj = Instantiate(ReadonlyPropertyPrefab, grid.ScrollRect.content.transform).GetComponent<ReadOnlyProperty>();
            obj.Name = name;
            obj.Data = value;
            return obj;
        }

        public EnumProperty InstantiateEnumField<T>(FieldGrid grid, string name, T value) where T : Enum
        {
            EnumProperty obj = Instantiate(EnumPropertyPrefab, grid.ScrollRect.content.transform).GetComponent<EnumProperty>();
            obj.Name = name;
            obj.EnumType = value.GetType();
            obj.Data = (int)Enum.Parse(obj.EnumType, Enum.GetName(obj.EnumType, value));
            return obj;
        }

        public ButtonField InstantiateButtonField(FieldGrid grid, string name, UnityAction action)
        {
            ButtonField obj = Instantiate(ButtonFieldPrefab, grid.ScrollRect.content.transform).GetComponent<ButtonField>();
            obj.Name = name;
            obj.ActionButton.onClick.AddListener(action);
            return obj;
        }

        public ListSelectionProperty InstantiateListSelection(FieldGrid grid, string name)
        {
            ListSelectionProperty obj = Instantiate(ListSelectionPrefab, grid.ScrollRect.content.transform).GetComponent<ListSelectionProperty>();
            obj.Name = name;
            return obj;
        }

        void Start()
        {
            Instance = this;
        }
    }
}
