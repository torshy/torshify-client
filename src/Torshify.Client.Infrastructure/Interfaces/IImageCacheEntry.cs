using System.Windows.Media.Imaging;

namespace Torshify.Client.Infrastructure.Interfaces
{
    public interface IImageCacheEntry
    {
        #region Properties

        BitmapSource Bitmap
        {
            get;
        }

        string ID
        {
            get;
        }

        bool IsLoaded
        {
            get;
        }

        #endregion Properties
    }
}