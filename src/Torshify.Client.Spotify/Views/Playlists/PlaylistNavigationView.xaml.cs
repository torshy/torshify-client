using System.Windows;
using System.Windows.Controls;

using Microsoft.Practices.Prism.Regions;

namespace Torshify.Client.Spotify.Views.Playlists
{
    [ViewSortHint("2")]
    public partial class PlaylistNavigationView : UserControl
    {
        #region Constructors

        public PlaylistNavigationView(PlaylistNavigationViewModel viewModel)
        {
            Model = viewModel;
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public PlaylistNavigationViewModel Model
        {
            get
            {
                return DataContext as PlaylistNavigationViewModel;
            }
            private set
            {
                DataContext = value;
            }
        }

        #endregion Properties

        #region Methods

        private void TreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var oldValue = e.OldValue as PlaylistNavigationItem;
            var newValue = e.NewValue as PlaylistNavigationItem;
            
            Model.SelectedItemChanged(oldValue, newValue);
        }

        #endregion Methods
    }
}