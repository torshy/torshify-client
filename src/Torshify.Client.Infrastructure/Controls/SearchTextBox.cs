using System.Windows;
using System.Windows.Input;

namespace Torshify.Client.Infrastructure.Controls
{
    public class SearchTextBox : WatermarkTextBox
    {
        #region Fields

        public static readonly DependencyProperty SearchCommandParameterProperty = 
            DependencyProperty.Register("SearchCommandParameter", typeof(object), typeof(SearchTextBox),
                new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty SearchCommandProperty = 
            DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchTextBox),
                new FrameworkPropertyMetadata((ICommand)null));

        #endregion Fields

        #region Constructors

        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        #endregion Constructors

        #region Properties

        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        public object SearchCommandParameter
        {
            get { return (object)GetValue(SearchCommandParameterProperty); }
            set { SetValue(SearchCommandParameterProperty, value); }
        }

        #endregion Properties
    }
}