using System;
using System.Collections.Generic;
using System.Windows.Input;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerController _player;

        private SubscriptionToken _appInactivityToken;
        private PlayerQueueItem _currentTrack;
        private bool _hackFirstTime = true;
        private bool _isUserInactive;
        private IRegionNavigationService _navigationService;
        private double _requestSeek;
        private SubscriptionToken _sysInactivityToken;

        private List<IBackgroundEffect> _backgroundEffects;

        #endregion Fields

        #region Constructors

        public NowPlayingViewModel(
            IPlayerController player,
            IEventAggregator eventAggregator)
        {
            _player = player;
            _eventAggregator = eventAggregator;
            
            _backgroundEffects = new List<IBackgroundEffect>();
            //_backgroundEffects.Add(ServiceLocator.Current.TryResolve<KenBurnsBackgroundEffect>());
            _backgroundEffects.Add(ServiceLocator.Current.TryResolve<ImageMapBackgroundEffect>());
            _backgroundEffects.Add(ServiceLocator.Current.TryResolve<ColorOverlayBackgroundEffect>());

            NavigateBackCommand = new StaticCommand(ExecuteNavigateBack);
            JumpToTrackCommand = new StaticCommand<PlayerQueueItem>(ExecuteJumpToTrack);
        }

        #endregion Constructors

        #region Properties

        public PlayerQueueItem CurrentTrack
        {
            get
            {
                return _currentTrack;
            }
            private set
            {
                _currentTrack = value;
                RaisePropertyChanged("CurrentTrack");
            }
        }

        public bool IsUserInactive
        {
            get { return _isUserInactive; }
            set
            {
                if (_isUserInactive != value)
                {
                    _isUserInactive = value;
                    RaisePropertyChanged("IsUserInactive");
                }
            }
        }

        public ICommand JumpToTrackCommand
        {
            get;
            private set;
        }

        public StaticCommand NavigateBackCommand
        {
            get;
            private set;
        }

        public IPlayerController Player
        {
            get
            {
                return _player;
            }
        }

        public IEnumerable<PlayerQueueItem> Playlist
        {
            get
            {
                return _player.Playlist.Left;
            }
        }

        public double RequestSeek
        {
            get
            {
                return _requestSeek;
            }
            set
            {
                _requestSeek = value;

                // depressing i know. quick and filthy workaround for a bug that comes first time accessing the now playing view. It seeks to 0
                if (!_hackFirstTime)
                {
                    _player.Seek(TimeSpan.FromSeconds(value));
                }
                else
                {
                    _hackFirstTime = false;
                }
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
            _eventAggregator.GetEvent<ApplicationInactivityEvent>().Unsubscribe(_appInactivityToken);
            _eventAggregator.GetEvent<SystemInactivityEvent>().Unsubscribe(_sysInactivityToken);

            _player.Playlist.CurrentChanged -= OnCurrentSongChanged;

            foreach (var backgroundEffect in _backgroundEffects)
            {
                backgroundEffect.NavigatedFrom();
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _requestSeek = _player.DurationPlayed.TotalSeconds;
            RaisePropertyChanged("RequestSeek");

            _appInactivityToken = _eventAggregator.GetEvent<ApplicationInactivityEvent>().Subscribe(
                OnApplicationInactivity,
                ThreadOption.PublisherThread,
                true);

            _sysInactivityToken = _eventAggregator.GetEvent<SystemInactivityEvent>().Subscribe(
                OnSystemInactivity,
                ThreadOption.PublisherThread,
                true);

            _navigationService = navigationContext.NavigationService;
            _player.Playlist.CurrentChanged += OnCurrentSongChanged;

            CurrentTrack = _player.Playlist.Current;

            foreach (var backgroundEffect in _backgroundEffects)
            {
                backgroundEffect.NavigatedTo();
                backgroundEffect.OnTrackChanged(null, CurrentTrack != null ? CurrentTrack.Track : null);
            }
        }

        private void ExecuteJumpToTrack(PlayerQueueItem item)
        {
            _player.Playlist.MoveCurrentTo(item);
        }

        private void ExecuteNavigateBack()
        {
            if (_navigationService != null)
            {
                _navigationService.Journal.GoBack();
            }
        }

        private void OnApplicationInactivity(bool isInactive)
        {
            IsUserInactive = isInactive;
        }

        private void OnCurrentSongChanged(object sender, EventArgs e)
        {
            var previous = CurrentTrack;
            CurrentTrack = _player.Playlist.Current;

            if (CurrentTrack != null)
            {
                foreach (var backgroundEffect in _backgroundEffects)
                {
                    backgroundEffect.OnTrackChanged(previous.Track, CurrentTrack.Track);
                }
            }
            else
            {
                NavigateBackCommand.Execute();
            }
        }

        private void OnSystemInactivity(bool isInactive)
        {

        }

        #endregion Methods
    }
}