using System;
using System.Timers;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Spotify.Services
{
    public class PlayerController : NotificationObject, IPlayerController
    {
        #region Fields

        private readonly ISession _session;
        private readonly ILoggerFacade _logger;

        private IPlayer _player;
        private bool _isPlaying;
        private Error? _lastLoadStatus;
        private IPlayerQueue _playlist;
        private TimeSpan _playLocation = TimeSpan.Zero;
        private Timer _timer;
        private float _volume;

        #endregion Fields

        #region Constructors

        public PlayerController(
            ISession session, 
            IPlayer player,
            Dispatcher dispatcher, 
            ILoggerFacade logger)
        {
            _player = player;
            _volume = 0.2f;
            _session = session;
            _logger = logger;
            _session.EndOfTrack += OnSessionEndOfTrack;
            _session.MusicDeliver += OnSessionMusicDeliver;
            _session.PlayTokenLost += OnSessionPlayerTokenLost;
            _session.StopPlayback += OnSessionStopPlayback;
            _session.StartPlayback += OnSessionStartPlayback;

            _playlist = new PlayerQueue(dispatcher);
            _playlist.CurrentChanged += OnCurrentChanged;

            _timer = new Timer(100);
            _timer.Elapsed += OnTimerElapsed;
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsPlayingChanged;

        #endregion Events

        #region Properties

        public TimeSpan DurationPlayed
        {
            get
            {
                return _playLocation;
            }
            set
            {
                Seek(value);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;

                    if (_isPlaying)
                    {
                        _timer.Start();
                    }
                    else
                    {
                        _timer.Stop();
                    }

                    OnIsPlayingChanged();
                }
            }
        }

        public IPlayerQueue Playlist
        {
            get { return _playlist; }
        }

        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = value;
                _player.Volume = value;
                RaisePropertyChanged("Volume");
            }
        }

        #endregion Properties

        #region Methods

        public void Pause()
        {
            if (_lastLoadStatus == Error.OK)
            {
                _session.PlayerPause();
                _player.Pause();

                IsPlaying = false;

                _logger.Log("Paused", Category.Info, Priority.Low);
            }
        }

        public void Play()
        {
            if (!_lastLoadStatus.HasValue)
            {
                if (_playlist.Current == null)
                {
                    _playlist.Next();
                }

                if (_playlist.Current != null)
                {
                    var track = (Track)Playlist.Current.Track;
                    _lastLoadStatus = track.InternalTrack.Load();
                }
            }

            if (_lastLoadStatus.HasValue && _lastLoadStatus == Error.OK)
            {
                _session.PlayerPlay();
                _player.Play();
                _logger.Log("Playing", Category.Info, Priority.Low);

                IsPlaying = true;
            }
        }

        public void Seek(TimeSpan timeSpan)
        {
            if (_isPlaying)
            {
                _session.PlayerSeek(timeSpan);
                _player.Seek();

                _playLocation = timeSpan;
                RaisePropertyChanged("DurationPlayed");

                _logger.Log("Seeking " + timeSpan, Category.Info, Priority.Low);
            }
        }

        public void Stop()
        {
            if (_isPlaying)
            {
                _session.PlayerUnload();
                _player.Dispose();

                IsPlaying = false;
                _playLocation = TimeSpan.Zero;
                RaisePropertyChanged("DurationPlayed");

                _logger.Log("Stop", Category.Info, Priority.Low);
            }
        }

        protected virtual void OnIsPlayingChanged()
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            if (Playlist.Current != null)
            {
                var track = Playlist.Current.Track as Track;

                if (track != null && track.InternalTrack.IsValid())
                {
                    _session.PlayerUnload();
                    _player.ClearBuffers();

                    track.InternalTrack.Load();

                    if (IsPlaying)
                    {
                        track.InternalTrack.Play();
                    }

                    _playLocation = TimeSpan.Zero;

                    _logger.Log("Changing track to " + track.Name, Category.Info, Priority.Low);
                }
            }
            else
            {
                _logger.Log("No more tracks to play.", Category.Info, Priority.Low);

                Stop();
            }
        }

        private void OnSessionEndOfTrack(object sender, SessionEventArgs e)
        {
            if (Playlist.CanGoNext)
            {
                Playlist.Next();
            }
        }

        private void OnSessionMusicDeliver(object sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                e.ConsumedFrames = _player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
                IsPlaying = true;
            }
            else
            {
                e.ConsumedFrames = 0;
                IsPlaying = false;
            }
        }

        private void OnSessionPlayerTokenLost(object sender, SessionEventArgs e)
        {
            IsPlaying = false;
        }

        private void OnSessionStartPlayback(object sender, SessionEventArgs e)
        {
            _logger.Log("Session - Start playback recevied", Category.Info, Priority.Low);
        }

        private void OnSessionStopPlayback(object sender, SessionEventArgs e)
        {
            _logger.Log("Session - Stop playback recevied", Category.Info, Priority.Low);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (Playlist.Current != null)
            {
                if ((DurationPlayed >= Playlist.Current.Track.Duration) && IsPlaying)
                {
                    if (Playlist.CanGoNext)
                    {
                        Playlist.Next();
                    }
                }

                _playLocation = _playLocation.Add(TimeSpan.FromMilliseconds(_timer.Interval));
                RaisePropertyChanged("DurationPlayed");
            }
        }

        #endregion Methods
    }
}