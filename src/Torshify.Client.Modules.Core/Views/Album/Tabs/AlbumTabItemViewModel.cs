using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Album.Tabs
{
    public class AlbumTabItemViewModel : NotificationObject, ITabViewModel<IAlbum>
    {
        #region Fields

        private IAlbum _album;
        private IEventAggregator _eventAggregator;
        private SubscriptionToken _trackMenuBarToken;
        private SubscriptionToken _tracksMenuBarToken;

        #endregion Fields

        #region Constructors

        public AlbumTabItemViewModel(IEventAggregator eventAggregator, IPlayerController player)
        {
            _eventAggregator = eventAggregator;

            Player = player;
            PlayAlbumTrackCommand = new StaticCommand<ITrack>(ExecutePlayAlbumTrack);
        }

        #endregion Constructors

        #region Properties

        public IAlbum Album
        {
            get
            {
                return _album;
            }
            set
            {
                if (_album != value)
                {
                    _album = value;
                    RaisePropertyChanged("Album");
                }
            }
        }

        public string Header
        {
            get { return _album.Artist.Name; }
        }

        public ICommand PlayAlbumTrackCommand
        {
            get;
            private set;
        }

        public IPlayerController Player
        {
            get;
            private set;
        }

        public Visibility Visibility
        {
            get { return Visibility.Visible; }
        }

        #endregion Properties

        #region Methods

        public void Deinitialize(NavigationContext navContext)
        {
            _eventAggregator.GetEvent<TrackCommandBarEvent>().Unsubscribe(_trackMenuBarToken);
            _eventAggregator.GetEvent<TracksCommandBarEvent>().Unsubscribe(_tracksMenuBarToken);
        }

        public void Initialize(NavigationContext navContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);
        }

        public void SetModel(IAlbum model)
        {
            Album = model;
            RaisePropertyChanged("Header");
        }

        public void NavigatedTo()
        {
        }

        public void NavigatedFrom()
        {
        }

        private void ExecutePlayAlbumTrack(ITrack track)
        {
            // Get the rest of the tracks from the album, including the one selected.
            IEnumerable<ITrack> tracks = GetTracksToPlay(track);
            CoreCommands.PlayTrackCommand.Execute(tracks);
        }

        private IEnumerable<ITrack> GetTracksToPlay(ITrack track)
        {
            var tracks = Album.Info.Tracks;
            int index = tracks.IndexOf(track);
            tracks = tracks.Skip(index);
            return tracks;
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            // Get the rest of the tracks from the album, including the one selected.
            IEnumerable<ITrack> tracks = GetTracksToPlay(model.Track);

            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, tracks)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track);
        }

        private void OnTracksMenuBarEvent(TracksCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Tracks.LastOrDefault())
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Tracks);
        }

        #endregion Methods
    }
}