using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyAlbumInformation = Torshify.Client.Infrastructure.Interfaces.IAlbumInformation;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class AlbumInformation : NotificationObject, ITorshifyAlbumInformation
    {
        #region Fields

        private readonly IAlbum _album;
        private readonly ObservableCollection<string> _copyrights;
        private readonly Dispatcher _dispatcher;
        private readonly ObservableCollection<Track> _tracks;
        
        private bool _isLoading;
        private string _review;

        #endregion Fields

        #region Constructors

        public AlbumInformation(IAlbum album, Dispatcher dispatcher)
        {
            _tracks = new ObservableCollection<Track>();
            _copyrights = new ObservableCollection<string>();
            _dispatcher = dispatcher;
            _album = album;
            var albumBrowse = _album.Browse();
            _isLoading = !albumBrowse.IsComplete;
            albumBrowse.Completed += AlbumBrowseCompleted;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<string> Copyrights
        {
            get
            {
                return _copyrights;
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

        public IEnumerable<ITorshifyTrack> Tracks
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

            using (browse)
            {
                Review = browse.Review;

                foreach (var copyright in browse.Copyrights)
                {
                    _dispatcher.BeginInvoke((Action<string>) _copyrights.Add, DispatcherPriority.Background, copyright);
                }

                foreach (var spotifyTrack in browse.Tracks)
                {
                    Track track = new Track(spotifyTrack, _dispatcher);
                    _dispatcher.BeginInvoke((Action<Track>)_tracks.Add, DispatcherPriority.Background, track);
                }
            }

            browse.Completed -= AlbumBrowseCompleted;

            IsLoading = false;
        }

        #endregion Methods
    }
}