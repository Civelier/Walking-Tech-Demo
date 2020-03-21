using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public interface ICollectionDataBinder<T> : IDataBinder
    {
        ICollectionData<T> Data { get; }
        ICollectionUIData<T> UIData { get; }
        void OnDataAdded(IEnumerable<T> items);
        void OnDataRemoved(IEnumerable<T> items);
        void OnDataIndexValueChanged(int index, T item);
        void OnUIDataAdded(IEnumerable<T> items);
        void OnUIDataRemoved(IEnumerable<T> items);
        void OnUIDataIndexValueChanged(int index, T item);
        void Unbind();
    }
}
