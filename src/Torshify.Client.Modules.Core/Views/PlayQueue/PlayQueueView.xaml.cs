using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.PlayQueue
{
    public partial class PlayQueueView : UserControl
    {
        #region Constructors

        public PlayQueueView(PlayQueueViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlayQueueViewModel Model
        {
            get { return DataContext as PlayQueueViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}