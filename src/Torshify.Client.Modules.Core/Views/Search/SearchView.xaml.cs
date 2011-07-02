using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.Search
{
    public partial class SearchView : UserControl
    {
        #region Constructors

        public SearchView(SearchViewModel viewModel)
        {
            InitializeComponent();

            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public SearchViewModel Model
        {
            get { return DataContext as SearchViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}