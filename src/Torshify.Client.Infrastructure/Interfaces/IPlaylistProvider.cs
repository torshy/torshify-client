using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylistProvider
    {
        IEnumerable<IPlaylist> Playlists { get; }
    }

    public interface IPlaylist
    {
        string Name { get; set; }
        string Description { get; set; }
        IEnumerable<IPlaylistTrack> Tracks { get; }
    }

    public interface ITrack
    {
        int ID { get; }
        int Index { get; }
        string Name { get; }
    }

    public interface IPlaylistTrack : ITrack
    {
        
    }
}