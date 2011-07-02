using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Search
{
    public class SearchViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ISearchProvider _searchProvider;

        private ISearch _currentSearch;
        private ObservableCollection<ITrack> _searchResults;
        private ICollectionView _searchResultsIcv;

        #endregion Fields

        #region Constructors

        public SearchViewModel(ISearchProvider searchProvider, Dispatcher dispatcher)
        {
            _searchProvider = searchProvider;
            _dispatcher = dispatcher;
            _searchResults = new ObservableCollection<ITrack>();
            _searchResultsIcv = new ListCollectionView(_searchResults);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView SearchResults
        {
            get
            {
                return _searchResultsIcv;
            }
        }

        #endregion Properties

        #region Methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _currentSearch.FinishedLoading -= SearchFinishedLoading;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string query = navigationContext.Parameters["Query"];
            _currentSearch = _searchProvider.Search(query, 0, 10, 0, 10, 0, 10);
            _currentSearch.FinishedLoading += SearchFinishedLoading;
        }

        private void SearchFinishedLoading(object sender, EventArgs e)
        {
            ISearch search = (ISearch) sender;
            search.FinishedLoading -= SearchFinishedLoading;

            _searchResults.Clear();
            foreach (var track in search.Tracks)
            {
                _searchResults.Add(track);
            }
        }

        #endregion Methods
    }
}