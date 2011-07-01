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
        private IAlbumBrowse _browse;

        #endregion Fields

        #region Constructors

        public AlbumInformation(IAlbum album, Dispatcher dispatcher)
        {
            _tracks = new ObservableCollection<Track>();
            _copyrights = new ObservableCollection<string>();
            _dispatcher = dispatcher;
            _album = album;
            _browse = _album.Browse();
            _isLoading = !_browse.IsComplete;
            _browse.Completed += AlbumBrowseCompleted;
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

            Review = browse.Review;

            _dispatcher.BeginInvoke(new Action<IAlbumBrowse>(LoadDataFromBrowse), DispatcherPriority.Background, browse);
            browse.Completed -= AlbumBrowseCompleted;
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
                    _tracks.Add(new Track(spotifyTrack, _dispatcher));
                }
            }

            IsLoading = false;
        }

        #endregion Methods
    }
}