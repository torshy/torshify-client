namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlayer
    {
        void Enqueue(ITrack track);
        void Play(ITrack track);
    }
}