using System;
using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylistProvider
    {
        #region Events

        event EventHandler<PlaylistEventArgs> PlaylistAdded;

        event EventHandler<PlaylistEventArgs> PlaylistRemoved;

        event EventHandler<PlaylistMovedEventArgs> PlaylistMoved;

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

        public PlaylistEventArgs(IPlaylist playlist, int position)
        {
            Playlist = playlist;
            Position = position;
        }

        #endregion Constructors

        #region Properties

        public IPlaylist Playlist
        {
            get; private set;
        }

        public int Position
        {
            get; private set;
        }

        #endregion Properties
    }

    public class PlaylistMovedEventArgs : EventArgs
    {
        #region Constructors

        public PlaylistMovedEventArgs(IPlaylist playlist, int oldIndex, int newIndex)
        {
            Playlist = playlist;
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        #endregion Constructors

        #region Properties

        public IPlaylist Playlist
        {
            get;
            private set;
        }

        public int OldIndex
        {
            get; private set;
        }

        public int NewIndex
        {
            get; private set;
        }

        #endregion Properties
    }
}