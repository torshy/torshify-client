using System.Collections.Generic;
using System.Windows.Media.Imaging;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class ArtistInformation : IArtistInformation
    {
        public IEnumerable<IAlbum> Albums { get; set; }
        public IEnumerable<ITrack> Tracks { get; set; }
        public string Biography { get; set; }
        public IEnumerable<IArtist> SimilarArtists { get; set; }
        public IEnumerable<BitmapSource> Portraits { get; set; }
        public bool IsLoading { get; set; }
    }
}