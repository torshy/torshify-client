using System;
using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class PlaylistNavigationItem : NavigationItem
    {
        #region Fields

        private readonly Infrastructure.Interfaces.IPlaylist _containerPlaylist;
        private bool _hasTrackPlaying;

        #endregion Fields

        #region Constructors

        public PlaylistNavigationItem(Infrastructure.Interfaces.IPlaylist playlist)
            : base(new Uri(MusicRegionViewNames.PlaylistView, UriKind.Relative))
        {
            _containerPlaylist = playlist;
        }

        #endregion Constructors

        #region Properties

        public Infrastructure.Interfaces.IPlaylist Playlist
        {
            get
            {
                return _containerPlaylist;
            }
        }

        public bool HasTrackPlaying
        {
            get { return _hasTrackPlaying; }
            set
            {
                if (_hasTrackPlaying != value)
                {
                    _hasTrackPlaying = value;
                    RaisePropertyChanged("HasTrackPlaying");
                }
            }
        }

        #endregion Properties
    }
}