using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public class BasicDataBinder<T> : IDataBinder<T>, IRevertableBinder
    {
        private bool _isRevertable;

        public IData<T> Data { get; private set; }

        public IUIData<T> UIData { get; private set; }

        public bool IsRevertable => _isRevertable;

        public void OnDataChanged()
        {
            if (!IsRevertable) OnDataChangedApply();
        }
        void OnDataChangedApply()
        {
            if (!(UIData.Data?.Equals(Data.Data) ?? false)) UIData.Data = Data.Data;
        }

        public void OnUIDataChanged()
        {
            if (!IsRevertable) OnUIDataChangedApply();
        }
        void OnUIDataChangedApply()
        {
            if (!(Data.Data?.Equals(UIData.Data) ?? false)) Data.Data = UIData.Data;
        }

        public void Unbind()
        {
            Data.DataChanged = null;
            UIData.DataChanged = null;
        }

        public void ApplyToUI()
        {
            OnDataChangedApply();
        }

        public void ApplyToData()
        {
            OnUIDataChangedApply();
        }

        public BasicDataBinder(IData<T> data, IUIData<T> uiData, bool isRevertable)
        {
            _isRevertable = isRevertable;
            Data = data;
            UIData = uiData;
            Data.DataChanged = new DataChangedEvent<T>(this);
            UIData.DataChanged = new DataChangedEvent<T>(this);
            Data.DataChanged.AddListener(OnDataChanged);
            UIData.DataChanged.AddListener(OnUIDataChanged);
        }
    }
}
