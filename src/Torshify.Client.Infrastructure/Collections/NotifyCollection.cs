using System.Collections.Generic;
using System.Collections.ObjectModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Collections
{
    public class NotifyCollection<T> : ObservableCollection<T>, INotifyEnumerable<T>
    {
        #region Fields

        private object _lockObject = new object();

        #endregion Fields

        #region Constructors

        public NotifyCollection()
        {
        }

        public NotifyCollection(List<T> list)
            : base(list)
        {
        }

        public NotifyCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        #endregion Constructors

        #region Properties

        public object SyncRoot
        {
            get { return _lockObject; }
        }

        #endregion Properties

        #region Methods

        protected override void InsertItem(int index, T item)
        {
            lock (_lockObject)
            {
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (_lockObject)
            {
                base.RemoveItem(index);
            }
        }

        #endregion Methods
    }
}