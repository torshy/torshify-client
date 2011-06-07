using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.Playlist
{
    public partial class PlaylistView : UserControl
    {
        #region Constructors

        public PlaylistView(PlaylistViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlaylistViewModel Model
        {
            get { return DataContext as PlaylistViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}