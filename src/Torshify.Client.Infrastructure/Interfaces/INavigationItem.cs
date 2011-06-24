using System.Windows;

using Microsoft.Practices.Prism.Regions;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface INavigationItem
    {
        #region Properties

        DataTemplate DataTemplate
        {
            get;
        }

        #endregion Properties

        #region Methods

        bool IsMe(IRegionNavigationJournalEntry entry);

        void NavigateTo();

        #endregion Methods
    }
}