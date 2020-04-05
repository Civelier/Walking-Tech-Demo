using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.FieldAttributes
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class RangeAttribute : Attribute
    {
        public readonly float Min;
        public readonly float Max;
        public RangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
