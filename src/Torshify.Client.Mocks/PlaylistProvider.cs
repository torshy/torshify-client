using System.Collections.Generic;

using FizzWare.NBuilder;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class PlaylistProvider : IPlaylistProvider, IInitializable
    {
        #region Fields

        private IList<Playlist> _playlists;

        #endregion Fields

        #region Constructors

        public PlaylistProvider()
        {
            _playlists = new List<Playlist>();
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
                .CreateListOfSize(15)
                .WhereAll()
                .Has(p=>p.Tracks = GetTracks())
                .Build();
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<IPlaylistTrack> GetTracks()
        {
            return Builder<PlaylistTrack>
                .CreateListOfSize(35)
                .Build();
        }

        #endregion Private Methods
    }
}