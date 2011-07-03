using System.Windows.Controls;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.WhatsNew.Tabs
{
    public partial class TopListsTabItemView : UserControl, ITab<object>
    {
        #region Constructors

        public TopListsTabItemView(TopListsTabItemViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        #endregion Constructors

        #region Properties

        public ITabViewModel<object> ViewModel
        {
            get { return DataContext as TopListsTabItemViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}