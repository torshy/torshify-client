using Microsoft.Practices.Prism.Regions;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface INavigationItem
    {
        void NavigateTo();

        bool IsMe(IRegionNavigationJournalEntry entry);
    }
}