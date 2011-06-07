using System;
using System.Collections.Generic;
using FizzWare.NBuilder;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class PlaylistProvider : IPlaylistProvider, IInitializable
    {
        private IList<Playlist> _playlists;

        public PlaylistProvider()
        {
            _playlists = new List<Playlist>(); 
    
        }

        public IEnumerable<IPlaylist> Playlists
        {
            get { return _playlists; }
        }

        public void Initialize()
        {
            _playlists = Builder<Playlist>
                .CreateListOfSize(15).Build();
        }
    }
}