using System.Windows.Threading;
using ITorshifyPlaylist = Torshify.Client.Infrastructure.Interfaces.IPlaylist;

using ITorshifyPlaylistTrack = Torshify.Client.Infrastructure.Interfaces.IPlaylistTrack;

namespace Torshify.Client.Spotify.Services
{
    public class PlaylistTrack : Track, ITorshifyPlaylistTrack
    {
        #region Constructors

        public PlaylistTrack(ITorshifyPlaylist parentPlaylist, IPlaylistTrack track, Dispatcher dispatcher)
            : base(track, dispatcher)
        {
            Playlist = parentPlaylist;
        }

        #endregion Constructors

        #region Properties

        public ITorshifyPlaylist Playlist
        {
            get;
            private set;
        }

        #endregion Properties
    }
}