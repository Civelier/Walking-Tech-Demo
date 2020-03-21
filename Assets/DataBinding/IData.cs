using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public interface IData : IBindable
    {
        void OnDataChanged();
    }
    public interface IData<T> : IData
    {
        DataChangedEvent<T> DataChanged { get; set; }
        T Data { get; set; }
        IUIData<T> GetUIData();
    }
}
