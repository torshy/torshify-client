using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Input;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;
using Torshify.Client.Modules.Core.Controls;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IBackdropService _backdropService;
        private readonly Dispatcher _dispatcher;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
        private readonly IPlayerController _player;
        private readonly IRegionManager _regionManager;

        private SubscriptionToken _appInactivityToken;
        private DispatcherTimer _backdropDelayDownloadTimer;
        private PlayerQueueItem _currentTrack;
        private bool _hackFirstTime = true;
        private bool _isUserInactive;
        private IRegionNavigationService _navigationService;
        private double _requestSeek;
        private SubscriptionToken _sysInactivityToken;

        #endregion Fields

        #region Constructors

        public NowPlayingViewModel(
            IRegionManager regionManager,
            IPlayerController player,
            IBackdropService backdropService,
            IEventAggregator eventAggregator,
            ILoggerFacade logger,
            Dispatcher dispatcher)
        {
            _regionManager = regionManager;
            _player = player;
            _backdropService = backdropService;
            _eventAggregator = eventAggregator;
            _logger = logger;
            _dispatcher = dispatcher;
            _backdropDelayDownloadTimer = new DispatcherTimer();
            _backdropDelayDownloadTimer.Interval = TimeSpan.FromSeconds(1);
            _backdropDelayDownloadTimer.Tick += OnDelayedBackdropFetchTimerElapsed;
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

            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            var kenBurnsView = region.GetView("KenBurnsBackground");

            if (kenBurnsView != null)
            {
                region.Remove(kenBurnsView);
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

            if (CurrentTrack != null)
            {
                GetBackdropForTrack(CurrentTrack.Track);
            }
        }

        private void DisplayBackgroundImage(ImageSource imageSource)
        {
            RemoveKenBurnsEffect();

            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            ImageMontage montage = new ImageMontage();
            montage.Initialize(imageSource);
            montage.UI.InputBindings.Add(
                new ExtendedMouseBinding
                {
                    Command = NavigateBackCommand,
                    Gesture = new ExtendedMouseGesture(MouseButton.XButton1)
                });
            region.Add(montage.UI, "KenBurnsBackground");
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

        private void GetBackdropForTrack(ITrack track)
        {
            if (track == null || track.Album == null || track.Album.Artist == null)
                return;

            _backdropService.GetBackdrop(
                track.Album.Artist.Name,
                backdropFile =>
                {
                    Task.Factory.StartNew(() =>
                                            {
                                                try
                                                {
                                                    BitmapImage bitmapImage = new BitmapImage();
                                                    bitmapImage.BeginInit();
                                                    bitmapImage.DecodePixelHeight = 800;
                                                    bitmapImage.StreamSource = new FileStream(
                                                        backdropFile,
                                                        FileMode.Open,
                                                        FileAccess.Read);
                                                    bitmapImage.EndInit();
                                                    bitmapImage.Freeze();

                                                    _dispatcher.BeginInvoke(
                                                        new Action<ImageSource>(DisplayBackgroundImage), bitmapImage);
                                                }
                                                catch (Exception e)
                                                {
                                                    _logger.Log(e.ToString(), Category.Exception, Priority.Medium);
                                                }
                                            });
                },
                didNotFindBackdrop: () => _dispatcher.BeginInvoke((Action)RemoveKenBurnsEffect,
                                                                DispatcherPriority.Background));
        }

        private void OnApplicationInactivity(bool isInactive)
        {
            IsUserInactive = isInactive;
        }

        // TODO : Refactor this method when bothered. Awful stuff
        private void OnCurrentSongChanged(object sender, EventArgs e)
        {
            var previous = CurrentTrack;
            CurrentTrack = _player.Playlist.Current;

            if (CurrentTrack != null)
            {
                ITrack currentTrack = CurrentTrack.Track;

                if (previous != null)
                {
                    _backdropDelayDownloadTimer.Stop();

                    // Only get a new backdrop if the current artist is different than the previous
                    ITrack previousTrack = previous.Track;

                    if (previousTrack.Album != null && currentTrack.Album != null)
                    {
                        if (previousTrack.Album.Artist.Name != currentTrack.Album.Artist.Name)
                        {
                            // Start the timer. This is done to limit the amount of times we need to get a backdrop, if the user presses Next/Previous a alot
                            _backdropDelayDownloadTimer.Tag = currentTrack;
                            _backdropDelayDownloadTimer.Start();
                        }
                    }
                }
                else
                {
                    GetBackdropForTrack(currentTrack);
                }
            }
            else
            {
                NavigateBackCommand.Execute();
            }
        }

        private void OnDelayedBackdropFetchTimerElapsed(object sender, EventArgs eventArgs)
        {
            _backdropDelayDownloadTimer.Stop();
            ITrack track = (ITrack)_backdropDelayDownloadTimer.Tag;
            GetBackdropForTrack(track);
        }

        private void OnSystemInactivity(bool isInactive)
        {
        }

        private void RemoveKenBurnsEffect()
        {
            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            var kenBurnsView = region.GetView("KenBurnsBackground");

            if (kenBurnsView != null)
            {
                region.Remove(kenBurnsView);
            }
        }

        #endregion Methods
    }
}