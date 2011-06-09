using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class NavigationViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly ObservableCollection<INavigationItem> _navigationItems;

        private IRegion _mainRegion;

        #endregion Fields

        #region Constructors

        public NavigationViewModel(IRegionManager regionManager, IPlaylistProvider playlistProvider)
        {
            _regionManager = regionManager;
            _mainRegion = regionManager.Regions[CoreRegionNames.MainMusicRegion];
            _mainRegion.NavigationService.Navigated += (s, e) =>
                                                           {
                                                               var navEntry = e.NavigationContext.NavigationService.Journal.CurrentEntry;
                                                               var navItem = _navigationItems.FirstOrDefault(n => n.IsMe(navEntry));

                                                               if (navItem != null && NavigationItems.CurrentItem != navItem)
                                                               {
                                                                   NavigationItems.MoveCurrentTo(navItem);
                                                               }
                                                           };

            _navigationItems = new ObservableCollection<INavigationItem>();
            _navigationItems.Add(new PlayQueueNavigationItem(_regionManager));

            foreach (var playlist in playlistProvider.Playlists)
            {
                _navigationItems.Add(new PlaylistNavigationItem(playlist, _regionManager));
            }

            NavigationItems = CollectionViewSource.GetDefaultView(_navigationItems);
            NavigateToItemCommand = new AutomaticCommand<INavigationItem>(ExecuteNavigateToItem, CanExecuteNavigateToItem);
        }

        #endregion Constructors

        #region Properties

        public ICollectionView NavigationItems
        {
            get;
            private set;
        }

        public AutomaticCommand<INavigationItem> NavigateToItemCommand
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

        private void ExecuteNavigateToItem(INavigationItem item)
        {
            item.NavigateTo();
        }

        private bool CanExecuteNavigateToItem(INavigationItem item)
        {
            return true;
        }

        #endregion Private Methods
    }
}