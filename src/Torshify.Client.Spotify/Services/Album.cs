using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Caching;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyAlbum = Torshify.Client.Infrastructure.Interfaces.IAlbum;

using ITorshifyArtist = Torshify.Client.Infrastructure.Interfaces.IArtist;

using ITorshifyTrack = Torshify.Client.Infrastructure.Interfaces.ITrack;

namespace Torshify.Client.Spotify.Services
{
    public class Album : NotificationObject, ITorshifyAlbum
    {
        #region Fields

        private readonly Dispatcher _dispatcher;

        private Lazy<ITorshifyArtist> _artist;

        #endregion Fields

        #region Constructors

        public Album(IAlbum album, Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            InternalAlbum = album;

            _artist = new Lazy<ITorshifyArtist>(() => new Artist(InternalAlbum.Artist));
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

        public IEnumerable<ITorshifyTrack> Tracks
        {
            get
            {
                var tracksCache =
                    MemoryCache.Default.Get("Torshify_AlbumTracks_" + GetHashCode()) as Lazy<IEnumerable<ITorshifyTrack>>;

                if (tracksCache == null)
                {
                    tracksCache = new Lazy<IEnumerable<ITorshifyTrack>>(CreateTrackList);

                    MemoryCache.Default.Add(
                        "Torshify_AlbumTracks_" + GetHashCode(), 
                        tracksCache,
                        new CacheItemPolicy {SlidingExpiration = TimeSpan.FromSeconds(45)});
                }

                return tracksCache.Value;
            }
        }

        public int Year
        {
            get { return InternalAlbum.Year; }
        }

        #endregion Properties

        #region Methods

        private void AlbumBrowseCompleted(object sender, UserDataEventArgs e)
        {
            var trackList = (ObservableCollection<Track>) e.Tag;
            var browse = (IAlbumBrowse) sender;

            using(browse)
            {
                foreach (var spotifyTrack in browse.Tracks)
                {
                    Track track = new Track(spotifyTrack, _dispatcher);
                    _dispatcher.BeginInvoke((Action<Track>) trackList.Add, DispatcherPriority.Background, track);
                }
            }

            browse.Completed -= AlbumBrowseCompleted;
        }

        private IEnumerable<ITorshifyTrack> CreateTrackList()
        {
            var tracks = new ObservableCollection<Track>();
            var browse = InternalAlbum.Browse(tracks);
            browse.Completed += AlbumBrowseCompleted;
            return tracks;
        }

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