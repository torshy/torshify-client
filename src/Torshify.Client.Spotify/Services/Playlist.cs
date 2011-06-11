using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyPlaylist = Torshify.Client.Infrastructure.Interfaces.IPlaylist;

using ITorshifyPlaylistTrack = Torshify.Client.Infrastructure.Interfaces.IPlaylistTrack;

namespace Torshify.Client.Spotify.Services
{
    public class Playlist : NotificationObject, ITorshifyPlaylist
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private ObservableCollection<PlaylistTrack> _tracks;
        private bool _isLoaded;
        #endregion Fields

        #region Constructors

        public Playlist(IPlaylist playlist, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _tracks = new ObservableCollection<PlaylistTrack>();

            InternalPlaylist = playlist;
            InternalPlaylist.DescriptionChanged += OnDescriptionChanged;
            InternalPlaylist.MetadataUpdated += OnMetadataChanged;
            InternalPlaylist.Renamed += OnRenamed;
            InternalPlaylist.StateChanged += OnStateChanged;
            InternalPlaylist.TracksAdded += OnTracksAdded;
            InternalPlaylist.TracksRemoved += OnTracksRemoved;

            _isLoaded = InternalPlaylist.IsLoaded;

            if (_isLoaded)
            {
                FetchTracks();
            }
        }

        #endregion Constructors

        #region Properties

        public IPlaylist InternalPlaylist
        {
            get;
            private set;
        }

        public string Name
        {
            get
            {
                return InternalPlaylist.Name;
            }
            set
            {
                InternalPlaylist.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return InternalPlaylist.Description;
            }
        }

        public IEnumerable<ITorshifyPlaylistTrack> Tracks
        {
            get
            {
                return _tracks;
            }
        }

        #endregion Properties

        #region Private Methods

        private void FetchTracks()
        {
            for (int i = 0; i < InternalPlaylist.Tracks.Count; i++)
            {
                var track = InternalPlaylist.Tracks[i];
                Add(track);
            }
        }

        private void OnTracksRemoved(object sender, TracksRemovedEventArgs e)
        {
        }

        private void OnTracksAdded(object sender, TracksAddedEventArgs e)
        {
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (!_isLoaded && InternalPlaylist.IsLoaded)
            {
                _isLoaded = true;
                FetchTracks();
            }
        }

        private void OnRenamed(object sender, EventArgs e)
        {
            RaisePropertyChanged("Name");
        }

        private void OnMetadataChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Name", "Description");
        }

        private void OnDescriptionChanged(object sender, DescriptionEventArgs e)
        {
            RaisePropertyChanged("Description");
        }
        
        private void Add(IPlaylistTrack track)
        {
            if (_dispatcher.CheckAccess())
            {
                _tracks.Add(new PlaylistTrack(track));
            }
            else
            {
                _dispatcher.BeginInvoke((Action<IPlaylistTrack>) Add, track);
            }
        }

        #endregion Private Methods
    }
}