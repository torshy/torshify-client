using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Torshify.Client.Infrastructure.Controls;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class DataGridRowActionBehavior : Behavior<DataGrid>
    {
        #region Fields

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(DataGridRowActionBehavior),
                new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DataGridRowActionBehavior),
                new FrameworkPropertyMetadata((ICommand)null));

        #endregion Fields

        #region Properties

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.MouseDoubleClick += OnMouseDoubleClick;
            AssociatedObject.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseDoubleClick -= OnMouseDoubleClick;
            AssociatedObject.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnKeyDown));
            base.OnDetaching();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var element = e.OriginalSource as DependencyObject;

                if (element != null)
                {
                    var row = element.FindVisualAncestorByType<DataGridRow>();

                    if (row != null && Command != null && Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                        e.Handled = true;
                    }
                }
            }
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = e.OriginalSource as DependencyObject;

            if (element != null && e.ChangedButton == MouseButton.Left)
            {
                var row = element.FindVisualAncestorByType<DataGridRow>();

                if (row != null && Command != null && Command.CanExecute(CommandParameter))
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        #endregion Methods
    }
}