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
    }
}