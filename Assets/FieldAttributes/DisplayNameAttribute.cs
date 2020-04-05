using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.FieldAttributes
{
    [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class DisplayNameAttribute : Attribute
    {
        readonly string name;

        /// <summary>
        /// Overwrites the default display name of a field
        /// </summary>
        /// <param name="name">Name to display</param>
        public DisplayNameAttribute(string name)
        {
            this.name = name;
        }

        public string Name => name;
    }
}
