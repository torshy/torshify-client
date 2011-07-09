using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core.Controls;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class ImageMapBackgroundEffect : IBackgroundEffect
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public ImageMapBackgroundEffect(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Methods

        public void NavigatedFrom()
        {
            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            var mapView = region.GetView("ImageMap");

            if (mapView != null)
            {
                region.Remove(mapView);
            }
        }

        public void NavigatedTo()
        {
            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            var mapView = region.GetView("ImageMap");

            if (mapView == null)
            {
                var map = new ImageMapFrame();
                map.Initialize(AppConstants.CoverArtCacheFolder);
                region.Add(map, "ImageMap");
            }
        }

        public void OnTrackChanged(ITrack previous, ITrack current)
        {
        }

        #endregion Methods
    }
}