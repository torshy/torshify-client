using System;
using System.Windows.Input;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure;
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

            CoreCommands.NavigateBackCommand.RegisterCommand(new AutomaticCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand));
            CoreCommands.NavigateForwardCommand.RegisterCommand(new AutomaticCommand(ExecuteGoForwardCommand, CanExecuteGoForwardCommand));
            CoreCommands.SearchCommand.RegisterCommand(new AutomaticCommand<string>(ExecuteSearchCommand, CanExecuteSearchCommand));

            SearchCommand = CoreCommands.SearchCommand;
            GoBackCommand = CoreCommands.NavigateBackCommand;
            GoForwardCommand = CoreCommands.NavigateForwardCommand;
        }

        #endregion Constructors

        #region Properties

        public ICommand GoForwardCommand
        {
            get;
            private set;
        }

        public ICommand GoBackCommand
        {
            get;
            private set;
        }

        public ICommand SearchCommand
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

        public void Search(string text)
        {
            InputFieldText = text;
            UriQuery query = new UriQuery();
            query.Add("Query", text);
            MusicViewRegion.RequestNavigate(new Uri(MusicRegionViewNames.SearchView + query, UriKind.Relative));
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (MusicViewRegion == null)
            {
                MusicViewRegion = RegionManager.Regions[CoreRegionNames.MainMusicRegion];
                MusicViewRegion.RequestNavigate(MusicRegionViewNames.WhatsNew);
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
            Search(text);
        }

        #endregion Private Methods
    }
}