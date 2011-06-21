using System;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Media.Imaging;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

namespace Torshify.Client.Spotify.Services
{
    public class Album : NotificationObject, ITorshifyAlbum
    {
        #region Fields

        private Lazy<ITorshifyArtist> _artist;

        #endregion Fields

        #region Constructors

        public Album(IAlbum album)
        {
            InternalAlbum = album;

            _artist = new Lazy<ITorshifyArtist>(() => new Artist(InternalAlbum.Artist));
        }

        #endregion Constructors

        #region Properties

        public IAlbum InternalAlbum
        {
            get;
            private set;
        }

        public ITorshifyArtist Artist
        {
            get { return _artist.Value; }
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

        #endregion Properties

        #region Private Methods

        private void OnCoverImageLoaded(object sender, EventArgs e)
        {
            IImage image = (IImage)sender;
            image.Loaded -= OnCoverImageLoaded;

            InitializeCover(image);
        }

        private void InitializeCover(IImage image)
        {
            if (image.Error == Error.OK && image.Data.Length > 0)
            {
                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
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

        #endregion Private Methods
    }
}