using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Collections;
using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyPlaylist = Torshify.Client.Infrastructure.Interfaces.IPlaylist;

using ITorshifyPlaylistTrack = Torshify.Client.Infrastructure.Interfaces.IPlaylistTrack;

namespace Torshify.Client.Spotify.Services
{
    public class Playlist : NotificationObject, ITorshifyPlaylist
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly NotifyCollection<PlaylistTrack> _tracks;

        private bool _isLoaded;
        private bool _isUpdating;
        private object _lockObject = new object();

        #endregion Fields

        #region Constructors

        public Playlist(IPlaylist playlist, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _tracks = new NotifyCollection<PlaylistTrack>();

            InternalPlaylist = playlist;
            InternalPlaylist.DescriptionChanged += OnDescriptionChanged;
            InternalPlaylist.MetadataUpdated += OnMetadataChanged;
            InternalPlaylist.Renamed += OnRenamed;
            InternalPlaylist.StateChanged += OnStateChanged;
            InternalPlaylist.TracksAdded += OnTracksAdded;
            InternalPlaylist.TracksRemoved += OnTracksRemoved;
            InternalPlaylist.TracksMoved += OnTracksMoved;
            InternalPlaylist.UpdateInProgress += OnUpdateInProgress;

            _isLoaded = InternalPlaylist.IsLoaded;

            if (_isLoaded)
            {
                FetchTracks();
            }
        }

        #endregion Constructors

        #region Properties

        public string Description
        {
            get
            {
                return InternalPlaylist.Description;
            }
        }

        public IPlaylist InternalPlaylist
        {
            get;
            private set;
        }

        public bool IsCollaborative
        {
            get
            {
                return InternalPlaylist.IsCollaborative;
            }
            set
            {
                InternalPlaylist.IsCollaborative = value;
            }
        }

        public bool IsUpdating
        {
            get
            {
                return _isUpdating;
            }
            private set
            {
                if (_isUpdating != value)
                {
                    _isUpdating = value;
                    RaisePropertyChanged("IsUpdating");
                }
            }
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

        public INotifyEnumerable<ITorshifyPlaylistTrack> Tracks
        {
            get
            {
                return _tracks;
            }
        }

        #endregion Properties

        #region Methods

        public void MoveTrack(int oldIndex, int newIndex)
        {
            if (InternalPlaylist.IsValid())
            {
                InternalPlaylist.Tracks.Move(oldIndex, newIndex);
            }
        }

        public void MoveTracks(int[] indices, int newIndex)
        {
            if (InternalPlaylist.IsValid())
            {
                InternalPlaylist.Tracks.Move(indices, newIndex);
            }
        }

        public void RemoveTrack(ITorshifyPlaylistTrack track)
        {
            PlaylistTrack myTrack = track as PlaylistTrack;
            if (InternalPlaylist.IsValid() && myTrack != null)
            {
                InternalPlaylist.Tracks.Remove((IPlaylistTrack)myTrack.InternalTrack);
            }
        }

        private void Add(IPlaylistTrack track)
        {
            if (_dispatcher.CheckAccess())
            {
                lock (_lockObject)
                {
                    _tracks.Add(new PlaylistTrack(this, track, _dispatcher));
                }
            }
            else
            {
                _dispatcher.BeginInvoke((Action<IPlaylistTrack>)Add, track);
            }
        }

        private void FetchTracks()
        {
            for (int i = 0; i < InternalPlaylist.Tracks.Count; i++)
            {
                var track = InternalPlaylist.Tracks[i];
                Add(track);
            }
        }

        private void InsertAt(int index, IPlaylistTrack track)
        {
            if (_dispatcher.CheckAccess())
            {
                lock (_lockObject)
                {
                    _tracks.Insert(index, new PlaylistTrack(this, track, _dispatcher));
                }
            }
            else
            {
                _dispatcher.BeginInvoke((Action<int, IPlaylistTrack>)InsertAt, index, track);
            }
        }

        private void Move(int oldIndex, int newIndex)
        {
            if (_dispatcher.CheckAccess())
            {
                lock (_lockObject)
                {
                    _tracks.Move(oldIndex, newIndex);
                }
            }
            else
            {
                _dispatcher.BeginInvoke((Action<int, int>)Move, oldIndex, newIndex);
            }
        }

        private void OnDescriptionChanged(object sender, DescriptionEventArgs e)
        {
            RaisePropertyChanged("Description");
        }

        private void OnMetadataChanged(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                foreach (var playlistTrack in _tracks)
                {
                    playlistTrack.Refresh();
                }
            }
        }

        private void OnRenamed(object sender, EventArgs e)
        {
            RaisePropertyChanged("Name");
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (!_isLoaded && InternalPlaylist.IsLoaded)
            {
                _isLoaded = true;
                FetchTracks();
            }

            RaisePropertyChanged("IsCollaborative");
        }

        private void OnTracksAdded(object sender, TracksAddedEventArgs e)
        {
            for (int i = 0; i < e.Tracks.Length; i++)
            {
                var track = e.Tracks[i] as IPlaylistTrack;
                int trackIndex = e.TrackIndices[i];

                if (track != null)
                {
                    InsertAt(trackIndex, track);
                }
            }
        }

        private void OnTracksMoved(object sender, TracksMovedEventArgs e)
        {
            for (int i = 0; i < e.TrackIndices.Length; i++)
            {
                Move(e.TrackIndices[i], e.NewPosition - 1);
            }
        }

        private void OnTracksRemoved(object sender, TracksRemovedEventArgs e)
        {
            for (int i = 0; i < e.TrackIndices.Length; i++)
            {
                RemoveAt(e.TrackIndices[i]);
            }
        }

        private void OnUpdateInProgress(object sender, PlaylistUpdateEventArgs e)
        {
            IsUpdating = !e.IsDone;
        }

        private void RemoveAt(int index)
        {
            if (_dispatcher.CheckAccess())
            {
                if (index >= 0 && index < _tracks.Count)
                {
                    lock (_lockObject)
                    {
                        _tracks.RemoveAt(index);
                    }
                }
            }
            else
            {
                _dispatcher.BeginInvoke((Action<int>)RemoveAt, index);
            }
        }

        #endregion Methods
    }
}