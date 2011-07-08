using System;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlayer : IDisposable
    {
        int EnqueueSamples(int channels, int rate, byte[] samples, int frames);

        void ClearBuffers();

        void Pause();

        void Play();

        void Seek();

        float Volume { get; set; }
    }
}