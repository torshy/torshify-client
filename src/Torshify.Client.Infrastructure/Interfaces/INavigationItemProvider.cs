using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface INavigationItemProvider
    {
        IEnumerable<INavigationItem> Items { get; }
    }
}