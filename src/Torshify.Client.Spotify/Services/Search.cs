using System;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

using ITorshifyImage = Torshify.Client.Infrastructure.Interfaces.IImage;

using ITorshifySearch = Torshify.Client.Infrastructure.Interfaces.ISearch;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class Search : NotificationObject, ITorshifySearch
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private INotifyEnumerable<ITorshifyAlbum> _albums;
        private INotifyEnumerable<ITorshifyArtist> _artists;
        private string _didYouMean;
        private bool _isLoading;
        private string _query;
        private int _totalAlbums;
        private int _totalArtists;
        private int _totalTracks;
        private INotifyEnumerable<ITorshifyTrack> _tracks;

        #endregion Fields

        #region Constructors

        public Search(ISearch search, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            InternalSearch = search;
        }

        #endregion Constructors

        #region Events

        public event EventHandler FinishedLoading;

        #endregion Events

        #region Properties

        public INotifyEnumerable<ITorshifyAlbum> Albums
        {
            get { return _albums; }
        }

        public INotifyEnumerable<ITorshifyArtist> Artists
        {
            get { return _artists; }
        }

        public string DidYouMean
        {
            get { return _didYouMean; }
        }

        public ISearch InternalSearch
        {
            get; private set;
        }

        public bool IsLoading
        {
            get { return _isLoading; }
        }

        public string Query
        {
            get { return _query; }
        }

        public int TotalAlbums
        {
            get { return _totalAlbums; }
        }

        public int TotalArtists
        {
            get { return _totalArtists; }
        }

        public int TotalTracks
        {
            get { return _totalTracks; }
        }

        public INotifyEnumerable<ITorshifyTrack> Tracks
        {
            get { return _tracks; }
        }

        #endregion Properties
    }
}