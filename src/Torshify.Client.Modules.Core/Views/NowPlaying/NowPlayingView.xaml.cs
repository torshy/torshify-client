using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Torshify.Client.Infrastructure.Input;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public partial class NowPlayingView : UserControl
    {
        #region Constructors

        public NowPlayingView(NowPlayingViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
            InputBindings.Add(
                new ExtendedMouseBinding
                {
                    Command = viewModel.NavigateBackCommand,
                    Gesture = new ExtendedMouseGesture(MouseButton.XButton1)
                });
        }

        #endregion Constructors

        #region Properties

        public NowPlayingViewModel Model
        {
            get { return DataContext as NowPlayingViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        private void OnRequestSeek(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Model.RequestSeek = e.NewValue;
        }
    }
}