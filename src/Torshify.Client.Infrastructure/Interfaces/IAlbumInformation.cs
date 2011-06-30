using System.Collections.Generic;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IAlbumInformation
    {
        IEnumerable<ITrack> Tracks
        {
            get;
        }

        IEnumerable<string> Copyrights
        {
            get;
        }

        string Review
        {
            get;
        }
    }
}