namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylistTrack : ITrack
    {
        IPlaylist Playlist { get; }
    }
}