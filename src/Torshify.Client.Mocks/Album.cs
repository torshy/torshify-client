using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Mocks
{
    public class Album : IAlbum
    {
        #region Properties

        public IArtist Artist
        {
            get;
            set;
        }

        public bool IsAvailable
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }

        public BitmapSource Cover
        {
            get
            {
                var coverArtSource = new BitmapImage();
                coverArtSource.BeginInit();
                coverArtSource.CacheOption = BitmapCacheOption.None;
                coverArtSource.UriSource = new Uri("pack://application:,,,/Torshify.Client.Mocks;component/UnknownCoverArt.png");
                coverArtSource.EndInit();
                coverArtSource.Freeze();
                return coverArtSource;
            }
        }

        public IImage CoverArt
        {
            get; 
            set;
        }

        public IAlbumInformation Info
        {
            get; 
            set;
        }

        public AlbumType Type
        {
            get; 
            set;
        }

        #endregion Properties
    }
}