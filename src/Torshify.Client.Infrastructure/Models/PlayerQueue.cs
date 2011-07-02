using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Models
{
    public class PlayerQueue : NotificationObject, IPlayerQueue
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private PlayerQueueItem _currentTrack;
        private ObservableCollection<PlayerQueueItem> _left;
        private List<PlayerQueueItem> _playlist;
        private int[] _playlistIndicies;
        private int _playlistTrackIndex;
        private Queue<PlayerQueueItem> _queue;
        private object _queueLock = new object();
        private bool _repeat;
        private bool _shuffle;

        #endregion Fields

        #region Constructors

        public PlayerQueue(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _queue = new Queue<PlayerQueueItem>();
            _playlist = new List<PlayerQueueItem>();
            _left = new ObservableCollection<PlayerQueueItem>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler Changed;

        public event EventHandler CurrentChanged;

        public event EventHandler RepeatChanged;

        public event EventHandler ShuffleChanged;

        #endregion Events

        #region Properties

        public IEnumerable<PlayerQueueItem> All
        {
            get
            {
                if (Current != null)
                {
                    yield return Current;
                }

                foreach (var track in _queue.ToArray())
                {
                    if (track != Current)
                        yield return track;
                }

                foreach (var track in _playlist.ToArray())
                {
                    if (track != Current)
                        yield return track;
                }
            }
        }

        public bool CanGoNext
        {
            get
            {
                if (_queue.Count > 0)
                {
                    return true;
                }

                if (_playlistIndicies == null || _playlistTrackIndex == -1)
                {
                    return false;
                }

                if (_playlistTrackIndex < (_playlistIndicies.Length - 1))
                {
                    return true;
                }

                if (_repeat)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanGoPrevious
        {
            get
            {
                if (_playlistIndicies == null || _playlistTrackIndex == -1)
                {
                    return false;
                }

                if (_playlistTrackIndex > 0)
                {
                    return true;
                }

                if (_repeat)
                {
                    return true;
                }

                return false;
            }
        }

        public PlayerQueueItem Current
        {
            get
            {
                return _currentTrack;
            }
            private set
            {
                _currentTrack = value;
                OnCurrentChanged();
                RaisePropertyChanged("Current");
                RaisePropertyChanged("HasCurrent");
                RaisePropertyChanged("CanGoNext");
                RaisePropertyChanged("CanGoPrevious");
            }
        }

        public bool HasCurrent
        {
            get
            {
                return Current != null;
            }
        }

        public IEnumerable<PlayerQueueItem> Left
        {
            get
            {
                return _left;
            }
        }

        public IEnumerable<PlayerQueueItem> Queued
        {
            get
            {
                return _queue.ToArray();
            }
        }

        public bool Repeat
        {
            get { return _repeat; }
            set
            {
                _repeat = value;
                OnRepeatChanged();
                RaisePropertyChanged("Repeat");
            }
        }

        public bool Shuffle
        {
            get { return _shuffle; }
            set
            {
                _shuffle = value;
                Update();
                OnShuffleChanged();
                RaisePropertyChanged("Shuffle");
            }
        }

        #endregion Properties

        #region Methods

        public void Enqueue(ITrack track)
        {
            if (_dispatcher.CheckAccess())
            {
                lock (_queueLock)
                {
                    var item = new PlayerQueueItem(true, track);
                    _queue.Enqueue(item);

                    if (_playlist.Count > 0)
                    {
                        _left.Insert(_queue.Count, item);
                    }
                    else
                    {
                        _left.Add(item);
                    }


                }

                OnChanged();
            }
            else
            {
                _dispatcher.BeginInvoke((Action<ITrack>)Enqueue, track);
            }
        }

        public void Enqueue(IEnumerable<ITrack> tracks)
        {
            if (_dispatcher.CheckAccess())
            {
                lock (_queueLock)
                {
                    foreach (var track in tracks)
                    {
                        var item = new PlayerQueueItem(true, track);
                        _queue.Enqueue(item);
                        if (_playlist.Count > 0)
                        {
                            _left.Insert(_queue.Count + 1, item);
                        }
                        else
                        {
                            _left.Add(item);
                        }
                    }
                }

                OnChanged();
            }
            else
            {
                _dispatcher.BeginInvoke((Action<IEnumerable<ITrack>>) Enqueue, tracks);
            }
        }

        public bool IsQueued(ITrack track)
        {
            return _queue.Any(i => i.Track.ID == track.ID);
        }

        public bool Next()
        {
            if (_dispatcher.CheckAccess())
            {
                _left.Remove(Current);
            }
            else
            {
                _dispatcher.Invoke((Func<PlayerQueueItem, bool>)_left.Remove, Current);
            }

            if (_queue.Count > 0)
            {
                Current = _queue.Dequeue();
                return true;
            }

            if (_playlistIndicies == null)
            {
                return false;
            }

            if (_playlistTrackIndex < (_playlistIndicies.Length - 1))
            {
                _playlistTrackIndex++;
                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];
                return true;
            }

            if (Repeat)
            {
                _playlistTrackIndex = 0;
                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];
                return true;
            }

            return false;
        }

        public bool Previous()
        {
            if (_playlistIndicies == null)
            {
                return false;
            }

            if (_playlistTrackIndex > 0)
            {
                _playlistTrackIndex--;

                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];

                if (_dispatcher.CheckAccess())
                {
                    _left.Insert(0, Current);
                }
                else
                {
                    _dispatcher.Invoke((Action<int, PlayerQueueItem>)_left.Insert, 0, Current);
                }

                return true;
            }

            if (_repeat)
            {
                _playlistTrackIndex = _playlistIndicies.Length - 1;

                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];

                if (_dispatcher.CheckAccess())
                {
                    _left.Insert(0, Current);
                }
                else
                {
                    _dispatcher.Invoke((Action<int, PlayerQueueItem>)_left.Insert, 0, Current);
                }

                return true;
            }

            return false;
        }

        public void Set(IEnumerable<ITrack> tracks)
        {
            if (_dispatcher.CheckAccess())
            {
                _playlist = new List<PlayerQueueItem>();
                _left.Clear();

                foreach (var track in tracks)
                {
                    var item = new PlayerQueueItem(false, track);
                    _playlist.Add(item);
                    _left.Add(item);
                }

                Update();

                Current = _playlist[_playlistIndicies[0]];

                OnChanged();
            }
            else
            {
                _dispatcher.BeginInvoke((Action<IEnumerable<ITrack>>)Set, tracks);
            }
        }

        protected virtual void OnChanged()
        {
            var handler = Changed;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnCurrentChanged()
        {
            var handler = CurrentChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnRepeatChanged()
        {
            var handler = RepeatChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnShuffleChanged()
        {
            var handler = ShuffleChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private static int[] BuildShuffledIndexArray(int size)
        {
            int[] array = new int[size];
            Random rand = new Random();
            for (int currentIndex = array.Length - 1; currentIndex > 0; currentIndex--)
            {
                int nextIndex = rand.Next(currentIndex + 1);
                Swap(array, currentIndex, nextIndex);
            }

            return array;
        }

        private static void Swap(IList<int> array, int firstIndex, int secondIndex)
        {
            if (array[firstIndex] == 0)
            {
                array[firstIndex] = firstIndex;
            }

            if (array[secondIndex] == 0)
            {
                array[secondIndex] = secondIndex;
            }

            int temp = array[secondIndex];
            array[secondIndex] = array[firstIndex];
            array[firstIndex] = temp;
        }

        private void Update()
        {
            if (_playlist == null)
            {
                return;
            }

            if (Shuffle)
            {
                _playlistIndicies = BuildShuffledIndexArray(_playlist.Count);
            }
            else
            {
                _playlistIndicies = new int[_playlist.Count];

                for (int i = 0; i < _playlist.Count; i++)
                {
                    _playlistIndicies[i] = i;
                }
            }
        }

        #endregion Methods
    }
}