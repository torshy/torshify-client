using System;
using ITorshifyPlaylistTrack = Torshify.Client.Infrastructure.Interfaces.IPlaylistTrack;

namespace Torshify.Client.Spotify.Services
{
    public class PlaylistTrack : Track, ITorshifyPlaylistTrack
    {
        public PlaylistTrack(IPlaylistTrack track)
            : base(track)
        {
            
        }
    }
}