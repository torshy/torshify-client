using System;
using Microsoft.Practices.Prism.Regions;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class PlaylistNavigationItem : INavigationItem
    {
        private readonly IPlaylist _playlist;
        private readonly IRegionManager _regionManager;
        private Uri _uri;

        public PlaylistNavigationItem(IPlaylist playlist, IRegionManager regionManager)
        {
            _playlist = playlist;
            _regionManager = regionManager;
            _uri = new Uri(MusicRegionViewNames.PlaylistView, UriKind.Relative);
        }

        public IPlaylist Playlist
        {
            get { return _playlist; }
        }

        public void NavigateTo()
        {
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, _uri, _playlist);
        }

        public bool IsMe(IRegionNavigationJournalEntry entry)
        {
            var parts = entry.Uri.OriginalString.Split('?');

            if (parts[0] == _uri.OriginalString && entry.Tag == Playlist)
            {
                return true;
            }

            return false;
        }
    }
}