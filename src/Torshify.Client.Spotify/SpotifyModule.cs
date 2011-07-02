using System;
using System.Windows;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Spotify.Services;
using Torshify.Client.Spotify.Views.Login;

namespace Torshify.Client.Spotify
{
    public class SpotifyModule : IModule
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;

        #endregion Fields

        #region Constructors

        public SpotifyModule(IRegionManager regionManager, IUnityContainer container)
        {
            _regionManager = regionManager;
            _container = container;
        }

        #endregion Constructors

        #region Properties

        protected static ISession Session
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Static Methods

        public static void InitializeLibspotify()
        {
            Session = SessionFactory.CreateSession(
                Constants.ApplicationKey,
                Constants.CacheFolder,
                Constants.SettingsFolder,
                Constants.UserAgent);

            Application.Current.Exit += delegate
            {
                if (Session != null)
                {
                    Session.Dispose();
                }
            };
        }

        #endregion Public Static Methods

        #region Public Methods

        public void Initialize()
        {
            _container.RegisterInstance(Session);
            _container.RegisterType<IPlaylistProvider, PlaylistProvider>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IPlayer, Player>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISearchProvider, SearchProvider>(new ContainerControlledLifetimeManager());
            _container.RegisterType<LoginView>("LoginView");
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(LoginView));
        }

        #endregion Public Methods
    }
}