using System.ComponentModel;
using System.Windows.Data;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Playlists
{
    public class PlaylistsViewModel : NotificationObject, INavigationAware
    {
        private readonly IPlaylistProvider _playlistProvider;
        private readonly IRegionManager _regionManager;

        public PlaylistsViewModel(
            IPlaylistProvider playlistProvider,
            IRegionManager regionManager)
        {
            _playlistProvider = playlistProvider;
            _regionManager = regionManager;

            Playlists = CollectionViewSource.GetDefaultView(_playlistProvider.Playlists);
        }

        public ICollectionView Playlists
        {
            get; 
            private set;
        }

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
    }
}