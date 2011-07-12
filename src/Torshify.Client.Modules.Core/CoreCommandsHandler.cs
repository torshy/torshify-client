using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Input;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core
{
    public class CoreCommandsHandler : IInitializable
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly ILoggerFacade _logger;

        #endregion Fields

        #region Constructors

        public CoreCommandsHandler(IRegionManager regionManager, ILoggerFacade logger)
        {
            _regionManager = regionManager;
            _logger = logger;
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

            Application.Current.MainWindow.InputBindings.Add(
                new KeyBinding
                {
                    Command = CoreCommands.NavigateBackCommand,
                    Gesture = new KeyGesture(Key.Back)
                });

            Application.Current.MainWindow.InputBindings.Add(
                new KeyBinding
                {
                    Command = CoreCommands.NavigateBackCommand,
                    Gesture = new KeyGesture(Key.Left, ModifierKeys.Alt)
                });

            Application.Current.MainWindow.InputBindings.Add(
                new KeyBinding
                {
                    Command = CoreCommands.NavigateForwardCommand,
                    Gesture = new KeyGesture(Key.Right, ModifierKeys.Alt)
                });

            Application.Current.MainWindow.InputBindings.Add(
                new KeyBinding
                {
                    Command = CoreCommands.Debug.ToggleDebugWindowCommand,
                    Gesture = new KeyGesture(Key.D0, ModifierKeys.Alt)
                });

            Application.Current.MainWindow.InputBindings.Add(
                new KeyBinding
                {
                    Command = new StaticCommand(ExecuteGarbageCollection),
                    Gesture = new KeyGesture(Key.OemPlus, ModifierKeys.Alt)
                });

            CoreCommands.Debug
                .ToggleDebugWindowCommand.RegisterCommand(new StaticCommand(ExecuteToggleDebugWindowCommand));
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

        private void ExecuteGarbageCollection()
        {
            _logger.Log("Requesting Garabage Collecton", Category.Info, Priority.Low);
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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

        private void ExecuteToggleDebugWindowCommand()
        {
            ConsoleManager.Toggle();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("...torshify debug window...");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void ExecuteToggleTrackIsStarred(ITrack track)
        {
            track.IsStarred = !track.IsStarred;
        }

        #endregion Methods
    }
}