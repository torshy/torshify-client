using System.Windows;

namespace Torshify.Client.Infrastructure.Controls
{
    public class BorderlessWindow : Window
    {
        static BorderlessWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderlessWindow), new FrameworkPropertyMetadata(typeof(BorderlessWindow)));
        }

        public BorderlessWindow()
        {
        }
    }
}