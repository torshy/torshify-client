using System.Collections.Generic;

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

        IEnumerable<IPlaylistTrack> Tracks
        {
            get;
        }
    }
}