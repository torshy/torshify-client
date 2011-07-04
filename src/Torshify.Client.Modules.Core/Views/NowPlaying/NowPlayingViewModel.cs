using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
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
        private readonly IPlayer _player;
        private readonly IRegionManager _regionManager;

        private SubscriptionToken _appInactivityToken;
        private IRegionNavigationService _navigationService;
        private SubscriptionToken _sysInactivityToken;
        private bool _isUserInactive;

        #endregion Fields

        #region Constructors

        public NowPlayingViewModel(
            IRegionManager regionManager,
            IPlayer player,
            IBackdropService backdropService,
            IEventAggregator eventAggregator,
            Dispatcher dispatcher)
        {
            _regionManager = regionManager;
            _player = player;
            _backdropService = backdropService;
            _eventAggregator = eventAggregator;
            _dispatcher = dispatcher;

            NavigateBackCommand = new StaticCommand(ExecuteNavigateBack);
        }

        #endregion Constructors

        #region Properties

        public PlayerQueueItem CurrentTrack
        {
            get
            {
                if (_player.Playlist.Current != null)
                {
                    return _player.Playlist.Current;
                }

                return null;
            }
        }

        public StaticCommand NavigateBackCommand
        {
            get;
            private set;
        }

        public IPlayer Player
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
            _appInactivityToken = _eventAggregator.GetEvent<ApplicationInactivityEvent>().Subscribe(
                OnApplicationInactivity,
                ThreadOption.PublisherThread,
                true);

            _sysInactivityToken =_eventAggregator.GetEvent<SystemInactivityEvent>().Subscribe(
                OnSystemInactivity,
                ThreadOption.PublisherThread,
                true);

            _navigationService = navigationContext.NavigationService;

            if (_player.Playlist.Current != null)
            {
                GetBackdropForTrack(_player.Playlist.Current.Track);
            }

            _player.Playlist.CurrentChanged += OnCurrentSongChanged;

            RaisePropertyChanged("CurrentTrack");
        }

        private void DisplayBackgroundImage(ImageSource imageSource)
        {
            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            var kenBurnsView = region.GetView("KenBurnsBackground");

            if (kenBurnsView != null)
            {
                region.Remove(kenBurnsView);
            }

            ImageMontage montage = new ImageMontage();
            montage.Initialize(imageSource);
            region.Add(montage.UI, "KenBurnsBackground");
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

            _backdropService.GetBackdrop(track.Album.Artist.Name, backdropFile =>
            {
                Task.Factory.StartNew(() =>
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
                                        });
            });
        }

        private void OnApplicationInactivity(bool isInactive)
        {
            IsUserInactive = isInactive;
        }

        private void OnCurrentSongChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("CurrentTrack");

            PlayerQueueItem currentPlaying = _player.Playlist.Current;

            if (currentPlaying != null)
            {
                GetBackdropForTrack(currentPlaying.Track);
            }
        }

        private void OnSystemInactivity(bool isInactive)
        {
        }

        #endregion Methods
    }
}