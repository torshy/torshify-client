using System;
using System.Collections.Generic;

using FizzWare.NBuilder;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class PlaylistProvider : IPlaylistProvider, IInitializable
    {
        #region Fields

        private IList<Playlist> _playlists;
        private Random _random;

        #endregion Fields

        #region Constructors

        public PlaylistProvider()
        {
            _playlists = new List<Playlist>();
            _random = new Random();
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<IPlaylist> Playlists
        {
            get { return _playlists; }
        }

        #endregion Properties

        #region Public Methods

        public void Initialize()
        {
            _playlists = Builder<Playlist>
                .CreateListOfSize(20)
                .WhereAll()
                .Has(p=>p.Tracks = GetTracks())
                .Build();
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<IPlaylistTrack> GetTracks()
        {
            return Builder<PlaylistTrack>
                .CreateListOfSize(_random.Next(6,35))
                .Build();
        }

        #endregion Private Methods
    }
}