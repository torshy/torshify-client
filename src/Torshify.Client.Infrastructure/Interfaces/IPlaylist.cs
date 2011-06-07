using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylist
    {
        string Name { get; set; }
        string Description { get; set; }
        IEnumerable<IPlaylistTrack> Tracks { get; }
    }
}