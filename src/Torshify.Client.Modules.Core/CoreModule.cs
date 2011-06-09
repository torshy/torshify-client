using System;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Modules.Core.Views;
using Torshify.Client.Modules.Core.Views.Playlist;
using Torshify.Client.Modules.Core.Views.Playlists;

namespace Torshify.Client.Modules.Core
{
    public class CoreModule : IModule
    {
        #region Fields

        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly IPlayer _player;

        #endregion Fields

        #region Constructors

        public CoreModule(
            IUnityContainer container,
            IRegionManager regionManager,
            IPlayer player)
        {
            _container = container;
            _regionManager = regionManager;
            _player = player;
        }

        #endregion Constructors

        #region Public Methods

        public void Initialize()
        {
            _container.RegisterType<MainView>("MainView");
            _container.RegisterType<PlaylistView>(MusicRegionViewNames.PlaylistView);

            _regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(MainView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.LeftMusicRegion, typeof(PlaylistsView));
            _regionManager.RegisterViewWithRegion(CoreRegionNames.MainMusicRegion, typeof(PlaylistView));

            _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("MainView", UriKind.Relative));

            CoreCommands
                .PlayTrackCommand
                .RegisterCommand(new AutomaticCommand<ITrack>(ExecutePlayTrack, CanExecutePlayTrack));

            CoreCommands
                .QueueTrackCommand
                .RegisterCommand(new AutomaticCommand<ITrack>(ExecuteQueueTrack, CanExecuteQueueTrack));
        }

        #endregion Public Methods

        #region Private Methods

        private bool CanExecuteQueueTrack(ITrack track)
        {
            return true;
        }

        private void ExecuteQueueTrack(ITrack track)
        {
            _player.Enqueue(track);
        }

        private bool CanExecutePlayTrack(ITrack track)
        {
            return true;
        }

        private void ExecutePlayTrack(ITrack track)
        {
            _player.Play(track);
        }

        #endregion Private Methods
    }
}