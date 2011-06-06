using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;

namespace Torshify.Client.Modules.Core.Views
{
    public class MainViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private string _inputFieldText;

        #endregion Fields

        #region Constructors

        public MainViewModel(IRegionManager regionManager)
        {
            RegionManager = regionManager;
            SearchCommand = new AutomaticCommand<string>(ExecuteSearchCommand, CanExecuteSearchCommand);
            GoBackCommand = new AutomaticCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand);
            GoForwardCommand = new AutomaticCommand(ExecuteGoForwardCommand, CanExecuteGoForwardCommand);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand GoForwardCommand
        {
            get;
            private set;
        }

        public AutomaticCommand GoBackCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<string> SearchCommand
        {
            get;
            private set;
        }

        public string InputFieldText
        {
            get
            {
                return _inputFieldText;
            }
            set
            {
                if (_inputFieldText != value)
                {
                    _inputFieldText = value;
                    RaisePropertyChanged("InputFieldText");
                }
            }
        }

        protected IRegionManager RegionManager
        {
            get;
            private set;
        }

        protected IRegion MusicViewRegion
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Methods

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (MusicViewRegion == null)
            {
                MusicViewRegion = RegionManager.Regions[CoreRegionNames.MainMusicRegion];
            }
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

        private bool CanExecuteGoForwardCommand()
        {
            return MusicViewRegion != null && MusicViewRegion.NavigationService.Journal.CanGoForward;
        }

        private void ExecuteGoForwardCommand()
        {
            MusicViewRegion.NavigationService.Journal.GoForward();
        }

        private bool CanExecuteGoBackCommand()
        {
            return MusicViewRegion != null && MusicViewRegion.NavigationService.Journal.CanGoBack;
        }

        private void ExecuteGoBackCommand()
        {
            MusicViewRegion.NavigationService.Journal.GoBack();
        }

        private bool CanExecuteSearchCommand(string text)
        {
            return MusicViewRegion != null && !string.IsNullOrEmpty(text);
        }

        private void ExecuteSearchCommand(string text)
        {
            //Search(text);
        }

        #endregion Private Methods
    }
}