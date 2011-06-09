using Microsoft.Practices.Prism.Regions;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public class RadioNavigationItem : INavigationItem
    {
        private readonly IRegionManager _regionManager;

        public RadioNavigationItem(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void NavigateTo()
        {

        }
    }
}