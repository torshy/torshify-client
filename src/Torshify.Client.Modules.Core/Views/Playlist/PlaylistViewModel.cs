using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Playlist
{
    public class PlaylistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private IPlaylist _playlist;
        private SubscriptionToken _trackMenuBarToken;
        private ICollectionView _tracks;
        private SubscriptionToken _tracksMenuBarToken;

        #endregion Fields

        #region Constructors

        public PlaylistViewModel(IPlayerController player, IEventAggregator eventAggregator)
        {
            Player = player;
            _eventAggregator = eventAggregator;
            MoveItemCommand = new AutomaticCommand<Tuple<int, int>>(ExecuteMoveItem, CanExecuteMoveItem);
            RemoveItemCommand = new AutomaticCommand<IPlaylistTrack>(ExecuteRemoveItem, CanExecuteRemoveItem);
            RemoveItemsCommand = new AutomaticCommand<IEnumerable<IPlaylistTrack>>(ExecuteRemoveItems,
                                                                                   CanExecuteRemoveItems);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand<Tuple<int, int>> MoveItemCommand
        {
            get;
            private set;
        }

        public IPlayerController Player
        {
            get; private set;
        }

        public IPlaylist Playlist
        {
            get
            {
                return _playlist;
            }
            set
            {
                _playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }

        public AutomaticCommand<IPlaylistTrack> RemoveItemCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<IEnumerable<IPlaylistTrack>> RemoveItemsCommand
        {
            get; private set;
        }

        public ICollectionView Tracks
        {
            get
            {
                return _tracks;
            }
            private set
            {
                _tracks = value;
                RaisePropertyChanged("Tracks");
            }
        }

        #endregion Properties

        #region Methods

        bool INavigationAware.IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        void INavigationAware.OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<TrackCommandBarEvent>().Unsubscribe(_trackMenuBarToken);
            _eventAggregator.GetEvent<TracksCommandBarEvent>().Unsubscribe(_tracksMenuBarToken);

            if (Playlist != null && Tracks != null)
            {
                var selectedTrack = (ITrack)Tracks.CurrentItem;

                if (selectedTrack != null)
                {
                    UriQuery query = new UriQuery();
                    query.Add("PlaylistSelectedTrackID", selectedTrack.ID.ToString());

                    navigationContext.NavigationService.Journal.CurrentEntry.Uri = new Uri(MusicRegionViewNames.PlaylistView + query, UriKind.Relative);
                }
            }
        }

        void INavigationAware.OnNavigatedTo(NavigationContext navigationContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);

            Tracks = null;

            Playlist = navigationContext.Tag as IPlaylist;

            if (Playlist != null)
            {
                if (Playlist.Tracks != null)
                {
                    Tracks = new ListCollectionView((IList)Playlist.Tracks);

                    if (navigationContext.Parameters["PlaylistSelectedTrackID"] != null)
                    {
                        int trackId = int.Parse(navigationContext.Parameters["PlaylistSelectedTrackID"]);
                        var track = Playlist.Tracks.FirstOrDefault(t => t.ID == trackId);

                        if (track != null)
                        {
                            Tracks.MoveCurrentTo(track);
                        }
                    }
                }
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            Playlist.MoveTrack(oldIndex, newIndex);
        }

        private bool CanExecuteMoveItem(Tuple<int, int> arg)
        {
            return arg != null && arg.Item1 != arg.Item2;
        }

        private bool CanExecuteRemoveItem(IPlaylistTrack track)
        {
            return track != null && Playlist.Tracks.Contains(track);
        }

        private bool CanExecuteRemoveItems(IEnumerable<IPlaylistTrack> tracks)
        {
            return tracks != null;
        }

        private void ExecuteMoveItem(Tuple<int, int> oldAndNewIndex)
        {
            Move(oldAndNewIndex.Item1, oldAndNewIndex.Item2);
        }

        private void ExecuteRemoveItem(IPlaylistTrack track)
        {
            Playlist.RemoveTrack(track);
        }

        private void ExecuteRemoveItems(IEnumerable<IPlaylistTrack> tracks)
        {
            foreach (var playlistTrack in tracks)
            {
                Playlist.RemoveTrack(playlistTrack);
            }
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Track)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track)
                .AddSeparator()
                .AddCommand("Delete", RemoveItemCommand, model.Track);
        }

        private void OnTracksMenuBarEvent(TracksCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Tracks.LastOrDefault())
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Tracks)
                .AddSeparator()
                .AddCommand("Delete", RemoveItemsCommand, model.Tracks);
        }

        #endregion Methods
    }
}