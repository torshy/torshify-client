using System;
using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlayerQueue
    {
        #region Events

        event EventHandler CurrentChanged;

        event EventHandler RepeatChanged;

        event EventHandler ShuffleChanged;

        event EventHandler Changed;

        #endregion Events

        #region Properties

        bool CanGoNext
        {
            get;
        }

        bool CanGoPrevious
        {
            get;
        }

        PlayerQueueItem Current
        {
            get;
        }

        bool Repeat
        {
            get; set;
        }

        bool Shuffle
        {
            get; set;
        }

        IEnumerable<PlayerQueueItem> All
        {
            get;
        }

        IEnumerable<PlayerQueueItem> Queued
        {
            get;
        }

        #endregion Properties

        #region Methods

        void Enqueue(ITrack track);

        void Enqueue(IEnumerable<ITrack> tracks);

        bool Next();

        bool Previous();

        void Set(IEnumerable<ITrack> tracks);

        bool IsQueued(ITrack track);

        #endregion Methods
    }

    public class PlayerQueueItem
    {
        public PlayerQueueItem(bool isQueued, ITrack track)
        {
            IsQueued = isQueued;
            Track = track;
        }

        public bool IsQueued { get; private set; }
        public ITrack Track { get; private set; }
    }
}