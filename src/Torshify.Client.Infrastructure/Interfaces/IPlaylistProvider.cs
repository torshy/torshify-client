using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylistProvider
    {
        IEnumerable<IPlaylist> Playlists { get; }
    }
}