using System;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class PlaylistNavigationItem : NavigationItem
    {
        #region Fields

        private readonly Infrastructure.Interfaces.IPlaylist _containerPlaylist;

        #endregion Fields

        #region Constructors

        public PlaylistNavigationItem(Infrastructure.Interfaces.IPlaylist playlist, Uri navigationUri) : base(navigationUri)
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

        #endregion Properties
    }
}