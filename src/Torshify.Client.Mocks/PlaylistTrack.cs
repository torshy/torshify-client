using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class PlaylistTrack : Track, IPlaylistTrack
    {
        public IPlaylist Playlist
        {
            get; 
            set;
        }
    }
}