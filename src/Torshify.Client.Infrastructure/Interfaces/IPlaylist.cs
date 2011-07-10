namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IPlaylist
    {
        string Name
        {
            get; 
            set;
        }

        string Description
        {
            get;
        }

        bool IsCollaborative
        {
            get; 
            set; 
        }

        bool IsUpdating
        {
            get;
        }

        INotifyEnumerable<IPlaylistTrack> Tracks
        {
            get;
        }

        void MoveTrack(int oldIndex, int newIndex);

        void MoveTracks(int[] indices, int newIndex);

        void RemoveTrack(IPlaylistTrack track);
    }
}