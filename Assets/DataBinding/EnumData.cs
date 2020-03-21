using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public class EnumData : IData<int>
    {
        public DataChangedEvent<int> DataChanged { get; set; }

        int _data;
        public int Data
        {
            get => _data;
            set
            {
                _data = value;
            }
        }

        public bool IsBinded => DataChanged != null;

        public IUIData<int> GetUIData()
        {
            return DataChanged?.Binder.UIData;
        }

        public void SetValueWithoutNotify(int value)
        {
            _data = value;
        }

        public void OnDataChanged()
        {
            DataChanged.Invoke();
        }
    }
}
