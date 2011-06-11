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

        ITrack Current
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

        IEnumerable<ITrack> All
        {
            get;
        }

        IEnumerable<ITrack> Queued
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
}