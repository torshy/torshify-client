using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IAlbumInformation
    {
        INotifyEnumerable<ITrack> Tracks
        {
            get;
        }

        INotifyEnumerable<string> Copyrights
        {
            get;
        }

        string Review
        {
            get;
        }

        bool IsLoading
        {
            get;
        }
    }
}