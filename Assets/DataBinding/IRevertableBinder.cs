using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public interface IRevertableBinder : IDataBinder
    {
        bool IsRevertable { get; }
        void ApplyToUI();
        void ApplyToData();
    }
}
