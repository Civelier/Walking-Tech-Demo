using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataBinding
{
    public static class DataBinderUtillities
    {
        public static IDataBinder<T> Bind<T>(IData<T> data, IUIData<T> uiData)
        {
            return new BasicDataBinder<T>(data, uiData, false);
        }

        public static ICollectionDataBinder<T> CollectionBind<T>(ICollectionData<T> data, ICollectionUIData<T> uiData)
        {
            return new BasicCollectionDataBinder<T>(data, uiData, false);
        }

        public static IRevertableBinder RevertableBind<T>(IData<T> data, IUIData<T> uiData)
        {
            return new BasicDataBinder<T>(data, uiData, true);
        }

        public static IRevertableBinder RevertableCollectionBind<T>(ICollectionData<T> data, ICollectionUIData<T> uiData)
        {
            return new BasicCollectionDataBinder<T>(data, uiData, true);
        }
    }
}
