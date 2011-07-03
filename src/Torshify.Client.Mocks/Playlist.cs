using System.Collections.Generic;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Playlist : IPlaylist
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCollaborative { get; set; }
        public bool IsUpdating { get; set; }
        public IEnumerable<IPlaylistTrack> Tracks { get; set; }
    }
}