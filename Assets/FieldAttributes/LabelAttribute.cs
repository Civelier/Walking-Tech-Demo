using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.FieldAttributes
{
    
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    sealed class LabelAttribute : Attribute
    {
        readonly string label;

        /// <summary>
        /// Adds a label in between fields
        /// </summary>
        /// <param name="label">The text to show</param>
        public LabelAttribute(string label)
        {
            this.label = label;
        }

        public string Label
        {
            get { return label; }
        }
    }
}
