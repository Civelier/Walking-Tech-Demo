using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Assets.DataBinding
{
    public class DataChangedEvent<T> : UnityEvent
    {
        public readonly IDataBinder<T> Binder;
        public DataChangedEvent(IDataBinder<T> binder) : base()
        {
            Binder = binder;
        }
    }

    public class CollectionAddEvent<TItemType> : UnityEvent<IEnumerable<TItemType>>
    {
        public readonly ICollectionDataBinder<TItemType> Binder;

        public CollectionAddEvent(ICollectionDataBinder<TItemType> binder) : base()
        {
            Binder = binder;
        }
    }

    public class CollectionRemoveEvent<TItemType> : UnityEvent<IEnumerable<TItemType>>
    {
        public readonly ICollectionDataBinder<TItemType> Binder;

        public CollectionRemoveEvent(ICollectionDataBinder<TItemType> binder) : base()
        {
            Binder = binder;
        }
    }

    public class CollectionIndexValueChangeEvent<TItemType> : UnityEvent<int, TItemType>
    {
        public readonly ICollectionDataBinder<TItemType> Binder;

        public CollectionIndexValueChangeEvent(ICollectionDataBinder<TItemType> binder) : base()
        {
            Binder = binder;
        }
    }
}
