using System;
using System.Runtime.Caching;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

namespace Torshify.Client.Spotify.Services
{
    public class Artist : NotificationObject, ITorshifyArtist
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        #endregion Fields

        #region Constructors

        public Artist(IArtist artist, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            InternalArtist = artist;
        }

        #endregion Constructors

        #region Properties

        public IArtistInformation Info
        {
            get
            {
                var artistInfo = MemoryCache.Default.Get("Torshify_ArtistInfo_" + InternalArtist.GetHashCode()) as Lazy<ArtistInformation>;

                if (artistInfo == null)
                {
                    artistInfo = new Lazy<ArtistInformation>(() => new ArtistInformation(this, _dispatcher));

                    MemoryCache.Default.Add(
                        "Torshify_ArtistInfo_" + InternalArtist.GetHashCode(),
                        artistInfo,
                        new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(45) });
                }

                return artistInfo.Value;
            }
        }

        public IArtist InternalArtist
        {
            get; private set;
        }

        public string Name
        {
            get { return InternalArtist.Name; }
        }

        #endregion Properties
    }
}