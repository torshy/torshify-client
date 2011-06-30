using System;
using System.Collections.Generic;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class AlbumInformation : IAlbumInformation
    {
        public IEnumerable<ITrack> Tracks { get; set; }
        public IEnumerable<string> Copyrights { get; set; }
        public string Review { get; set; }
        public bool IsLoading { get; set; }
    }
}