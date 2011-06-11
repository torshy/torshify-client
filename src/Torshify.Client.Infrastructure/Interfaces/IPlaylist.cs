using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylist
    {
        string Name { get; set; }
        string Description { get; }
        IEnumerable<IPlaylistTrack> Tracks { get; }
    }
}