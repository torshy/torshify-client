using System;
using System.Globalization;
using System.Windows.Data;

using Microsoft.Practices.Prism;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure.Converters
{
    public class ImageCacheConverter : IValueConverter
    {
        #region Constructors

        public ImageCacheConverter()
        {
            DecodeHeight = 300;
            DecodeWidth = 300;
        }

        #endregion Constructors

        #region Properties

        public int DecodeHeight
        {
            get; set;
        }

        public int DecodeWidth
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IImage)
            {
                var imageCacheService = ServiceLocator.Current.TryResolve<IImageCacheService>();

                return imageCacheService.GetImage((IImage) value, DecodeWidth, DecodeHeight);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}