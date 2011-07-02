using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
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
        private readonly IRegionManager _regionManager;
        private readonly IPlayer _player;

        private SubscriptionToken _tracksMenuBarToken;
        private SubscriptionToken _trackMenuBarToken;

        #endregion Fields

        #region Constructors

        public PlayQueueViewModel(
            IPlayer player, 
            IEventAggregator eventAggregator, 
            IRegionManager regionManager)
        {
            _player = player;
            _player.Playlist.Changed += OnPlaylistChanged;

            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            GoToAlbumCommand = new AutomaticCommand<IAlbum>(ExecuteGoToAlbum, CanExecuteGoToAlbum);
            GoToArtistCommand = new AutomaticCommand<IArtist>(ExecuteGoToArtist, CanExecuteGoToArtist);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand<IAlbum> GoToAlbumCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<IArtist> GoToArtistCommand
        {
            get;
            private set;
        }

        public IEnumerable<PlayerQueueItem> Tracks
        {
            get
            {
                return _player.Playlist.Left;
            }
        }

        #endregion Properties

        #region Public Methods

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<TrackCommandBarEvent>().Unsubscribe(_trackMenuBarToken);
            _eventAggregator.GetEvent<TracksCommandBarEvent>().Unsubscribe(_tracksMenuBarToken);
        }

        #endregion Public Methods

        #region Private Methods

        private bool CanExecuteGoToAlbum(IAlbum album)
        {
            return album != null;
        }

        private bool CanExecuteGoToArtist(IArtist artist)
        {
            return artist != null;
        }

        private void ExecuteGoToAlbum(IAlbum album)
        {
            Uri uri = new Uri(MusicRegionViewNames.AlbumView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri, album);
        }

        private void ExecuteGoToArtist(IArtist artist)
        {
            Uri uri = new Uri(MusicRegionViewNames.ArtistView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri, artist);
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Track)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track);
        }

        private void OnTracksMenuBarEvent(TracksCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Tracks.LastOrDefault())
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Tracks);
        }

        private void OnPlaylistChanged(object sender, EventArgs e)
        {
            //RaisePropertyChanged("Tracks");
        }

        #endregion Private Methods
    }
}