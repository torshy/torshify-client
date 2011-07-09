using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.PlayQueue
{
    public class PlayQueueViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerController _player;
        private readonly IRegionManager _regionManager;

        private IPlayerQueue _playQueue;
        private SubscriptionToken _trackMenuBarToken;
        private SubscriptionToken _tracksMenuBarToken;

        #endregion Fields

        #region Constructors

        public PlayQueueViewModel(
            IPlayerController player,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _player = player;
            _playQueue = player.Playlist;

            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            JumpToTrackCommand = new StaticCommand<PlayerQueueItem>(ExecuteJumpToTrack);
            DeleteTracksCommand = new StaticCommand<IEnumerable>(ExecuteDeleteTracks);
        }

        #endregion Constructors

        #region Properties

        public ICommand DeleteTracksCommand
        {
            get;
            private set;
        }

        public ICommand JumpToTrackCommand
        {
            get;
            private set;
        }

        public IPlayerQueue PlayQueue
        {
            get
            {
                return _playQueue;
            }
            private set
            {
                _playQueue = value;
                RaisePropertyChanged("PlayQueue");
            }
        }

        public IEnumerable<PlayerQueueItem> Tracks
        {
            get
            {
                return _player.Playlist.Left;
            }
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
        }

        private void ExecuteDeleteTracks(IEnumerable tracks)
        {
            if (tracks != null)
            {
                foreach (PlayerQueueItem playerQueueItem in tracks)
                {
                    if (playerQueueItem.IsQueued)
                    {
                        // TODO : Remote track is it is queued
                    }
                }
            }
        }

        private void ExecuteJumpToTrack(PlayerQueueItem item)
        {
            _player.Playlist.MoveCurrentTo(item);
        }

        private IEnumerable<ITrack> GetTracksToPlay(ITrack track)
        {
            int index = Tracks.IndexOf(t => t.Track == track);
            var tracks = Tracks.Skip(index);
            return tracks.Select(t => t.Track).ToList();
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            var tracksToPlay = GetTracksToPlay(model.Track);

            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, tracksToPlay)
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