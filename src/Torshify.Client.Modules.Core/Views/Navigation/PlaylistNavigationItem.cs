using System;
using Microsoft.Practices.Prism.Regions;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class PlaylistNavigationItem : INavigationItem
    {
        private readonly IPlaylist _playlist;
        private readonly IRegionManager _regionManager;

        public PlaylistNavigationItem(IPlaylist playlist, IRegionManager regionManager)
        {
            _playlist = playlist;
            _regionManager = regionManager;
        }

        public IPlaylist Playlist
        {
            get { return _playlist; }
        }

        public void NavigateTo()
        {
            var uri = new Uri(MusicRegionViewNames.PlaylistView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri, _playlist);
        }
    }
}