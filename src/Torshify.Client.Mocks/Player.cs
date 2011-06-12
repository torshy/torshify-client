using System;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Mocks
{
    public class Player : IPlayer
    {
        #region Fields

        private PlayerQueue _playerQueue;

        #endregion Fields

        #region Constructors

        public Player()
        {
            _playerQueue = new PlayerQueue();
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsPlayingChanged;

        #endregion Events

        #region Properties

        public bool IsPlaying
        {
            get;
            private set;
        }

        public IPlayerQueue Playlist
        {
            get { return _playerQueue; }
        }

        #endregion Properties

        #region Public Methods

        public void Pause()
        {
        }

        public void Play()
        {
        }

        public void Seek(TimeSpan timeSpan)
        {
        }

        public void Stop()
        {
        }

        #endregion Public Methods
    }
}