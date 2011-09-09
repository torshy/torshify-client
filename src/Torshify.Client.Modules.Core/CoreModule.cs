using System;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure;
using Torshify.Client.Modules.Core.Views;
using Torshify.Client.Modules.Core.Views.Album;
using Torshify.Client.Modules.Core.Views.Artist;
using Torshify.Client.Modules.Core.Views.Navigation;
using Torshify.Client.Modules.Core.Views.Notifications;
using Torshify.Client.Modules.Core.Views.NowPlaying;
using Torshify.Client.Modules.Core.Views.Player;
using Torshify.Client.Modules.Core.Views.Playlist;
using Torshify.Client.Modules.Core.Views.PlayQueue;
using Torshify.Client.Modules.Core.Views.Search;
using Torshify.Client.Modules.Core.Views.Starred;
using Torshify.Client.Modules.Core.Views.WhatsNew;

namespace Torshify.Client.Modules.Core
{
    public class CoreModule : IModule
    {
        #region Fields

        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public CoreModule(
            IUnityContainer container,
            IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Public Methods

        public void Initialize()
        {
            _container.RegisterType<MainView>("MainView");
            _container.RegisterType<PlayerView>("PlayerView");
            _container.RegisterType<NotificationsView>("NotificationsView");
            _container.RegisterType<PlaylistView>(MusicRegionViewNames.PlaylistView);
            _container.RegisterType<PlayQueueView>(MusicRegionViewNames.PlayQueueView);
            _container.RegisterType<NowPlayingView>(MusicRegionViewNames.NowPlayingView);
            _container.RegisterType<AlbumView>(MusicRegionViewNames.AlbumView);
            _container.RegisterType<ArtistView>(MusicRegionViewNames.ArtistView);
            _container.RegisterType<SearchView>(MusicRegionViewNames.SearchView);
            _container.RegisterType<WhatsNewView>(MusicRegionViewNames.WhatsNew);
            _container.RegisterType<StarredView>(MusicRegionViewNames.StarredView);

            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(NowPlayingView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.BottomMusicRegion, typeof(PlayerView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.LeftMusicRegion, typeof(NavigationView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.TopMusicRegion, typeof (NotificationsView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(PlaylistView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(PlayQueueView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(ArtistView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(AlbumView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(SearchView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(WhatsNewView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(StarredView));
            _regionManager.RegisterViewWithRegion("Navigation", typeof(DefaultNavigationView));
#if MockEnabled
            _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("MainView", UriKind.Relative));
#else
            _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("LoginView", UriKind.Relative));
#endif    
            _container.Resolve<PlayerCommandsHandler>().Initialize();
            _container.Resolve<CoreCommandsHandler>().Initialize();
        }

        #endregion Public Methods
    }
}