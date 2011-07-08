using System.Windows.Controls;

namespace Torshify.Client.Modules.Core.Views.Notifications
{
    public partial class NotificationsView : UserControl
    {
        #region Constructors

        public NotificationsView(NotificationsViewModel viewModel)
        {
            InitializeComponent();

            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public NotificationsViewModel Model
        {
            get { return DataContext as NotificationsViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }
}