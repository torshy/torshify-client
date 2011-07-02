using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Microsoft.Practices.Prism.Events;

using Torshify.Client.Infrastructure.Controls;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.Artist.Tabs
{
    public partial class OverviewTabItemView : UserControl, ITab<IArtist>
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion Fields

        #region Constructors

        public OverviewTabItemView(OverviewTabItemViewModel viewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            ViewModel = viewModel;
        }

        #endregion Constructors

        #region Properties

        public ITabViewModel<IArtist> ViewModel
        {
            get { return DataContext as OverviewTabItemViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void DataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var datagridSource = (MultiSelector)sender;

            List<ITrack> selectedItems = new List<ITrack>();

            for (int i = 0; i < _albumsListControl.Items.Count; i++)
            {
                var dgContainer = _albumsListControl.ItemContainerGenerator.ContainerFromIndex(i);

                if (dgContainer != null)
                {
                    var dg = dgContainer.FindVisualDescendantByType<DataGrid>();

                    if (dg.SelectedItems.Count == 1)
                    {
                        var track = (ITrack)dg.SelectedItems[0];
                        selectedItems.Add(track);
                    }
                    else if (dg.SelectedItems.Count > 1)
                    {
                        var tracks = dg.SelectedItems.Cast<ITrack>();
                        selectedItems.AddRange(tracks);
                    }
                }
            }

            var commandbar = new CommandBar();

            if (selectedItems.Count == 1)
            {
                var track = selectedItems[0];
                _eventAggregator.GetEvent<TrackCommandBarEvent>().Publish(new TrackCommandBarModel(track, commandbar));
            }
            else if (selectedItems.Count > 1)
            {
                _eventAggregator.GetEvent<TracksCommandBarEvent>().Publish(new TracksCommandBarModel(selectedItems, commandbar));
            }

            datagridSource.ContextMenu = new CommandBarContextMenu
            {
                ItemsSource = commandbar.ChildMenuItems
            };
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

        #endregion Methods
    }
}