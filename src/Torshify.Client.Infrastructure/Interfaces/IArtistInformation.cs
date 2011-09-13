using System;
using System.Windows.Media.Imaging;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IArtistInformation
    {
        INotifyEnumerable<IAlbum> Albums
        {
            get;
        }

        INotifyEnumerable<ITrack> Tracks
        {
            get;
        }

        string Biography
        {
            get;
        }

        INotifyEnumerable<IArtist> SimilarArtists
        {
            get;
        }

        INotifyEnumerable<IImage> Portraits
        {
            get;
        }

        bool IsLoading
        {
            get;
        }

        IImage FirstPortrait { get; }

        event EventHandler FinishedLoading;
    }
}