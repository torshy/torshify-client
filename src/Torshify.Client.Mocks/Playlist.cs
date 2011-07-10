using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Playlist : IPlaylist
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCollaborative { get; set; }
        public bool IsUpdating { get; set; }

        public void MoveTrack(int oldIndex, int newIndex)
        {
            
        }

        public void MoveTracks(int[] indices, int newIndex)
        {

        }

        public INotifyEnumerable<IPlaylistTrack> Tracks { get; set; }
    }
}