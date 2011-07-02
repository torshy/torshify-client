using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Search
{
    public class SearchViewModel : NotificationObject, INavigationAware
    {
        private readonly ISearchProvider _searchProvider;

        public SearchViewModel(ISearchProvider searchProvider)
        {
            _searchProvider = searchProvider;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string query = navigationContext.Parameters["Query"];
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}