using System;
using System.Globalization;
using System.Windows.Data;

namespace Torshify.Client.Infrastructure.Converters
{
    public class TimeSpanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is TimeSpan)
            {
                TimeSpan t = (TimeSpan) value;

                if (t.Hours > 0)
                {
                    return string.Format("{0}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
                }

                return string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}