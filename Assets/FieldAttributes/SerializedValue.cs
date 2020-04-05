using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.FieldAttributes
{
    [Serializable]
    public class SerializedValue : UnityEngine.Object
    {
        public object Value;
        public Type TypeOfValue;

        public SerializedValue(object value, Type type, string name = null) : base()
        {
            Value = value;
            TypeOfValue = type;
            if (name != null)
            {
                this.name = name;
            }
        }

    }
}
