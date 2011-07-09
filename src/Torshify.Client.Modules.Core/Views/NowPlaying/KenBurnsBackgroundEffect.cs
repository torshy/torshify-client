using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core.Controls;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class KenBurnsBackgroundEffect : IBackgroundEffect
    {
        #region Fields

        private readonly IBackdropService _backdropService;
        private readonly Dispatcher _dispatcher;
        private readonly ILoggerFacade _logger;
        private readonly IRegionManager _regionManager;

        private DispatcherTimer _backdropDelayDownloadTimer;

        #endregion Fields

        #region Constructors

        public KenBurnsBackgroundEffect(
            IRegionManager regionManager,
            IBackdropService backdropService,
            ILoggerFacade logger,
            Dispatcher dispatcher)
        {
            _regionManager = regionManager;
            _backdropService = backdropService;
            _logger = logger;
            _dispatcher = dispatcher;
            _backdropDelayDownloadTimer = new DispatcherTimer();
            _backdropDelayDownloadTimer.Interval = TimeSpan.FromSeconds(1);
            _backdropDelayDownloadTimer.Tick += OnDelayedBackdropFetchTimerElapsed;
        }

        #endregion Constructors

        #region Methods

        public void NavigatedFrom()
        {
            RemoveKenBurnsEffect();
        }

        public void NavigatedTo()
        {
        }

        public void OnTrackChanged(ITrack previous, ITrack current)
        {
            if (previous != null)
            {
                _backdropDelayDownloadTimer.Stop();

                // Only get a new backdrop if the current artist is different than the previous
                ITrack previousTrack = previous;

                if (previousTrack.Album != null && current.Album != null)
                {
                    if (previousTrack.Album.Artist.Name != current.Album.Artist.Name)
                    {
                        // Start the timer. This is done to limit the amount of times we need to get a backdrop, if the user presses Next/Previous a alot
                        _backdropDelayDownloadTimer.Tag = current;
                        _backdropDelayDownloadTimer.Start();
                    }
                }
            }
            else
            {
                GetBackdropForTrack(current);
            }
        }

        private void DisplayBackgroundImage(ImageSource imageSource)
        {
            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];

            KenBurnsPhotoFrame frame = region.GetView("KenBurnsBackground") as KenBurnsPhotoFrame;

            if (frame == null)
            {
                frame = new KenBurnsPhotoFrame();
                region.Add(frame, "KenBurnsBackground");
            }

            frame.SetImageSource(imageSource);
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

        private void OnDelayedBackdropFetchTimerElapsed(object sender, EventArgs eventArgs)
        {
            _backdropDelayDownloadTimer.Stop();
            ITrack track = (ITrack)_backdropDelayDownloadTimer.Tag;
            GetBackdropForTrack(track);
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