using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Models
{
    public class vNextPlayerQueue : NotificationObject, IPlayerQueue
    {
        public event EventHandler CurrentChanged;
        public event EventHandler RepeatChanged;
        public event EventHandler ShuffleChanged;
        public event EventHandler Changed;

        public bool CanGoNext
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanGoPrevious
        {
            get { throw new NotImplementedException(); }
        }

        public PlayerQueueItem Current
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasCurrent
        {
            get { throw new NotImplementedException(); }
        }

        public bool Repeat
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool Shuffle
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IEnumerable<PlayerQueueItem> Left
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<PlayerQueueItem> Queued
        {
            get { throw new NotImplementedException(); }
        }

        public void Enqueue(ITrack track)
        {
            throw new NotImplementedException();
        }

        public void Enqueue(IEnumerable<ITrack> tracks)
        {
            throw new NotImplementedException();
        }

        public bool Next()
        {
            throw new NotImplementedException();
        }

        public bool Previous()
        {
            throw new NotImplementedException();
        }

        public void Set(IEnumerable<ITrack> tracks)
        {
            throw new NotImplementedException();
        }

        public bool IsQueued(ITrack track)
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentTo(PlayerQueueItem item)
        {
            throw new NotImplementedException();
        }
    }
}