using System;
using System.Security.Authentication;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;

namespace Torshify.Client.Spotify.Views.Login
{
    public class LoginViewModel : NotificationObject, IRegionMemberLifetime
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly IRegionManager _regionManager;
        private readonly ILoggerFacade _logger;
        private readonly ISession _session;

        private bool _hasLoginError;
        private bool _isLoggingIn;
        private string _loginError;
        private bool _rememberMe;
        private string _userName;

        #endregion Fields

        #region Constructors

        public LoginViewModel(
            ISession session,
            IRegionManager regionManager,
            ILoggerFacade logger,
            Dispatcher dispatcher)
        {
            _logger = logger;
            _session = session;
            _session.LoginComplete += OnLoginComplete;
            _rememberMe = !string.IsNullOrEmpty(_session.GetRememberedUser());
            _regionManager = regionManager;
            _dispatcher = dispatcher;

            LoginCommand = new AutomaticCommand<PasswordBox>(ExecuteLogin, CanExecuteLogin);
            IsLoggingIn = false;

            if (RememberMe)
            {
                try
                {
                    _session.Relogin();
                }
                catch (AuthenticationException ae)
                {
                    _logger.Log(ae.Message, Category.Warn, Priority.Medium);
                }
                catch(Exception ex)
                {
                    _logger.Log(ex.ToString(), Category.Exception, Priority.High);
                }
            }
        }

        #endregion Constructors

        #region Properties

        public Uri ForgotPasswordUrl
        {
            get
            {
                return new Uri("https://www.spotify.com/password-reset/?utm_source=spotify&utm_medium=login_box&utm_campaign=forgot_password", UriKind.Absolute);
            }
        }

        public bool HasLoginError
        {
            get { return _hasLoginError; }
            set
            {
                _hasLoginError = value;
                RaisePropertyChanged("HasLoginError");
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

        public bool KeepAlive
        {
            get { return false; }
        }

        public AutomaticCommand<PasswordBox> LoginCommand
        {
            get;
            private set;
        }

        public string LoginError
        {
            get { return _loginError; }
            set
            {
                _loginError = value;
                RaisePropertyChanged("LoginError");
            }
        }

        public bool RememberMe
        {
            get
            {
                return _rememberMe;
            }
            set
            {
                _rememberMe = value;
                RaisePropertyChanged("RememberMe");
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

        #endregion Properties

        #region Methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        private bool CanExecuteLogin(PasswordBox pbox)
        {
            return !string.IsNullOrEmpty(UserName) && pbox.SecurePassword.Length > 0;
        }

        private void ExecuteLogin(PasswordBox pbox)
        {
            IsLoggingIn = true;
            
            if (!string.IsNullOrEmpty(_session.GetRememberedUser()) && !RememberMe)
            {
                _session.ForgetStoredLogin();
            }

            _session.Login(UserName, pbox.Password, RememberMe);
        }

        private void OnLoginComplete(object sender, SessionEventArgs e)
        {
            IsLoggingIn = false;

            if (e.Status != Error.OK)
            {
                // TODO : Display error to user
                HasLoginError = true;
                LoginError = e.Status.GetMessage();
            }
            else
            {
                HasLoginError = false;
                LoginError = string.Empty;

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

        #endregion Methods
    }
}