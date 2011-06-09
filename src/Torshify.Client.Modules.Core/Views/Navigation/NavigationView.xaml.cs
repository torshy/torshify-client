using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public partial class NavigationView : UserControl
    {
        #region Constructors

        public NavigationView(NavigationViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public NavigationViewModel Model
        {
            get { return DataContext as NavigationViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}