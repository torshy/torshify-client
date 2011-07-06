using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core.Views.Artist.Tabs;

namespace Torshify.Client.Modules.Core.Views.Artist
{
    public class ArtistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private ObservableCollection<ITab<IArtist>> _tabs;
        private ICollectionView _tabsIcv;

        #endregion Fields

        #region Constructors

        public ArtistViewModel()
        {
            _tabs = new ObservableCollection<ITab<IArtist>>();
            _tabs.Add(ServiceLocator.Current.TryResolve<OverviewTabItemView>());
            _tabs.Add(ServiceLocator.Current.TryResolve<BiographyTabItemView>());

            _tabsIcv = new ListCollectionView(_tabs);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Tabs
        {
            get
            {
                return _tabsIcv;
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
            foreach (var tabItem in _tabs)
            {
                tabItem.ViewModel.Deinitialize(navigationContext);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var artist = navigationContext.Tag as IArtist;

            foreach (var tabItem in _tabs)
            {
                tabItem.ViewModel.SetModel(artist);
                tabItem.ViewModel.Initialize(navigationContext);
            }

            Tabs.MoveCurrentToFirst();
        }

        #endregion Methods
    }
}