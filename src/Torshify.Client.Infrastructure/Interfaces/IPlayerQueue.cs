using System;
using System.Collections.Generic;
using Torshify.Client.Infrastructure.Models;

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

        bool HasCurrent
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

        IEnumerable<PlayerQueueItem> Left
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

        bool MoveCurrentTo(PlayerQueueItem item);

        #endregion Methods
    }
}