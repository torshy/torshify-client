using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}