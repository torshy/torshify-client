using System;
using System.Collections.Generic;
using System.Linq;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core
{
    public class PlayerCommandsHandler : IInitializable
    {
        #region Fields

        private readonly IPlayer _player;

        #endregion Fields

        #region Constructors

        public PlayerCommandsHandler(IPlayer player)
        {
            _player = player;
        }

        #endregion Constructors

        #region Public Methods

        public void Initialize()
        {
            CoreCommands
                .PlayTrackCommand
                .RegisterCommand(new AutomaticCommand<ITrack>(ExecutePlayTrack, CanExecutePlayTrack));

            CoreCommands
                .QueueTrackCommand
                .RegisterCommand(new AutomaticCommand<ITrack>(ExecuteQueueTrack, CanExecuteQueueTrack));

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
        }

        #endregion Public Methods

        #region Private Methods

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

        private bool CanExecuteQueueTrack(ITrack track)
        {
            return true;
        }

        private void ExecuteQueueTrack(ITrack track)
        {
            _player.Playlist.Enqueue(track);
        }

        private bool CanExecutePlayTrack(ITrack track)
        {
            return true;
        }

        private void ExecutePlayTrack(ITrack track)
        {
            IPlaylistTrack playlistTrack = track as IPlaylistTrack;

            if (playlistTrack != null)
            {
                int index = playlistTrack.Playlist.Tracks.IndexOf(playlistTrack);

                List<ITrack> tracks = new List<ITrack>();

                for (int i = index; i < playlistTrack.Playlist.Tracks.Count(); i++)
                {
                    tracks.Add(playlistTrack.Playlist.Tracks.ElementAt(i));
                }

                _player.Playlist.Set(tracks);
                _player.Play();
            }
            else
            {
                _player.Playlist.Set(new[] { track });
                _player.Play();

                // 1. Get all tracks by artist
                // 2. Jump to the track
                // 3. Create a playlist with the remaining tracks
            }
        }

        #endregion Private Methods
    }
}