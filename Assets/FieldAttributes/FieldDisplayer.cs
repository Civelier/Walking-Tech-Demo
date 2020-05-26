using Assets.FieldGridElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Assets.DataBinding;

namespace Assets.FieldAttributes
{
    public class FieldDisplayer<TSerializable>
    {
        public readonly FieldGrid Grid;
        public readonly TSerializable SerializableObject;

        List<IDisplayableField> fields = new List<IDisplayableField>();
        List<IDataBinder> binders = new List<IDataBinder>();
        List<IData> datas = new List<IData>();

        public FieldDisplayer(FieldGrid grid, TSerializable serializableObject)
        {
            Grid = grid;
            SerializableObject = serializableObject;
        }

        IEnumerable<string> GetLabels(MemberInfo info)
        {
            foreach (var l in info.GetCustomAttributes<LabelAttribute>())
            {
                yield return l.Label;
            }
        }

        void Generate()
        {
            var members = SerializableObject.GetType().GetMembers();

            var properties = SerializableObject.GetType().GetProperties();
            var fields = SerializableObject.GetType().GetFields();
            var methods = SerializableObject.GetType().GetMethods();

            var pe = properties.GetEnumerator();
            var fe = fields.GetEnumerator();
            var me = methods.GetEnumerator();

            foreach (var member in members)
            {

                if (member.MemberType == MemberTypes.Field)
                {
                    fe.MoveNext();
                    var f = (FieldInfo)fe.Current;
                    if (f.GetCustomAttribute<NonSerializedAttribute>() == null)
                    {
                        var displayNameAttribute = f.GetCustomAttribute<DisplayNameAttribute>();
                        string[] labels = GetLabels(f).ToArray();
                        string name = displayNameAttribute?.Name ?? ObjectNames.NicifyVariableName(f.Name);
                        bool isReadonly = f.GetCustomAttribute<ReadonlyAttribute>() != null;
                    
                        if (isReadonly)
                        {
                            var data = new ValueTypeData();
                            data.Data = f.GetValue(SerializableObject);
                            var prop = FieldFactory.Instance.InstantiateReadOnly(Grid, name, data.Data);
                            binders.Add(DataBinderUtillities.Bind(data, prop));
                            this.fields.Add(prop);
                            datas.Add(data);
                            Grid.Add(prop);
                        }
                        else
                        {
                            if (f.FieldType == typeof(int))
                            {
                                var data = new ValueTypeData<int>();
                                data.Data = (int)f.GetValue(SerializableObject);
                                var rangeAttribute = f.GetCustomAttribute<RangeAttribute>();
                                var prop = FieldFactory.Instance.InstantiateInt(Grid, name, (int)rangeAttribute.Min, (int)rangeAttribute.Max, data.Data);
                                binders.Add(DataBinderUtillities.Bind(data, prop));
                                this.fields.Add(prop);
                                datas.Add(data);
                                Grid.Add(prop);
                                prop.DataChanged.AddListener(() => f.SetValue(SerializableObject, prop.Data));
                            }
                            if (f.FieldType == typeof(float))
                            {
                                var data = new ValueTypeData<float>();
                                data.Data = (float)f.GetValue(SerializableObject);
                                var rangeAttribute = f.GetCustomAttribute<RangeAttribute>();
                                var prop = FieldFactory.Instance.InstantiateFloat(Grid, name, rangeAttribute.Min, rangeAttribute.Max, data.Data);
                                binders.Add(DataBinderUtillities.Bind(data, prop));
                                this.fields.Add(prop);
                                datas.Add(data);
                                Grid.Add(prop);
                                prop.DataChanged.AddListener(() => f.SetValue(SerializableObject, prop.Data));
                            }
                        }
                    }
                }
                if (member.MemberType == MemberTypes.Property)
                {
                    pe.MoveNext();
                    var p = (PropertyInfo)pe.Current;
                    if (p.GetCustomAttribute<NonSerializedAttribute>() == null)
                    {
                        var displayNameAttribute = p.GetCustomAttribute<DisplayNameAttribute>();
                        string[] labels = GetLabels(p).ToArray();
                        string name = displayNameAttribute?.Name ?? ObjectNames.NicifyVariableName(p.Name);
                        bool isReadonly = p.GetCustomAttribute<ReadonlyAttribute>() != null || !p.CanWrite;

                        if (isReadonly)
                        {
                            var data = new ValueTypeData();
                            data.Data = p.GetValue(SerializableObject);
                            var prop = FieldFactory.Instance.InstantiateReadOnly(Grid, name, data.Data);
                            binders.Add(DataBinderUtillities.Bind(data, prop));
                            this.fields.Add(prop);
                            datas.Add(data);
                            Grid.Add(prop);
                        }
                        else
                        {
                            if (p.PropertyType == typeof(int))
                            {
                                var data = new ValueTypeData<int>();
                                data.Data = (int)p.GetValue(SerializableObject);
                                var rangeAttribute = p.GetCustomAttribute<RangeAttribute>();
                                var prop = FieldFactory.Instance.InstantiateInt(Grid, name, (int)rangeAttribute.Min, (int)rangeAttribute.Max, data.Data);
                                binders.Add(DataBinderUtillities.Bind(data, prop));
                                this.fields.Add(prop);
                                datas.Add(data);
                                Grid.Add(prop);
                                prop.DataChanged.AddListener(() => p.SetValue(SerializableObject, prop.Data));
                            }
                            if (p.PropertyType == typeof(float))
                            {
                                var data = new ValueTypeData<float>();
                                data.Data = (float)p.GetValue(SerializableObject);
                                var rangeAttribute = p.GetCustomAttribute<RangeAttribute>();
                                var prop = FieldFactory.Instance.InstantiateFloat(Grid, name, rangeAttribute.Min, rangeAttribute.Max, data.Data);
                                binders.Add(DataBinderUtillities.Bind(data, prop));
                                this.fields.Add(prop);
                                datas.Add(data);
                                Grid.Add(prop);
                                prop.DataChanged.AddListener(() => p.SetValue(SerializableObject, prop.Data));
                            }
                        }
                    }
                }
                if (member.MemberType == MemberTypes.Method)
                {
                    me.MoveNext();
                    if (member.GetCustomAttribute<DisplayableInFieldGridAttribute>() != null)
                    {
                        var m = (MethodInfo)me.Current;
                        if (m.GetCustomAttribute<NonSerializedAttribute>() == null)
                        {
                            var displayNameAttribute = m.GetCustomAttribute<DisplayNameAttribute>();
                            string[] labels = GetLabels(m).ToArray();
                            string name = displayNameAttribute?.Name ?? ObjectNames.NicifyVariableName(m.Name);
                            if (m.GetParameters().Length == 0)
                            {
                                var field = FieldFactory.Instance.InstantiateButtonField(Grid, name, () => m.Invoke(SerializableObject, new object[0]));
                                this.fields.Add(field);
                                Grid.Add(field);
                            }
                        }
                    }
                }
            }
        }

        public void Destroy()
        {
            Grid.Clear();
            UnityEngine.Object.Destroy(Grid);
        }

        public void Display()
        {
            Generate();
        }

        public void Update()
        {
            foreach (var d in datas)
            {
                d.OnDataChanged();
            }
        }
    }
}
