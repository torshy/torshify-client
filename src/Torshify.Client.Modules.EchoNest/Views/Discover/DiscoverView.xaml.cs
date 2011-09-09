using System.Windows.Controls;

namespace Torshify.Client.Modules.EchoNest.Views.Discover
{
    public partial class DiscoverView : UserControl
    {
        #region Constructors

        public DiscoverView(DiscoverViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public DiscoverViewModel Model
        {
            get { return DataContext as DiscoverViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}