using System;

using NAudio.Wave;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Spotify
{
    public class NAudioPlayer : IPlayer
    {
        #region Fields

        private WaveOut _waveOut;
        private BufferedWaveProvider _waveProvider;

        #endregion Fields

        #region Properties

        public float Volume
        {
            get
            {
                if (_waveOut != null)
                {
                    return _waveOut.Volume;
                }

                return 0.0f;
            }
            set
            {
                if (_waveOut != null)
                {
                    _waveOut.Volume = value;
                }
            }
        }

        #endregion Properties

        #region Methods

        public void ClearBuffers()
        {
            if (_waveOut != null)
            {
                _waveOut.Stop();
                _waveOut.Dispose();
                _waveOut = null;
            }

            _waveProvider = null;
        }

        public void Dispose()
        {
            if (_waveOut != null)
            {
                _waveOut.Dispose();
                _waveOut = null;
            }

            _waveProvider = null;
        }

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            int consumed = 0;

            if (_waveProvider == null)
            {
                _waveOut = new WaveOut();
                _waveProvider = new BufferedWaveProvider(new WaveFormat(rate, channels));
                _waveProvider.BufferDuration = TimeSpan.FromSeconds(5);
                _waveOut.Init(_waveProvider);
                _waveOut.Play();
            }

            if ((_waveProvider.BufferLength - _waveProvider.BufferedBytes) > samples.Length)
            {
                _waveProvider.AddSamples(samples, 0, samples.Length);
                consumed = frames;
            }

            return consumed;
        }

        public void Pause()
        {
            if (_waveOut != null)
            {
                _waveOut.Pause();
            }
        }

        public void Play()
        {
            if (_waveOut != null)
            {
                _waveOut.Play();
            }
        }

        public void Seek()
        {
            ClearBuffers();
        }

        #endregion Methods
    }
}