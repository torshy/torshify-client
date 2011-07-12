using System.Collections.Generic;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class FolderPlaylistNavigationItem : PlaylistNavigationItem
    {
        #region Constructors

        public FolderPlaylistNavigationItem(Infrastructure.Interfaces.IPlaylist playlist)
            : base(playlist)
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

    public class EndFolderPlaylistNavigationItem : PlaylistNavigationItem
    {
        public EndFolderPlaylistNavigationItem(Infrastructure.Interfaces.IPlaylist playlist)
            : base(playlist)
        {

        }
    }
}