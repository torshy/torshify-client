using System;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Collections;
using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyAlbumInformation = Torshify.Client.Infrastructure.Interfaces.IAlbumInformation;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class AlbumInformation : NotificationObject, ITorshifyAlbumInformation
    {
        #region Fields

        private readonly Album _album;
        private readonly NotifyCollection<string> _copyrights;
        private readonly Dispatcher _dispatcher;
        private readonly NotifyCollection<Track> _tracks;

        private IAlbumBrowse _browse;
        private bool _isLoading;
        private Artist _orginalArtist;
        private string _review;

        #endregion Fields

        #region Constructors

        public AlbumInformation(Artist originalArtist, Album album, Dispatcher dispatcher)
        {
            _orginalArtist = originalArtist;
            _tracks = new NotifyCollection<Track>();
            _copyrights = new NotifyCollection<string>();
            _dispatcher = dispatcher;
            _album = album;
            _browse = _album.InternalAlbum.Browse();
            _isLoading = !_browse.IsComplete;

            if (IsLoading)
            {
                _browse.Completed += AlbumBrowseCompleted;
            }
            else
            {
                AlbumBrowseCompleted(_browse, null);
            }
        }

        #endregion Constructors

        #region Events

        public event EventHandler<AlbumInformationEventArgs> Loaded;

        #endregion Events

        #region Properties

        public INotifyEnumerable<string> Copyrights
        {
            get
            {
                return _copyrights;
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

        public string Review
        {
            get
            {
                return _review;
            }
            private set
            {
                if (_review != value)
                {
                    _review = value;
                    RaisePropertyChanged("Review");
                }
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

        private void AlbumBrowseCompleted(object sender, UserDataEventArgs e)
        {
            var browse = (IAlbumBrowse)sender;
            browse.Completed -= AlbumBrowseCompleted;

            Review = browse.Review;
            _dispatcher.BeginInvoke(new Action<IAlbumBrowse>(LoadDataFromBrowse), DispatcherPriority.Background, browse);
        }

        private void LoadDataFromBrowse(IAlbumBrowse browse)
        {
            using (browse)
            {
                foreach (var copyright in browse.Copyrights)
                {
                    _copyrights.Add(copyright);
                }

                foreach (var spotifyTrack in browse.Tracks)
                {
                    if (_orginalArtist != null && _orginalArtist.Name != spotifyTrack.Album.Artist.Name)
                    {
                        _album.ChangeAlbumType(Infrastructure.Interfaces.AlbumType.Compilation);
                    }

                    _tracks.Add(new Track(spotifyTrack, _dispatcher));
                }
            }

            IsLoading = false;

            OnLoaded();
        }

        private void OnLoaded()
        {
            var handler = Loaded;
            
            if (handler != null)
            {
                handler(this, new AlbumInformationEventArgs(_album));
            }
        }

        #endregion Methods
    }
}