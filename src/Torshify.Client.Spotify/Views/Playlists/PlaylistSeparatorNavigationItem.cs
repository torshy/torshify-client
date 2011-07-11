using System;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class PlaylistSeparatorNavigationItem : PlaylistNavigationItem
    {
        #region Constructors

        public PlaylistSeparatorNavigationItem(Infrastructure.Interfaces.IPlaylist playlist, Uri navigationUri)
            : base(playlist, navigationUri)
        {
        }

        #endregion Constructors
    }
}