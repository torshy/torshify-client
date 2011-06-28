using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Torshify.Client.Infrastructure.Converters;

namespace Torshify.Client.Modules.Core.Views.Player
{
    public partial class PlayerView : UserControl
    {
        #region Constructors

        public PlayerView(PlayerViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlayerViewModel Model
        {
            get { return DataContext as PlayerViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }

    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan) value).TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TimeSpan.FromSeconds(System.Convert.ToDouble(value));
        }
    }

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