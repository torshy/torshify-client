using System;
using System.Windows.Media.Imaging;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class ArtistInformation : IArtistInformation
    {
        public INotifyEnumerable<IAlbum> Albums { get; set; }
        public INotifyEnumerable<ITrack> Tracks { get; set; }
        public string Biography { get; set; }
        public INotifyEnumerable<IArtist> SimilarArtists { get; set; }
        public INotifyEnumerable<BitmapSource> Portraits { get; set; }
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