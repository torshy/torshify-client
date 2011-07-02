using System;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface ISearch
    {
        INotifyEnumerable<IAlbum> Albums
        {
            get;
        }

        INotifyEnumerable<IArtist> Artists
        {
            get;
        }

        INotifyEnumerable<ITrack> Tracks
        {
            get;
        }

        string DidYouMean
        {
            get;
        }

        string Query
        {
            get;
        }

        int TotalArtists
        {
            get;
        }

        int TotalAlbums
        {
            get;
        }

        int TotalTracks
        {
            get;
        }

        bool IsLoading
        {
            get;
        }

        event EventHandler FinishedLoading;
    }
}