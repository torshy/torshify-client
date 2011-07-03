using System;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlayer
    {
        #region Events

        event EventHandler IsPlayingChanged;

        #endregion Events

        #region Properties

        bool IsPlaying
        {
            get;
        }

        TimeSpan DurationPlayed
        {
            get;
        }

        IPlayerQueue Playlist
        {
            get;
        }

        float Volume
        {
            get; 
            set;
        }

        #endregion Properties

        #region Methods

        void Pause();

        void Play();

        void Seek(TimeSpan timeSpan);

        void Stop();

        #endregion Methods
    }
}