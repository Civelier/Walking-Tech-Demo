using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.DataBinding
{
    public interface IUIData : IUIBindable
    {
        void OnUIDataChanged();
    }

    public interface IUIData<T> : IUIData
    {
        DataChangedEvent<T> DataChanged { get; set; }
        T Data { get; set; }
        IData<T> GetData();
    }
}
