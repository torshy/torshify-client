using System;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Collections;
using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class ArtistInformation : NotificationObject, IArtistInformation
    {
        #region Fields

        private readonly IArtist _artist;
        private readonly Dispatcher _dispatcher;

        private NotifyCollection<Album> _albums;
        private string _biography;
        private IArtistBrowse _browse;
        private bool _isLoading;
        private NotifyCollection<BitmapSource> _portraits;
        private NotifyCollection<Artist> _similarArtists;
        private NotifyCollection<Track> _tracks;

        #endregion Fields

        #region Constructors

        public ArtistInformation(IArtist artist, Dispatcher dispatcher)
        {
            _portraits = new NotifyCollection<BitmapSource>();
            _tracks = new NotifyCollection<Track>();
            _albums = new NotifyCollection<Album>();
            _similarArtists = new NotifyCollection<Artist>();
            _dispatcher = dispatcher;
            _artist = artist;
            _browse = _artist.Browse();
            _isLoading = !_browse.IsComplete;
            _browse.Completed += ArtistBrowseCompleted;
        }

        #endregion Constructors

        #region Events

        public event EventHandler FinishedLoading;

        #endregion Events

        #region Properties

        public INotifyEnumerable<ITorshifyAlbum> Albums
        {
            get
            {
                return _albums;
            }
        }

        public string Biography
        {
            get
            {
                return _biography;
            }
            private set
            {
                if (_biography != value)
                {
                    _biography = value;
                    RaisePropertyChanged("Biography");
                }
            }
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            private set
            {
                _isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }

        public INotifyEnumerable<BitmapSource> Portraits
        {
            get
            {
                return _portraits;
            }
        }

        public INotifyEnumerable<ITorshifyArtist> SimilarArtists
        {
            get
            {
                return _similarArtists;
            }
        }

        public INotifyEnumerable<ITorshifyTrack> Tracks
        {
            get
            {
                return _tracks;
            }
        }

        #endregion Properties

        #region Methods

        private void ArtistBrowseCompleted(object sender, EventArgs e)
        {
            IArtistBrowse browse = (IArtistBrowse)sender;
            browse.Completed -= ArtistBrowseCompleted;
            Biography = browse.Biography;
            _dispatcher.BeginInvoke(new Action<IArtistBrowse>(LoadBrowseData), DispatcherPriority.Background, browse);
        }

        private void LoadBrowseData(IArtistBrowse browse)
        {
            using (browse)
            {
                foreach (var spotifyAlbum in browse.Albums)
                {
                    _albums.Add(new Album(spotifyAlbum, _dispatcher));
                }

                foreach (var spotifyTrack in browse.Tracks)
                {
                    _tracks.Add(new Track(spotifyTrack, _dispatcher));
                }

                foreach (var spotifyArtist in browse.SimilarArtists)
                {
                    _similarArtists.Add(new Artist(spotifyArtist, _dispatcher));
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