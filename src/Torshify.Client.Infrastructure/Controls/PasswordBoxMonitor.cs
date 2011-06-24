namespace Torshify.Client.Infrastructure.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class PasswordBoxMonitor : DependencyObject
    {
        #region Fields

        public static readonly DependencyProperty HasPasswordProperty = 
            DependencyProperty.RegisterAttached("HasPassword", typeof(bool), typeof(PasswordBoxMonitor),
                new FrameworkPropertyMetadata((bool)false));
        public static readonly DependencyProperty IsMonitoringProperty = 
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));
        public static readonly DependencyProperty PasswordLengthProperty = 
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor), new UIPropertyMetadata(0));

        #endregion Fields

        #region Methods

        public static bool GetHasPassword(DependencyObject d)
        {
            return (bool)d.GetValue(HasPasswordProperty);
        }

        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }

        public static void SetHasPassword(DependencyObject d, bool value)
        {
            d.SetValue(HasPasswordProperty, value);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as PasswordBox;
            if (pb == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += PasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= PasswordChanged;
            }
        }

        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as PasswordBox;
            if (pb == null)
            {
                return;
            }

            if (pb.Password.Length == 0)
            {
                SetHasPassword(pb, false);
            }
            else
            {
                SetHasPassword(pb, true);
            }

            SetPasswordLength(pb, pb.Password.Length);
        }

        #endregion Methods
    }
}