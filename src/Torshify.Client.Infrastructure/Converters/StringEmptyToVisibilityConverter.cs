using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Torshify.Client.Infrastructure.Converters
{
    [Localizability(LocalizationCategory.NeverLocalize)]
    public class StringEmptyToVisibilityConverter : IValueConverter
    {
        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string;
            return string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }

        #endregion Public Methods
    }
}