using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.FieldGridElements
{
    public interface IDisplayableProperty<T> : IDisplayableField
    {
        T Data { get; set; }
    }
}
