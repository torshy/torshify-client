using System.Collections.Generic;
using System.Collections.Specialized;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface INotifyEnumerable<out T> : IEnumerable<T>, INotifyCollectionChanged
    {
        object SyncRoot { get; }

        int Count { get; }

        T this[int index] { get; }
    }
}