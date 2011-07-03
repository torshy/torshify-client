using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.WhatsNew
{
    public partial class WhatsNewView : UserControl
    {
        #region Constructors

        public WhatsNewView(WhatsNewViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public WhatsNewViewModel Model
        {
            get { return DataContext as WhatsNewViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}