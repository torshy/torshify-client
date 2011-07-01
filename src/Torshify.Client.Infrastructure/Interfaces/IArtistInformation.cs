using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IArtistInformation
    {
        IEnumerable<IAlbum> Albums
        {
            get;
        }

        IEnumerable<ITrack> Tracks
        {
            get;
        }

        string Biography
        {
            get;
        }

        IEnumerable<IArtist> SimilarArtists
        {
            get;
        }

        IEnumerable<BitmapSource> Portraits
        {
            get;
        }

        bool IsLoading
        {
            get;
        }
    }
}