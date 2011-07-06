using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class KeyboardBehavior : Behavior<UIElement>
    {
        #region Fields

        public static readonly DependencyProperty CommandParameterProperty = 
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(KeyboardBehavior),
                new FrameworkPropertyMetadata((object)null));
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register("Command", typeof(ICommand), typeof(KeyboardBehavior),
                new FrameworkPropertyMetadata((ICommand)null));
        public static readonly DependencyProperty KeyProperty = 
            DependencyProperty.Register("Key", typeof(Key), typeof(KeyboardBehavior),
                new FrameworkPropertyMetadata(Key.None));
        public static readonly DependencyProperty ModifierProperty = 
            DependencyProperty.Register("Modifier", typeof(ModifierKeys), typeof(KeyboardBehavior),
                new FrameworkPropertyMetadata(ModifierKeys.None));

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

        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        public ModifierKeys Modifier
        {
            get { return (ModifierKeys)GetValue(ModifierProperty); }
            set { SetValue(ModifierProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnKeyDown), true);
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnKeyDown));
            base.OnDetaching();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key && Keyboard.Modifiers == Modifier)
            {
                var element = e.OriginalSource as DependencyObject;

                if (element != null)
                {
                    if (Command != null && Command.CanExecute(CommandParameter))
                    {
                        Command.Execute(CommandParameter);
                        e.Handled = true;
                    }
                }
            }
        }

        #endregion Methods
    }
}