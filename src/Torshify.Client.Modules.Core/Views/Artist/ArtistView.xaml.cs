using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Torshify.Client.Infrastructure.Controls;

namespace Torshify.Client.Modules.Core.Views.Artist
{
    public partial class ArtistView : UserControl
    {
        #region Constructors

        public ArtistView(ArtistViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public ArtistViewModel Model
        {
            get { return DataContext as ArtistViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        private void DataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            
        }

        private void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _albumsListControl.Items.Count; i++)
            {
                var dgContainer = _albumsListControl.ItemContainerGenerator.ContainerFromIndex(i);

                if (dgContainer != null)
                {
                    var dg = dgContainer.FindVisualDescendantByType<DataGrid>();

                    if (dg != e.Source)
                    {
                        if (Keyboard.Modifiers != ModifierKeys.Control)
                        {
                            dg.SelectedItem = null;
                        }
                    }
                }
            }
            
        }
    }
}