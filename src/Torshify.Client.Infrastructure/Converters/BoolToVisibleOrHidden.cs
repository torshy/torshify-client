using System;
using System.Windows;
using System.Windows.Data;

namespace Torshify.Client.Infrastructure.Converters
{
    /// <summary>
    /// Thanks to http://www.rhyous.com/2011/02/22/binding-visibility-to-a-bool-value-in-wpf/
    /// </summary>
    public class BoolToVisibleOrHidden : IValueConverter
    {
        #region Properties

        public bool Collapse
        {
            get; set;
        }

        public bool Reverse
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bValue = (bool)value;

            if (bValue != Reverse)
            {
                return Visibility.Visible;
            }
            else
            {
                if (Collapse)
                    return Visibility.Collapsed;
                else
                    return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;

            if (visibility == Visibility.Visible)
                return !Reverse;
            else
                return Reverse;
        }

        #endregion Methods
    }
}