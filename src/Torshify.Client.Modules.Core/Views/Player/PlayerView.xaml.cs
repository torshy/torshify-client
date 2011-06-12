using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.Player
{
    public partial class PlayerView : UserControl
    {
        #region Constructors

        public PlayerView(PlayerViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlayerViewModel Model
        {
            get { return DataContext as PlayerViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}