using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure;

namespace Torshify.Client.Modules.Core.Views.Album
{
    public class AlbumViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private IAlbum _album;
        private SubscriptionToken _trackMenuBarToken;
        private SubscriptionToken _tracksMenuBarToken;

        #endregion Fields

        #region Constructors

        public AlbumViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

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

        public ICommand PlayAlbumTrackCommand
        {
            get; 
            private set;
        }

        #endregion Properties

        #region Methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<TrackCommandBarEvent>().Unsubscribe(_trackMenuBarToken);
            _eventAggregator.GetEvent<TracksCommandBarEvent>().Unsubscribe(_tracksMenuBarToken);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);

            Album = navigationContext.Tag as IAlbum;
        }

        private void ExecutePlayAlbumTrack(ITrack track)
        {
            // Get the rest of the tracks from the album, including the one selected.
            IEnumerable<ITrack> tracks = GetTracksToPlay(track);
            CoreCommands.PlayTrackCommand.Execute(tracks);
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

        private IEnumerable<ITrack> GetTracksToPlay(ITrack track)
        {
            var tracks = Album.Info.Tracks;
            int index = tracks.IndexOf(track);
            tracks = tracks.Skip(index);
            return tracks;
        }

        #endregion Methods
    }
}