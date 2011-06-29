using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.Album
{
    public partial class AlbumView : UserControl
    {
        #region Constructors

        public AlbumView(AlbumViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public AlbumViewModel Model
        {
            get { return DataContext as AlbumViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}