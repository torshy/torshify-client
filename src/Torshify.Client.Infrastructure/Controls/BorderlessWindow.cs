using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Torshify.Client.Infrastructure.Controls
{
    [TemplatePart(Name = "PART_MinimizeButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_MaximizeButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(ButtonBase))]
    public class BorderlessWindow : Window
    {
        #region Fields

        private WindowState _lastWindowState;
        private Button _partCloseButton;
        private Button _partMaximizeButton;
        private Button _partMinimizeButton;

        #endregion Fields

        #region Constructors

        static BorderlessWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderlessWindow), new FrameworkPropertyMetadata(typeof(BorderlessWindow)));
        }

        #endregion Constructors

        #region Methods

        public override void OnApplyTemplate()
        {
            // Close Button
            Button oldCloseButton = _partCloseButton;
            _partCloseButton = Template.FindName("PART_CloseButton", this) as Button;

            if (!ReferenceEquals(oldCloseButton, _partCloseButton))
            {
                if (oldCloseButton != null)
                {
                    oldCloseButton.Click -= OnCloseButtonClick;
                }

                if (_partCloseButton != null)
                {
                    _partCloseButton.Click += OnCloseButtonClick;
                }
            }

            // Minimize Button
            Button oldMinimizeButton = _partMinimizeButton;
            _partMinimizeButton = Template.FindName("PART_MinimizeButton", this) as Button;

            if (!ReferenceEquals(oldMinimizeButton, _partMinimizeButton))
            {
                if (oldMinimizeButton != null)
                {
                    oldMinimizeButton.Click -= OnMinimizeButtonClick;
                }

                if (_partMinimizeButton != null)
                {
                    _partMinimizeButton.Click += OnMinimizeButtonClick;
                }
            }

            // Maximize Button
            Button oldMaximizeButton = _partMaximizeButton;
            _partMaximizeButton = Template.FindName("PART_MaximizeButton", this) as Button;

            if (!ReferenceEquals(oldMaximizeButton, _partMaximizeButton))
            {
                if (oldMaximizeButton != null)
                {
                    oldMaximizeButton.Click -= OnMaximizeButtonClick;
                }

                if (_partMaximizeButton != null)
                {
                    _partMaximizeButton.Click += OnMaximizeButtonClick;
                }
            }
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = _lastWindowState;
            }
            else
            {
                _lastWindowState = WindowState;
                WindowState = WindowState.Maximized;
            }
        }

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion Methods
    }
}