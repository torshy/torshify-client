using System;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Search : ISearch
    {
        public INotifyEnumerable<IAlbum> Albums { get; set; }
        public INotifyEnumerable<IArtist> Artists { get; set; }
        public INotifyEnumerable<ITrack> Tracks { get; set; }
        public string DidYouMean { get; set; }
        public string Query { get; set; }
        public int TotalArtists { get; set; }
        public int TotalAlbums { get; set; }
        public int TotalTracks { get; set; }
        public bool IsLoading { get; set; }
        public event EventHandler FinishedLoading;

        public void RaiseFinishedLoading()
        {
            var handler = FinishedLoading;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}