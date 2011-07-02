using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.Practices.Prism.Events;

using Torshify.Client.Infrastructure.Controls;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.PlayQueue
{
    public partial class PlayQueueView : UserControl
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        #endregion Fields

        #region Constructors

        public PlayQueueView(PlayQueueViewModel viewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            InitializeComponent();
            Model = viewModel;
        }

        #endregion Constructors

        #region Properties

        public PlayQueueViewModel Model
        {
            get { return DataContext as PlayQueueViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties

        #region Methods

        private void DataGridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            var dg = (MultiSelector)sender;

            if (element != null && element.DataContext is PlayerQueueItem)
            {
                var commandbar = new CommandBar();

                if (dg.SelectedItems.Count == 1)
                {
                    var track = (PlayerQueueItem)dg.SelectedItems[0];
                    _eventAggregator.GetEvent<TrackCommandBarEvent>().Publish(new TrackCommandBarModel(track.Track, commandbar));
                }
                else if (dg.SelectedItems.Count > 1)
                {
                    var tracks = dg.SelectedItems.Cast<PlayerQueueItem>().Select(q => q.Track);
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