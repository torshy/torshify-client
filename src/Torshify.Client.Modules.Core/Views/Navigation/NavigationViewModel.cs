using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class NavigationViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly ObservableCollection<INavigationItem> _navigationItems;

        #endregion Fields

        #region Constructors

        public NavigationViewModel(IRegionManager regionManager, IPlaylistProvider playlistProvider)
        {
            _regionManager = regionManager;
            _navigationItems = new ObservableCollection<INavigationItem>();
            _navigationItems.Add(new PlayQueueNavigationItem(_regionManager));
            
            foreach (var playlist in playlistProvider.Playlists)
            {
                _navigationItems.Add(new PlaylistNavigationItem(playlist, _regionManager));
            }

            NavigationItems = CollectionViewSource.GetDefaultView(_navigationItems);
            NavigationItems.CurrentChanged += OnCurrentNavigationItemChanged;
        }

        #endregion Constructors

        #region Properties

        public ICollectionView NavigationItems
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Methods

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion Public Methods

        #region Private Methods

        private void OnCurrentNavigationItemChanged(object sender, EventArgs e)
        {
            INavigationItem item = NavigationItems.CurrentItem as INavigationItem;

            if (item != null)
            {
                item.NavigateTo();
            }
        }

        #endregion Private Methods
    }
}