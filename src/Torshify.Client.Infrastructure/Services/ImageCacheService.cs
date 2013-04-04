using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Caching;
using System.Threading;
using System.Windows.Media.Imaging;

using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Services
{
    public sealed class ImageCacheService : IImageCacheService
    {
        #region Fields

        private MemoryCache _cache = MemoryCache.Default;

        #endregion Fields

        #region Properties

        public string CacheLocation
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public IImageCacheEntry GetImage(IImage image, int decodeWidth = 300, int decodeHeight = 300)
        {
            string cacheKey = "ImageCacheService_" + image.ID + "_" + decodeWidth + "x" + decodeHeight;
            var entry = _cache.Get(cacheKey) as IImageCacheEntry;

            if (entry == null)
            {
                string filePath;

                if (TryGetFilePath(image.ID, out filePath))
                {
                    entry = new DiskImageCacheEntry(
                        image.ID,
                        filePath,
                        decodeWidth,
                        decodeHeight);

                    _cache.Add(cacheKey, entry, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromSeconds(30) });
                }
                else
                {
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        entry = new FirstTimeImageCacheEntry(image, filePath, decodeWidth, decodeHeight);
                    }
                }
            }

            return entry;
        }

        private bool TryGetFilePath(string imageId, out string filePath)
        {
            if (imageId.Length > 2)
            {
                string index = imageId.Substring(0, 2);
                string indexDirectory = Path.Combine(CacheLocation, index);
                Directory.CreateDirectory(indexDirectory);

                filePath = Path.Combine(indexDirectory, imageId + ".jpg");
                if (File.Exists(filePath))
                {
                    return true;
                }
            }
            else
            {
                filePath = string.Empty;
            }

            return false;
        }

        #endregion Methods

        #region Nested Types

        public sealed class ActionPump
        {
            #region Fields

            private static readonly ActionPump _instance = new ActionPump();

            private object _lock = new object();
            private ConcurrentQueue<Action> _queue;
            private Thread _worker;

            #endregion Fields

            #region Constructors

            public ActionPump()
            {
                _queue = new ConcurrentQueue<Action>();
                _worker = new Thread(Run);
                _worker.IsBackground = true;
                _worker.Start();
            }

            #endregion Constructors

            #region Properties

            public static ActionPump Instance
            {
                get
                {
                    return _instance;
                }
            }

            #endregion Properties

            #region Methods

            public void Enqueue(Action action)
            {
                action();
                //_queue.Enqueue(action);

                //lock (_lock)
                //{
                //    Monitor.Pulse(_lock);
                //}
            }

            private void Run()
            {
                while (true)
                {
                    Action action;

                    if (_queue.TryDequeue(out action))
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                    else
                    {
                        lock (_lock)
                        {
                            Monitor.Wait(_lock);
                        }
                    }

                }
            }

            #endregion Methods
        }

        public sealed class DiskImageCacheEntry : NotificationObject, IImageCacheEntry
        {
            #region Fields

            private readonly int _decodeHeight;
            private readonly int _decodeWidth;

            private Action _action;
            private BitmapSource _bitmap;
            private string _id;
            private bool _isLoaded;
            private Lazy<Action> _lazyLoad;
            private string _path;

            #endregion Fields

            #region Constructors

            public DiskImageCacheEntry(string id, string path, int decodeWidth = 300, int decodeHeight = 300)
            {
                _id = id;
                _path = path;
                _decodeWidth = decodeWidth;
                _decodeHeight = decodeHeight;
                _action = new Action(LoadCoverArt);
                _lazyLoad = new Lazy<Action>(() => () => ActionPump.Instance.Enqueue(_action), LazyThreadSafetyMode.PublicationOnly);
            }

            #endregion Constructors

            #region Properties

            public BitmapSource Bitmap
            {
                get
                {
                    if (_bitmap == null)
                    {
                        _lazyLoad.Value();
                    }

                    return _bitmap;
                }
            }

            public string ID
            {
                get { return _id; }
            }

            public bool IsLoaded
            {
                get
                {
                    return _isLoaded;
                }
                private set
                {
                    _isLoaded = value;
                    RaisePropertyChanged("IsLoaded");
                }
            }

            public string Path
            {
                get { return _path; }
            }

            #endregion Properties

            #region Methods

            public static BitmapImage GetCoverArtSource(string absolutePath, int decodeHeight, int decodeWidth)
            {
                var coverArtSource = new BitmapImage();
                coverArtSource.BeginInit();
                coverArtSource.DecodePixelHeight = decodeHeight;
                coverArtSource.DecodePixelWidth = decodeWidth;
                coverArtSource.CacheOption = BitmapCacheOption.None;
                coverArtSource.UriSource = new Uri(absolutePath, UriKind.Absolute);
                coverArtSource.EndInit();
                coverArtSource.Freeze();
                return coverArtSource;
            }

            public override bool Equals(object obj)
            {
                IImageCacheEntry cacheCacheEntry = obj as IImageCacheEntry;
                if (cacheCacheEntry != null)
                {
                    return cacheCacheEntry.ID.Equals(ID);
                }
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return ID.GetHashCode() ^ Path.GetHashCode();
            }

            public void SetBitmap(BitmapSource bitmapSource)
            {
                _bitmap = bitmapSource;
                RaisePropertyChanged("Bitmap");
            }

            private void LoadCoverArt()
            {
                BitmapImage coverArtSource = GetCoverArtSource(_path, _decodeHeight, _decodeWidth);
                SetBitmap(coverArtSource);
                IsLoaded = true;
            }

            #endregion Methods
        }

        public sealed class FirstTimeImageCacheEntry : NotificationObject, IImageCacheEntry
        {
            #region Fields

            private readonly int _decodeHeight;
            private readonly int _decodeWidth;
            private readonly IImage _image;
            private readonly string _saveLocation;

            private BitmapSource _bitmap;
            private string _id;
            private bool _isLoaded;

            #endregion Fields

            #region Constructors

            public FirstTimeImageCacheEntry(IImage image, string saveLocation, int decodeWidth = 300, int decodeHeight = 300)
            {
                _image = image;
                _saveLocation = saveLocation;
                _decodeWidth = decodeWidth;
                _decodeHeight = decodeHeight;

                _image.FinishedLoading += OnFinishedLoading;
                _image.Load();

                _id = image.ID;
            }

            #endregion Constructors

            #region Properties

            public BitmapSource Bitmap
            {
                get { return _bitmap; }
            }

            public string ID
            {
                get { return _id; }
            }

            public bool IsLoaded
            {
                get
                {
                    return _isLoaded;
                }
                private set
                {
                    _isLoaded = value;
                    RaisePropertyChanged("IsLoaded");
                }
            }

            #endregion Properties

            #region Methods

            public void SetBitmap(BitmapSource bitmapSource)
            {
                _bitmap = bitmapSource;
                RaisePropertyChanged("Bitmap");
            }

            private void OnFinishedLoading(object sender, EventArgs e)
            {
                ((IImage)sender).FinishedLoading -= OnFinishedLoading;
                ActionPump.Instance.Enqueue(PrepareImage);
            }

            private void PrepareImage()
            {
                bool success = false;
                try
                {
                    if (!File.Exists(_saveLocation))
                    {
                        using (MemoryStream stream = new MemoryStream(_image.Raw))
                        {
                            JpegBitmapEncoder bitmapEncoder = new JpegBitmapEncoder();
                            bitmapEncoder.Frames.Add(BitmapFrame.Create(stream));
                            bitmapEncoder.QualityLevel = 100;

                            using (FileStream fs = new FileStream(_saveLocation, FileMode.Create, FileAccess.Write))
                            {
                                bitmapEncoder.Save(fs);
                            }

                            success = true;
                        }
                    }
                    else
                    {
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                if (success)
                {
                    var bitmapImage = DiskImageCacheEntry.GetCoverArtSource(_saveLocation, _decodeWidth, _decodeHeight);
                    SetBitmap(bitmapImage);
                }

                IsLoaded = true;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}