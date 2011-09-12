using System;

using Microsoft.Practices.Prism.ViewModel;

using ITorshifyImage = Torshify.Client.Infrastructure.Interfaces.IImage;

namespace Torshify.Client.Spotify.Services
{
    public class Image : NotificationObject, ITorshifyImage
    {
        #region Fields

        private readonly string _imageId;
        private readonly ISession _session;

        private Lazy<byte[]> _data;
        private WeakReference _imageRef;
        private bool _isLoaded;
        private IImage _image;

        #endregion Fields

        #region Constructors

        public Image(ISession session, string imageId)
        {
            _session = session;
            _imageId = imageId;
        }

        #endregion Constructors

        #region Events

        public event EventHandler FinishedLoading;

        #endregion Events

        #region Properties

        public string ID
        {
            get
            {
                return _imageId;
            }
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

        public byte[] Raw
        {
            get
            {
                if (IsLoaded)
                {
                    return _data.Value;
                }

                Load();

                return new byte[0];
            }
        }

        #endregion Properties

        #region Methods

        public void Load()
        {
            try
            {
                _image = _session.GetImage(ID);
                _imageRef = new WeakReference(_image);
                _data = new Lazy<byte[]>(() =>
                {
                    IImage img = _imageRef.Target as IImage;
                    return img != null ? img.Data : new byte[0];
                });

                IsLoaded = _image.IsLoaded;
                _image.Loaded += OnImageLoaded;

                if (_image.IsLoaded)
                {
                    _image.Loaded -= OnImageLoaded;
                    RaisePropertyChanged("Raw");
                    RaiseFinishedLoading();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                IsLoaded = false;
            }
        }

        private void OnImageLoaded(object sender, EventArgs e)
        {
            IImage image = (IImage)sender;
            image.Loaded -= OnImageLoaded;
            
            // Just access the data property as it lazy loaded.
            image.Data.Length.ToString();
            IsLoaded = true;
            RaisePropertyChanged("Raw");
            RaiseFinishedLoading();
        }

        private void RaiseFinishedLoading()
        {
            var handler = FinishedLoading;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Methods
    }
}