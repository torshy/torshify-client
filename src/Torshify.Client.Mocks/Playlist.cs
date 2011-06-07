using System.Collections.Generic;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Playlist : IPlaylist
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<IPlaylistTrack> Tracks { get; private set; }
    }
}