using System;
using System.Collections.Generic;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class FolderPlaylistNavigationItem : PlaylistNavigationItem
    {
        #region Constructors

        public FolderPlaylistNavigationItem(Infrastructure.Interfaces.IPlaylist playlist, Uri navigationUri)
            : base(playlist, navigationUri)
        {

        }

        #endregion Constructors

        #region Properties

        public IEnumerable<PlaylistNavigationItem> Children
        {
            get;
            set;
        }

        #endregion Properties
    }
}