using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.Playlists
{
    public partial class PlaylistsView : UserControl
    {
        #region Constructors

        public PlaylistsView(PlaylistsViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlaylistsViewModel Model
        {
            get { return DataContext as PlaylistsViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}