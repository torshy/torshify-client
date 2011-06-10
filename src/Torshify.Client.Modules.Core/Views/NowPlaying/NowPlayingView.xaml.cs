using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public partial class NowPlayingView : UserControl
    {
        #region Constructors

        public NowPlayingView(NowPlayingViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public NowPlayingViewModel Model
        {
            get { return DataContext as NowPlayingViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}