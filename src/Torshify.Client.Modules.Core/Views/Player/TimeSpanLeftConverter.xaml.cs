using System;
using System.Globalization;
using System.Windows.Data;
using Torshify.Client.Infrastructure.Converters;

namespace Torshify.Client.Modules.Core.Views.Player
{
    public class TimeSpanLeftConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is TimeSpan && values[1] is TimeSpan)
            {
                TimeSpan t1 = (TimeSpan) values[0];
                TimeSpan t2 = (TimeSpan) values[1];

                return "-" + new TimeSpanToTextConverter().Convert(t1.Subtract(t2), targetType, parameter, culture);
            }

            return new TimeSpanToTextConverter().Convert(TimeSpan.Zero, targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}