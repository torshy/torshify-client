using System;
using Microsoft.Practices.Prism.Regions;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class PlayQueueNavigationItem : INavigationItem
    {
        private readonly IRegionManager _regionManager;

        public PlayQueueNavigationItem(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void NavigateTo()
        {
            var uri = new Uri(MusicRegionViewNames.PlayQueueView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri);
        }
    }
}