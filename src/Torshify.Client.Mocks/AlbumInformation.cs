using System;
using System.Collections.Generic;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class AlbumInformation : IAlbumInformation
    {
        public INotifyEnumerable<ITrack> Tracks { get; set; }
        public INotifyEnumerable<string> Copyrights { get; set; }
        public string Review { get; set; }
        public bool IsLoading { get; set; }
    }
}