using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views
{
    public partial class MainView : UserControl
    {
        #region Constructors

        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public MainViewModel Model
        {
            get { return DataContext as MainViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}