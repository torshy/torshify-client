using System;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Collections;
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

        private NotifyCollection<ITorshifyAlbum> _albums;
        private NotifyCollection<ITorshifyArtist> _artists;
        private NotifyCollection<ITorshifyTrack> _tracks;
        private string _didYouMean;
        private bool _isLoading;
        private string _query;
        private int _totalAlbums;
        private int _totalArtists;
        private int _totalTracks;

        #endregion Fields

        #region Constructors

        public Search(ISearch search, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _albums = new NotifyCollection<ITorshifyAlbum>();
            _artists = new NotifyCollection<ITorshifyArtist>();
            _tracks = new NotifyCollection<ITorshifyTrack>();

            InternalSearch = search;

            if (!InternalSearch.IsComplete)
            {
                InternalSearch.Completed += OnSearchCompleted;
            }
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
            private set
            {
                if (_didYouMean != value)
                {
                    _didYouMean = value;
                    RaisePropertyChanged("DidYouMean");
                }
            }
        }

        public ISearch InternalSearch
        {
            get; private set;
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            private set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }

        public string Query
        {
            get { return _query; }
            private set
            {
                if (_query != value)
                {
                    _query = value;
                    RaisePropertyChanged("Query");
                }
            }
        }

        public int TotalAlbums
        {
            get { return _totalAlbums; }
            private set
            {
                if (_totalAlbums != value)
                {
                    _totalAlbums = value;
                    RaisePropertyChanged("TotalAlbums");
                }
            }
        }

        public int TotalArtists
        {
            get { return _totalArtists; }
            private set
            {
                if (_totalArtists != value)
                {
                    _totalArtists = value;
                    RaisePropertyChanged("TotalArtists");
                }
            }
        }

        public int TotalTracks
        {
            get { return _totalTracks; }
            private set
            {
                if (_totalTracks != value)
                {
                    _totalTracks = value;
                    RaisePropertyChanged("TotalTracks");
                }
            }
        }

        public INotifyEnumerable<ITorshifyTrack> Tracks
        {
            get { return _tracks; }
        }

        #endregion Properties

        #region Methods

        private void OnSearchCompleted(object sender, SearchEventArgs e)
        {
            ISearch search = (ISearch)sender;
            search.Completed -= OnSearchCompleted;
            _dispatcher.BeginInvoke(new Action<ISearch>(LoadSearchData), DispatcherPriority.Background, search);
        }

        private void LoadSearchData(ISearch search)
        {
            using (search)
            {
                TotalAlbums = search.TotalAlbums;
                TotalTracks = search.TotalTracks;
                TotalArtists = search.TotalArtists;
                Query = search.Query;
                DidYouMean = search.DidYouMean;

                foreach (var spotifyAlbum in search.Albums)
                {
                    _albums.Add(new Album(spotifyAlbum, _dispatcher));
                }

                foreach (var spotifyTrack in search.Tracks)
                {
                    _tracks.Add(new Track(spotifyTrack, _dispatcher));
                }

                foreach (var spotifyArtist in search.Artists)
                {
                    _artists.Add(new Artist(spotifyArtist, _dispatcher));
                }
            }

            IsLoading = false;
            RaiseFinishedLoading();
        }

        private void RaiseFinishedLoading()
        {
            var handler = FinishedLoading;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}