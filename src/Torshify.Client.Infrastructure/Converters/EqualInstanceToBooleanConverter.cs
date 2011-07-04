using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Torshify.Client.Infrastructure.Converters
{
    public class EqualInstanceToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return true;

            var valueToCompare = values.First();

            return values.All(i => i == valueToCompare);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}