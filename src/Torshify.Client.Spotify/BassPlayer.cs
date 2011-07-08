using System;
using Torshify.Client.Infrastructure.Interfaces;
using Un4seen.Bass;

namespace Torshify.Client.Spotify
{
    public class BassPlayer : IPlayer
    {
        #region Fields

        private BASSBuffer _bassBuffer = null;
        private bool _lazyVolumeSet;
        private float _lazyVolumeValue;
        private STREAMPROC _streamproc = null;

        #endregion Fields

        #region Properties

        public void Seek()
        {
            if (_bassBuffer != null)
            {
                _bassBuffer.Clear();
            }
        }

        public float Volume
        {
            get
            {
                return Bass.BASS_GetVolume();
            }
            set
            {
                if (!Bass.BASS_SetVolume(value))
                {
                    _lazyVolumeValue = value;
                    _lazyVolumeSet = true;
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            if (_bassBuffer != null)
            {
                _bassBuffer.Dispose();
                _bassBuffer = null;
            }

            Bass.BASS_Free();
        }

        public int EnqueueSamples(int channels, int rate, byte[] samples, int frames)
        {
            int consumed = 0;
            if (_bassBuffer == null)
            {
                Bass.BASS_Init(-1, rate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                _bassBuffer = new BASSBuffer(0.5f, rate, channels, 2);
                _streamproc = new STREAMPROC(Reader);

                if (_lazyVolumeSet)
                {
                    Volume = _lazyVolumeValue;
                }

                Bass.BASS_ChannelPlay(
                    Bass.BASS_StreamCreate(rate, channels, BASSFlag.BASS_DEFAULT, _streamproc, IntPtr.Zero),
                    false
                    );
            }

            if (_bassBuffer.Space(0) > samples.Length)
            {
                _bassBuffer.Write(samples, samples.Length);
                consumed = frames;
            }

            return consumed;
        }

        public void ClearBuffers()
        {
            // In real world usage you must remember to free the BASS stream if not reusing it!
            if (_bassBuffer != null)
            {
                _bassBuffer.Clear();
            }
        }

        public void Pause()
        {
            Bass.BASS_Pause();
        }

        public void Play()
        {
            Bass.BASS_Start();
        }

        private int Reader(int handle, IntPtr buffer, int length, IntPtr user)
        {
            return _bassBuffer.Read(buffer, length, user.ToInt32());
        }

        #endregion Methods
    }
}