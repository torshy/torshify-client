using System.Windows.Controls;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.WhatsNew.Tabs
{
    public partial class WhatsNewTabItemView : UserControl, ITab<object>
    {
        #region Constructors

        public WhatsNewTabItemView(WhatsNewTabItemViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        #endregion Constructors

        #region Properties

        public ITabViewModel<object> ViewModel
        {
            get { return DataContext as WhatsNewTabItemViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}
