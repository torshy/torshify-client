using System;
using System.Windows;
using System.Windows.Controls;

using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    public partial class DefaultNavigationView : UserControl
    {
        #region Constructors

        public DefaultNavigationView(DefaultNavigationViewModel viewModel)
        {
            InitializeComponent();

            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public DefaultNavigationViewModel Model
        {
            get { return DataContext as DefaultNavigationViewModel; }
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