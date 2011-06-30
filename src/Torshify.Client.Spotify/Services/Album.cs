using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Interfaces;
using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class Album : NotificationObject, ITorshifyAlbum
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        
        private Lazy<Artist> _artist;
        
        #endregion Fields

        #region Constructors

        public Album(IAlbum album, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            InternalAlbum = album;

            _artist = new Lazy<Artist>(() => new Artist(InternalAlbum.Artist));
        }

        #endregion Constructors

        #region Properties

        public ITorshifyArtist Artist
        {
            get { return _artist.Value; }
        }

        public BitmapSource Cover
        {
            get
            {
                string coverId = InternalAlbum.CoverId;

                var source = MemoryCache.Default.Get(coverId) as BitmapSource;

                if (source == null)
                {
                    if (!string.IsNullOrEmpty(coverId))
                    {
                        IImage image = InternalAlbum.Session.GetImage(coverId);

                        if (!image.IsLoaded)
                        {
                            image.Loaded += OnCoverImageLoaded;
                        }
                        else
                        {
                            InitializeCover(image);
                        }
                    }
                }

                return source;
            }
        }

        public IAlbumInformation Info
        {
            get
            {
                var albumInfo = MemoryCache.Default.Get("Torshify_AlbumInfo_" + GetHashCode()) as Lazy<AlbumInformation>;

                if (albumInfo == null)
                {
                    albumInfo = new Lazy<AlbumInformation>(() => new AlbumInformation(InternalAlbum, _dispatcher));

                    MemoryCache.Default.Add(
                        "Torshify_AlbumInfo_" + GetHashCode(),
                        albumInfo,
                        new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(45) });
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
            get { return InternalAlbum.IsAvailable; }
        }

        public string Name
        {
            get { return InternalAlbum.Name; }
        }

        public int Year
        {
            get { return InternalAlbum.Year; }
        }

        #endregion Properties

        #region Methods

        private void InitializeCover(IImage image)
        {
            if (image.Error == Error.OK && image.Data.Length > 0)
            {
                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.None;
                    bitmapImage.StreamSource = new MemoryStream(image.Data);
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    MemoryCache.Default.Add(
                        image.ImageId,
                        bitmapImage,
                        new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(1) });

                    RaisePropertyChanged("Cover");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void OnCoverImageLoaded(object sender, EventArgs e)
        {
            IImage image = (IImage)sender;
            image.Loaded -= OnCoverImageLoaded;

            InitializeCover(image);
        }

        #endregion Methods
    }
}