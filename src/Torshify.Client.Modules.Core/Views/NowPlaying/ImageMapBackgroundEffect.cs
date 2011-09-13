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
        private ImageMapFrame _map;

        #endregion Fields

        #region Constructors

        public ImageMapBackgroundEffect(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _map = new ImageMapFrame();
        }

        #endregion Constructors

        #region Methods

        public void NavigatedFrom()
        {
            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            region.Remove(_map);
            _map.MapCanvas.Children.Clear();
        }

        public void NavigatedTo()
        {
            IRegion region = _regionManager.Regions[RegionNames.BackgroundRegion];
            var mapView = region.GetView("ImageMap");

            if (mapView == null)
            {
                _map.Initialize(AppConstants.CoverArtCacheFolder);
                region.Add(_map, "ImageMap");
            }
        }

        public void OnTrackChanged(ITrack previous, ITrack current)
        {
        }

        #endregion Methods
    }
}