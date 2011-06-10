using System;
using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylistProvider
    {
        #region Events

        event EventHandler<PlaylistEventArgs> PlaylistAdded;

        event EventHandler<PlaylistEventArgs> PlaylistRemoved;

        #endregion Events

        #region Properties

        IEnumerable<IPlaylist> Playlists
        {
            get;
        }

        #endregion Properties
    }

    public class PlaylistEventArgs : EventArgs
    {
        #region Constructors

        public PlaylistEventArgs(IPlaylist playlist)
        {
            Playlist = playlist;
        }

        #endregion Constructors

        #region Properties

        public IPlaylist Playlist
        {
            get; private set;
        }

        #endregion Properties
    }
}