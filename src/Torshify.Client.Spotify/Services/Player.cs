using System;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Spotify.Services
{
    public class Player : IPlayer
    {
        #region Fields

        private readonly ISession _session;
        private BassPlayer _bass;
        private bool _isPlaying;
        private IPlayerQueue _playlist;
        private Error? _lastLoadStatus;
        private DateTime _trackPaused;
        private DateTime _trackStarted;

        #endregion Fields

        #region Constructors

        public Player(ISession session)
        {
            _bass = new BassPlayer();
            _session = session;
            _session.MusicDeliver += OnMusicDeliver;
            _playlist = new PlayerQueue();
            _playlist.CurrentChanged += OnCurrentChanged;
        }

        #endregion Constructors

        #region Events

        public event EventHandler IsPlayingChanged;

        #endregion Events

        #region Properties

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
                    OnIsPlayingChanged();
                }
            }
        }

        public IPlayerQueue Playlist
        {
            get { return _playlist; }
        }

        #endregion Properties

        #region Public Methods

        public void Pause()
        {
            if (_lastLoadStatus == Error.OK)
            {
                _session.PlayerPause();
                IsPlaying = false;

                if (_trackPaused == DateTime.MinValue)
                {
                    _trackPaused = DateTime.Now;
                }
            }
        }

        public void Play()
        {
            if (!_lastLoadStatus.HasValue && Playlist.Current != null)
            {
                var track = Playlist.Current.Track as Track;
                _lastLoadStatus = track.InternalTrack.Load();
            }

            if (_lastLoadStatus.HasValue && _lastLoadStatus == Error.OK)
            {
                _session.PlayerPlay();
                IsPlaying = true;

                if (_trackPaused != DateTime.MinValue)
                {
                    _trackStarted = _trackStarted + DateTime.Now.Subtract(_trackPaused);
                    _trackPaused = DateTime.MinValue;
                }
                else
                {
                    _trackStarted = DateTime.Now;
                }
            }
        }

        public void Seek(TimeSpan timeSpan)
        {
            if (_isPlaying)
            {
                _session.PlayerSeek(timeSpan);
            }
        }

        public void Stop()
        {
            _session.PlayerUnload();
            IsPlaying = false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnIsPlayingChanged()
        {
            var handler = IsPlayingChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnCurrentChanged(object sender, EventArgs e)
        {
            var track = Playlist.Current.Track as Track;

            if (track != null)
            {
                track.InternalTrack.Load();

                if (IsPlaying)
                {
                    track.InternalTrack.Play();
                }
            }
        }

        private void OnMusicDeliver(object sender, MusicDeliveryEventArgs e)
        {
            if (e.Samples.Length > 0)
            {
                e.ConsumedFrames = _bass.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
                IsPlaying = true;
            }
            else
            {
                e.ConsumedFrames = 0;
                IsPlaying = false;
            }
        }

        #endregion Private Methods
    }
}