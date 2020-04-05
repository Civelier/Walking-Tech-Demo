using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.FieldAttributes
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class DisplayableInFieldGridAttribute : Attribute
    {
        /// <summary>
        /// Tags the class that it can be displayed in a <see cref="FieldGridElements.FieldGrid"/>
        /// </summary>
        public DisplayableInFieldGridAttribute()
        {
        }
    }
}
