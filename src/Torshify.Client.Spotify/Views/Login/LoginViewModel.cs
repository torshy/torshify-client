using System;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;

namespace Torshify.Client.Spotify.Views.Login
{
    public class LoginViewModel : NotificationObject, IRegionMemberLifetime
    {
        #region Fields

        private readonly ISession _session;
        private readonly IRegionManager _regionManager;
        private readonly Dispatcher _dispatcher;

        private string _userName;
        private bool _isLoggingIn;

        #endregion Fields

        #region Constructors

        public LoginViewModel(ISession session, IRegionManager regionManager, Dispatcher dispatcher)
        {
            _session = session;
            _regionManager = regionManager;
            _dispatcher = dispatcher;
            _session.LoginComplete += OnLoginComplete;
            LoginCommand = new AutomaticCommand<PasswordBox>(ExecuteLogin, CanExecuteLogin);
        }

        #endregion Constructors

        #region Properties

        public Uri ForgotPasswordUrl
        {
            get
            {
                return new Uri("https://www.spotify.com/no/password-reset/?utm_source=spotify&utm_medium=login_box&utm_campaign=forgot_password", UriKind.Absolute);
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public bool IsLoggingIn
        {
            get
            {
                return _isLoggingIn;
            }
            private set
            {
                _isLoggingIn = value;
                RaisePropertyChanged("IsLoggingIn");
            }
        }

        public AutomaticCommand<PasswordBox> LoginCommand
        {
            get;
            private set;
        }

        public bool KeepAlive
        {
            get { return false; }
        }

        #endregion Properties

        #region Public Methods

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

        #endregion Public Methods

        #region Private Methods

        private void OnLoginComplete(object sender, SessionEventArgs e)
        {
            IsLoggingIn = false;

            _session.LoginComplete -= OnLoginComplete;

            if (e.Status != Error.OK)
            {
                // TODO : Display error to user
            }
            else
            {
                if (!_session.PlaylistContainer.IsLoaded)
                {
                    _session.PlaylistContainer.Loaded += OnPlaylistContainerLoaded;
                }
                else
                {
                    _dispatcher.BeginInvoke(new Action(() => _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("MainView", UriKind.Relative))));
                }
            }
        }

        private void OnPlaylistContainerLoaded(object sender, EventArgs e)
        {
            _session.PlaylistContainer.Loaded -= OnPlaylistContainerLoaded;
            _dispatcher.BeginInvoke(new Action(() => _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("MainView", UriKind.Relative))));
        }

        private bool CanExecuteLogin(PasswordBox pbox)
        {
            return !string.IsNullOrEmpty(UserName) && pbox.SecurePassword.Length > 0;
        }

        private void ExecuteLogin(PasswordBox pbox)
        {
            IsLoggingIn = true;
            _session.Login(UserName, pbox.Password);
        }

        #endregion Private Methods
    }
}