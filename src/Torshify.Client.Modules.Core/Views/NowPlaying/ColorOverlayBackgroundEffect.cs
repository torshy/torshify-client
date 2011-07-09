using Microsoft.Practices.Prism.Regions;
using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core.Controls;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class ColorOverlayBackgroundEffect : IBackgroundEffect
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public ColorOverlayBackgroundEffect(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Methods

        public void NavigatedFrom()
        {
            IRegion backgroundOverlayRegion = _regionManager.Regions[RegionNames.BackgroundOverlayRegion];
            var colorOverlayView = backgroundOverlayRegion.GetView("ColorOverlay");

            if (colorOverlayView != null)
            {
                backgroundOverlayRegion.Remove(colorOverlayView);
            }
        }

        public void NavigatedTo()
        {
            IRegion backgroundOverlayRegion = _regionManager.Regions[RegionNames.BackgroundOverlayRegion];
            var colorOverlayView = backgroundOverlayRegion.GetView("ColorOverlay");

            if (colorOverlayView == null)
            {
                backgroundOverlayRegion.Add(new ColorOverlayFrame(), "ColorOverlay");
            }
        }

        public void OnTrackChanged(ITrack previous, ITrack current)
        {
        }

        #endregion Methods
    }
}