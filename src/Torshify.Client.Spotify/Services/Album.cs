using System;
using System.Runtime.Caching;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

using ITorshifyImage = Torshify.Client.Infrastructure.Interfaces.IImage;

using TorshifyAlbumType = Torshify.Client.Infrastructure.Interfaces.AlbumType;

namespace Torshify.Client.Spotify.Services
{
    public class Album : NotificationObject, ITorshifyAlbum
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private Lazy<TorshifyAlbumType> _albumType;
        private Lazy<Artist> _artist;
        private Lazy<string> _name;
        private Lazy<int> _year;
        private object _lockObject = new object();
        private Image _image;

        #endregion Fields

        #region Constructors

        public Album(IAlbum album, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            InternalAlbum = album;

            _artist = new Lazy<Artist>(() => new Artist(InternalAlbum.Artist, dispatcher));
            _albumType = new Lazy<TorshifyAlbumType>(GetAlbumType);
            _name = new Lazy<string>(() => InternalAlbum.Name);
            _year = new Lazy<int>(() => InternalAlbum.Year);
        }

        #endregion Constructors

        #region Properties

        public ITorshifyArtist Artist
        {
            get
            {
                if (InternalAlbum == null || !InternalAlbum.IsValid())
                {
                    return null;
                }

                return _artist.Value;
            }
        }

        public ITorshifyImage CoverArt
        {
            get
            {
                if (InternalAlbum == null || !InternalAlbum.IsValid())
                {
                    return null;
                }

                if (_image == null)
                {
                    _image = new Image(InternalAlbum.Session, InternalAlbum.CoverId);
                }

                return _image;
            }
        }

        public IAlbumInformation Info
        {
            get
            {
                if (InternalAlbum == null || !InternalAlbum.IsValid())
                {
                    return null;
                }

                var albumInfo = MemoryCache.Default.Get("Torshify_AlbumInfo_" + InternalAlbum.GetHashCode()) as Lazy<AlbumInformation>;

                lock (_lockObject)
                {
                    if (albumInfo == null)
                    {
                        albumInfo = new Lazy<AlbumInformation>(() => new AlbumInformation(InternalAlbum, _dispatcher));

                        MemoryCache.Default.Add(
                            "Torshify_AlbumInfo_" + InternalAlbum.GetHashCode(),
                            albumInfo,
                            new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(45) });
                    }
                }

                return albumInfo.Value;
            }
        }

        public IAlbum InternalAlbum
        {
            get;
            private set;
        }

        public bool IsAvailable
        {
            get
            {
                if (InternalAlbum == null || !InternalAlbum.IsValid())
                {
                    return false;
                }

                return InternalAlbum.IsAvailable;
            }
        }

        public string Name
        {
            get
            {
                if (InternalAlbum == null || !InternalAlbum.IsValid())
                {
                    return string.Empty;
                }

                return _name.Value;
            }
        }

        public TorshifyAlbumType Type
        {
            get
            {
                if (InternalAlbum == null || !InternalAlbum.IsValid())
                {
                    return TorshifyAlbumType.Unknown;
                }

                return _albumType.Value;
            }
        }

        public int Year
        {
            get
            {
                if (InternalAlbum == null || !InternalAlbum.IsValid())
                {
                    return 0;
                }

                return _year.Value;
            }
        }

        #endregion Properties

        #region Methods

        private TorshifyAlbumType GetAlbumType()
        {
            switch (InternalAlbum.Type)
            {
                case AlbumType.Album:
                    return TorshifyAlbumType.Album;
                case AlbumType.Single:
                    return TorshifyAlbumType.Single;
                case AlbumType.Compilation:
                    return TorshifyAlbumType.Compilation;
                case AlbumType.Unknown:
                    return TorshifyAlbumType.Unknown;
            }

            return TorshifyAlbumType.Unknown;
        }

        #endregion Methods
    }
}