using System;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class UnknownPlaylistNavigationItem : PlaylistNavigationItem
    {
        #region Constructors

        public UnknownPlaylistNavigationItem(Infrastructure.Interfaces.IPlaylist playlist, Uri navigationUri)
            : base(playlist, navigationUri)
        {
        }

        #endregion Constructors
    }
}