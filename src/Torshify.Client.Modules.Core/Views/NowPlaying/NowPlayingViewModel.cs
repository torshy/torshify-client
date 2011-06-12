using System;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        private IRegionNavigationService _navigationService;

        #region Constructors

        public NowPlayingViewModel()
        {
            NavigateBackCommand = new StaticCommand(ExecuteNavigateBack);
        }

        #endregion Constructors

        #region Properties

        public StaticCommand NavigateBackCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Methods

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion Public Methods

        #region Private Methods

        private void ExecuteNavigateBack()
        {
            if (_navigationService != null)
            {
                _navigationService.Journal.GoBack();
            }
        }

        #endregion Private Methods
    }
}