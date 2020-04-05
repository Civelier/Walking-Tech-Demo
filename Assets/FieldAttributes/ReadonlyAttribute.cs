using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.FieldAttributes
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class ReadonlyAttribute : Attribute
    {
        /// <summary>
        /// The UI will not be able to modify that property
        /// </summary>
        public ReadonlyAttribute()
        {
        }
    }
}
