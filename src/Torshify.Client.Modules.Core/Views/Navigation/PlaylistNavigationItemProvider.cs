using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Navigation
{
    [Export(typeof(INavigationItemProvider))]
    public class PlaylistNavigationItemProvider : INavigationItemProvider
    {
        #region Fields

        private ObservableCollection<PlaylistNavigationItem> _items = new ObservableCollection<PlaylistNavigationItem>();

        #endregion Fields

        #region Constructors

        public PlaylistNavigationItemProvider()
        {
            RegionManager = (IRegionManager)ServiceLocator.Current.GetInstance(typeof(IRegionManager));
            Dispatcher = (Dispatcher) ServiceLocator.Current.GetInstance(typeof (Dispatcher));
            PlaylistProvider = (IPlaylistProvider)ServiceLocator.Current.GetInstance(typeof (IPlaylistProvider));
            PlaylistProvider.PlaylistAdded += OnPlaylistAdded;
            PlaylistProvider.PlaylistMoved += OnPlaylistMoved;
            PlaylistProvider.PlaylistRemoved += OnPlaylistRemoved;

            foreach (var playlist in PlaylistProvider.Playlists)
            {
                _items.Add(new PlaylistNavigationItem(playlist, RegionManager));
            }
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<INavigationItem> Items
        {
            get { return _items; }
        }

        protected Dispatcher Dispatcher
        {
            get;
            private set;
        }

        protected IRegionManager RegionManager
        {
            get;
            private set;
        }

        protected IPlaylistProvider PlaylistProvider
        {
            get;
            private set;
        }

        #endregion Properties

        #region Private Methods

        private void OnPlaylistRemoved(object sender, PlaylistEventArgs e)
        {
            if (Dispatcher.CheckAccess())
            {
                var itemToRemove = _items.FirstOrDefault(i => i.Playlist == e.Playlist);

                if (itemToRemove != null)
                {
                    _items.Remove(itemToRemove);
                }
            }
            else
            {
                Dispatcher.BeginInvoke(new Action<object, PlaylistEventArgs>(OnPlaylistRemoved), sender, e);
            }
        }

        private void OnPlaylistMoved(object sender, PlaylistMovedEventArgs e)
        {
            if (Dispatcher.CheckAccess())
            {
                _items.Move(e.OldIndex, e.NewIndex);
            }
            else
            {
                Dispatcher.BeginInvoke(new Action<object, PlaylistMovedEventArgs>(OnPlaylistMoved), sender, e);
            }
        }

        private void OnPlaylistAdded(object sender, PlaylistEventArgs e)
        {
            if (Dispatcher.CheckAccess())
            {
                _items.Insert(e.Position, new PlaylistNavigationItem(e.Playlist, RegionManager));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action<object, PlaylistEventArgs>(OnPlaylistAdded), sender, e);
            }
        }

        #endregion Private Methods
    }
}