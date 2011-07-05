using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core
{
    public class PlayerCommandsHandler : IInitializable
    {
        #region Fields

        private readonly IPlayer _player;
        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Constructors

        public PlayerCommandsHandler(IPlayer player, IRegionManager regionManager)
        {
            _player = player;
            _regionManager = regionManager;
        }

        #endregion Constructors

        #region Public Methods

        public void Initialize()
        {
            CoreCommands
                .PlayTrackCommand
                .RegisterCommand(new AutomaticCommand<object>(ExecutePlayTrack, CanExecutePlayTrack));

            CoreCommands
                .QueueTrackCommand
                .RegisterCommand(new AutomaticCommand<object>(ExecuteQueueTrack, CanExecuteQueueTrack));

            CoreCommands
                .GoToNowPlayingCommand
                .RegisterCommand(new AutomaticCommand(ExecuteGoToNowPlaying, CanExecuteGoToNowPlaying));

            CoreCommands.Player
                .PlayCommand
                .RegisterCommand(new AutomaticCommand(ExecutePlay, CanExecutePlay));

            CoreCommands.Player
                .PauseCommand
                .RegisterCommand(new AutomaticCommand(ExecutePause, CanExecutePause));

            CoreCommands.Player
                .NextCommand
                .RegisterCommand(new AutomaticCommand(ExecuteNext, CanExecuteNext));

            CoreCommands.Player
                .PreviousCommand
                .RegisterCommand(new AutomaticCommand(ExecutePrevious, CanExecutePrevious));

            CoreCommands.Player
                .SeekCommand
                .RegisterCommand(new AutomaticCommand<TimeSpan>(ExecuteSeek, CanExecuteSeek));

            CoreCommands.Player
                .ToggleRepeatCommand
                .RegisterCommand(new AutomaticCommand(ExecuteToggleRepeat, CanExecuteToggleRepeat));

            CoreCommands.Player
                .ToggleShuffleCommand
                .RegisterCommand(new AutomaticCommand(ExecuteToggleShuffle, CanExecuteToggleShuffle));
        }

        #endregion Public Methods

        #region Private Methods

        private bool CanExecuteToggleShuffle()
        {
            return true;
        }

        private void ExecuteToggleShuffle()
        {
            _player.Playlist.Shuffle = !_player.Playlist.Shuffle;
        }

        private bool CanExecuteToggleRepeat()
        {
            return true;
        }

        private void ExecuteToggleRepeat()
        {
            _player.Playlist.Repeat = !_player.Playlist.Repeat;
        }

        private bool CanExecuteGoToNowPlaying()
        {
            return true;
        }

        private void ExecuteGoToNowPlaying()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri(MusicRegionViewNames.NowPlayingView, UriKind.Relative));
        }

        private bool CanExecuteSeek(TimeSpan timeSpan)
        {
            return true;
        }

        private void ExecuteSeek(TimeSpan timeSpan)
        {
            _player.Seek(timeSpan);
        }

        private bool CanExecutePrevious()
        {
            return _player.Playlist.CanGoPrevious;
        }

        private void ExecutePrevious()
        {
            _player.Playlist.Previous();
        }

        private bool CanExecuteNext()
        {
            return _player.Playlist.CanGoNext;
        }

        private void ExecuteNext()
        {
            _player.Playlist.Next();
        }

        private bool CanExecutePause()
        {
            return _player.IsPlaying;
        }

        private void ExecutePause()
        {
            _player.Pause();
        }

        private bool CanExecutePlay()
        {
            return !_player.IsPlaying;
        }

        private void ExecutePlay()
        {
            _player.Play();
        }

        private bool CanExecuteQueueTrack(object parameter)
        {
            ITrack track = parameter as ITrack;
            if (track != null)
            {
                return track.IsAvailable;
            }

            IEnumerable<ITrack> tracks = parameter as IEnumerable<ITrack>;
            if (tracks != null)
            {
                return true;
            }

            return false;
        }

        private void ExecuteQueueTrack(object parameter)
        {
            ITrack track = parameter as ITrack;
            if (track != null)
            {
                _player.Playlist.Enqueue(track);
            }

            IEnumerable<ITrack> tracks = parameter as IEnumerable<ITrack>;
            if (tracks != null)
            {
                _player.Playlist.Enqueue(tracks);
            }
        }

        private bool CanExecutePlayTrack(object parameter)
        {
            ITrack track = parameter as ITrack;
            if (track != null)
            {
                return track.IsAvailable;
            }

            IEnumerable<ITrack> tracks = parameter as IEnumerable<ITrack>;
            if (tracks != null)
            {
                return true;
            }

            return false;
        }

        private void ExecutePlayTrack(object parameter)
        {
            IPlaylistTrack playlistTrack = parameter as IPlaylistTrack;
            if (playlistTrack != null)
            {
                int index = playlistTrack.Playlist.Tracks.IndexOf(playlistTrack);

                List<ITrack> tracksToadd = new List<ITrack>();

                for (int i = index; i < playlistTrack.Playlist.Tracks.Count(); i++)
                {
                    IPlaylistTrack item = playlistTrack.Playlist.Tracks.ElementAt(i);

                    if (item.IsAvailable)
                    {
                        tracksToadd.Add(item);
                    }
                }

                _player.Playlist.Set(tracksToadd);
                _player.Play();

                return;
            }

            ITrack track = parameter as ITrack;
            if (track != null)
            {
                _player.Playlist.Set(new[] { track });
                _player.Play();

                return;
            }

            IEnumerable<ITrack> tracks = parameter as IEnumerable<ITrack>;
            if (tracks != null)
            {
                _player.Playlist.Set(tracks);
                _player.Play();

                return;
            }
        }

        #endregion Private Methods
    }
}