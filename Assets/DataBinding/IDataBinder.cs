using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public interface IDataBinder
    {
        void Unbind();
    }

    public interface IDataBinder<T> : IDataBinder
    {
        IData<T> Data { get; }
        IUIData<T> UIData { get; }
        void OnUIDataChanged();
        void OnDataChanged();
    }
}
