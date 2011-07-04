using System;
using System.Windows.Threading;

using Torshify.Client.Infrastructure.Interfaces;

using ITorshifySearch = Torshify.Client.Infrastructure.Interfaces.ISearch;

namespace Torshify.Client.Spotify.Services
{
    public class SearchProvider : ISearchProvider
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ISession _session;
        private ISearch _search;

        #endregion Fields

        #region Constructors

        public SearchProvider(ISession session, Dispatcher dispatcher)
        {
            _session = session;
            _dispatcher = dispatcher;
        }

        #endregion Constructors

        #region Methods

        public ITorshifySearch Search(string query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, object userData = null)
        {
            var spotifySearch = _session.Search(
                query,
                trackOffset,
                trackCount,
                albumOffset,
                albumCount,
                artistOffset,
                artistCount,
                userData);

            return new Search(spotifySearch, _dispatcher);
        }

        public ITorshifySearch Search(int fromYear, int toYear, Genre genre, object userData = null)
        {
            var spotifySearch = _session.Search(
                fromYear, 
                toYear, 
                (RadioGenre) genre, 
                userData);

            return new Search(spotifySearch, _dispatcher);
        }

        #endregion Methods
    }
}