using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;
using System.Linq;

namespace Torshify.Client.Infrastructure.Models
{
    public class vNextPlayerQueue : NotificationObject, IPlayerQueue
    {
        #region Fields

        private DispatcherObservableCollection<PlayerQueueItem> _tracksLeft;
        private List<PlayerQueueItem> _currentPlaylist;

        #endregion Fields

        #region Constructors

        public vNextPlayerQueue(Dispatcher dispatcher)
        {
            _tracksLeft = new DispatcherObservableCollection<PlayerQueueItem>(dispatcher);
            _currentPlaylist = new List<PlayerQueueItem>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler Changed;

        public event EventHandler CurrentChanged;

        public event EventHandler RepeatChanged;

        public event EventHandler ShuffleChanged;

        #endregion Events

        #region Properties

        public bool CanGoNext
        {
            get { return _tracksLeft.Count > 0; }
        }

        public bool CanGoPrevious
        {
            get
            {
              return false;
            }
        }

        public PlayerQueueItem Current
        {
            get { return _tracksLeft.FirstOrDefault(); }
        }

        public bool HasCurrent
        {
            get { return Current != null; }
        }

        public IEnumerable<PlayerQueueItem> Left
        {
            get { return _tracksLeft; }
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

        #endregion Properties

        #region Methods

        public void Enqueue(ITrack track)
        {
            if (_tracksLeft.Count > 0)
            {
                _tracksLeft.Insert(1, new PlayerQueueItem(true, track));
            }
            else
            {
                _tracksLeft.Add(new PlayerQueueItem(true, track));
            }
        }

        public void Enqueue(IEnumerable<ITrack> tracks)
        {
            foreach (var track in tracks)
            {
                Enqueue(track);
            }
        }

        public bool IsQueued(ITrack track)
        {
            return _tracksLeft.Any(t => t.IsQueued && t.Track == track);
        }

        public bool MoveCurrentTo(PlayerQueueItem item)
        {
            throw new NotImplementedException();
        }

        public bool Next()
        {
            if (_tracksLeft.Count > 0)
            {
                _tracksLeft.RemoveAt(0);
                return true;
            }

            return false;
        }

        public bool Previous()
        {
            throw new NotImplementedException();
        }

        public void Set(IEnumerable<ITrack> tracks)
        {
            _currentPlaylist.Clear();
            _currentPlaylist.AddRange(tracks.Select(t => new PlayerQueueItem(false, t)));
            
            var queuedTracks = _tracksLeft.Where(t => t.IsQueued).Select(t=>t.Track).ToArray();
            
            _tracksLeft.Clear();
            _tracksLeft.AddRange(_currentPlaylist);

            foreach (var queuedTrack in queuedTracks)
            {
                Enqueue(queuedTrack);
            }
        }

        #endregion Methods
    }

    public class DispatcherObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherObservableCollection(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        protected override void RemoveItem(int index)
        {
            if (_dispatcher.CheckAccess())
            {
                base.RemoveItem(index);
            }
            else
            {
                _dispatcher.Invoke(new Action<int>(RemoveItem), index);
            }
        }

        protected override void ClearItems()
        {
            if (_dispatcher.CheckAccess())
            {
                base.ClearItems();
            }
            else
            {
                _dispatcher.Invoke(new Action(ClearItems));
            }
        }

        protected override void InsertItem(int index, T item)
        {
            if (_dispatcher.CheckAccess())
            {
                base.InsertItem(index, item);
            }
            else
            {
                _dispatcher.Invoke(new Action<int, T>(InsertItem), index, item);
            }
        }

        protected override void SetItem(int index, T item)
        {
            if (_dispatcher.CheckAccess())
            {
                base.SetItem(index, item);
            }
            else
            {
                _dispatcher.Invoke(new Action<int, T>(SetItem), index, item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}