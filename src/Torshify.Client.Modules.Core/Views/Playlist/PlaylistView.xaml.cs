using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.Practices.Prism.Events;

using Torshify.Client.Infrastructure.Controls;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.Playlist
{
    public partial class PlaylistView : UserControl
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion Fields

        #region Constructors

        public PlaylistView(PlaylistViewModel viewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlaylistViewModel Model
        {
            get { return DataContext as PlaylistViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void DataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            var dg = (MultiSelector)sender;

            if (element != null && element.DataContext is ITrack)
            {
                var commandbar = new CommandBar();

                if (dg.SelectedItems.Count == 1)
                {
                    var track = (ITrack)dg.SelectedItems[0];
                    _eventAggregator.GetEvent<TrackCommandBarEvent>().Publish(new TrackCommandBarModel(track, commandbar));
                }
                else if (dg.SelectedItems.Count > 1)
                {
                    var tracks = dg.SelectedItems.Cast<ITrack>();
                    _eventAggregator.GetEvent<TracksCommandBarEvent>().Publish(new TracksCommandBarModel(tracks, commandbar));
                }

                dg.ContextMenu = new CommandBarContextMenu
                {
                    ItemsSource = commandbar.ChildMenuItems
                };
            }
        }

        #endregion Methods
    }
}