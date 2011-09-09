using System.Windows;
using System.Windows.Controls;

using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.EchoNest.Views
{
    public partial class EchoNestNavigationView : UserControl
    {
        #region Constructors

        public EchoNestNavigationView(EchoNestNavigationViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public EchoNestNavigationViewModel Model
        {
            get { return DataContext as EchoNestNavigationViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Model.SelectedItemChanged((NavigationItem)e.OldValue, (NavigationItem)e.NewValue);
        }

        #endregion Methods
    }
}