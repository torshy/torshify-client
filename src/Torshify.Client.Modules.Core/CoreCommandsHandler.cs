using System;
using System.Windows;
using System.Windows.Input;

using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Input;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core
{
    public class CoreCommandsHandler : IInitializable
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public CoreCommandsHandler(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Methods

        public void Initialize()
        {
            Application.Current.MainWindow.InputBindings.Add(
                new ExtendedMouseBinding
                    {
                        Command = CoreCommands.NavigateBackCommand,
                        Gesture = new ExtendedMouseGesture(MouseButton.XButton1)
                    });

            Application.Current.MainWindow.InputBindings.Add(
                new ExtendedMouseBinding
                {
                    Command = CoreCommands.NavigateForwardCommand,
                    Gesture = new ExtendedMouseGesture(MouseButton.XButton2)
                });

            CoreCommands.Views
                .GoToAlbumCommand.RegisterCommand(new AutomaticCommand<IAlbum>(ExecuteGoToAlbum, CanExecuteGoToAlbum));
            CoreCommands.Views
                .GoToArtistCommand.RegisterCommand(new AutomaticCommand<IArtist>(ExecuteGoToArtist, CanExecuteGoToArtist));
            CoreCommands
                .ToggleTrackIsStarredCommand.RegisterCommand(new AutomaticCommand<ITrack>(ExecuteToggleTrackIsStarred, CanExecuteToggleTrackIsStarred));
        }

        private bool CanExecuteGoToAlbum(IAlbum album)
        {
            return album != null;
        }

        private bool CanExecuteGoToArtist(IArtist artist)
        {
            return artist != null;
        }

        private bool CanExecuteToggleTrackIsStarred(ITrack track)
        {
            return track != null && track.IsAvailable;
        }

        private void ExecuteGoToAlbum(IAlbum album)
        {
            Uri uri = new Uri(MusicRegionViewNames.AlbumView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri, album);
        }

        private void ExecuteGoToArtist(IArtist artist)
        {
            Uri uri = new Uri(MusicRegionViewNames.ArtistView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri, artist);
        }

        private void ExecuteToggleTrackIsStarred(ITrack track)
        {
            track.IsStarred = !track.IsStarred;
        }

        #endregion Methods
    }
}