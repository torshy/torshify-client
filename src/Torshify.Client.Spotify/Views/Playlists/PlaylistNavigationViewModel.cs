using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;
using Torshify.Client.Spotify.Services;
using System.Linq;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class PlaylistNavigationViewModel : NavigationViewModelBase<PlaylistNavigationItem>
    {
        #region Fields

        private readonly IPlayerController _playerController;
        private readonly Dispatcher _dispatcher;
        private readonly IPlaylistProvider _playlistProvider;

        #endregion Fields

        #region Constructors

        public PlaylistNavigationViewModel(
            IPlaylistProvider playlistProvider,
            IRegionManager regionManager,
            IPlayerController playerController,
            Dispatcher dispatcher)
            : base(regionManager)
        {
            _playlistProvider = playlistProvider;
            _playerController = playerController;
            _dispatcher = dispatcher;
            _playerController.Playlist.CurrentChanged += OnCurrentSongChanged;
            _playlistProvider.PlaylistAdded += OnPlaylistAdded;
            _playlistProvider.PlaylistMoved += OnPlaylistMoved;
            _playlistProvider.PlaylistRemoved += OnPlaylistRemoved;
            Playlists = new ListCollectionView(NavigationItems);
            InitializeNavigationItems();
            MoveItemCommand = new AutomaticCommand<Tuple<int, int>>(ExecuteMoveItem, CanExecuteMoveItem);
            RemoveItemCommand = new AutomaticCommand<PlaylistNavigationItem>(ExecuteRemoveItem, CanExecuteRemoveItem);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand<Tuple<int, int>> MoveItemCommand
        {
            get;
            private set;
        }

        public ICollectionView Playlists
        {
            get;
            private set;
        }

        public AutomaticCommand<PlaylistNavigationItem> RemoveItemCommand
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        protected override bool CanNavigateToItem(PlaylistNavigationItem item)
        {
            if (item is PlaylistSeparatorNavigationItem || item is FolderPlaylistNavigationItem)
            {
                return false;
            }

            return true;
        }

        protected override object GetNavigationContextTag(PlaylistNavigationItem item)
        {
            return item.Playlist;
        }

        protected override bool IsFromThisItem(IRegionNavigationJournalEntry entry, PlaylistNavigationItem item)
        {
            var parts = entry.Uri.OriginalString.Split('?');
            return item.Playlist == entry.Tag && parts[0] == item.NavigationUrl.OriginalString;
        }

        private bool CanExecuteMoveItem(Tuple<int, int> item)
        {
            return true;
        }

        private bool CanExecuteRemoveItem(PlaylistNavigationItem item)
        {
            return true;
        }

        private void ExecuteMoveItem(Tuple<int, int> item)
        {
            _playlistProvider.Move(item.Item1, item.Item2);
        }

        private void ExecuteRemoveItem(PlaylistNavigationItem item)
        {
            var index = _playlistProvider.Playlists.IndexOf(item.Playlist);

            if (index != -1)
            {
                _playlistProvider.Remove(index);
            }
        }

        private void InitializeNavigationItems()
        {
            NavigationItems.Clear();

            ObservableCollection<PlaylistNavigationItem> currentList = NavigationItems;

            int index = 0;

            foreach (var p in _playlistProvider.Playlists)
            {
                Playlist playlist = p as Playlist;

                if (playlist == null)
                    continue;

                var containerPlaylist = playlist.InternalPlaylist as IContainerPlaylist;

                if (containerPlaylist == null)
                    continue;

                switch (containerPlaylist.Type)
                {
                    case PlaylistType.Playlist:
                        currentList.Add(containerPlaylist.Name == "-"
                                            ? new PlaylistSeparatorNavigationItem(p)
                                            : new PlaylistNavigationItem(p));
                        break;
                    case PlaylistType.StartFolder:
                        var folder = new FolderPlaylistNavigationItem(p);
                        var children = new ObservableCollection<PlaylistNavigationItem>();
                        folder.Children = children;
                        currentList.Add(folder);
                        currentList = children;
                        break;
                    case PlaylistType.EndFolder:
                        currentList = NavigationItems;
                        break;
                    case PlaylistType.Placeholder:
                        currentList.Add(new UnknownPlaylistNavigationItem(p));
                        break;
                }

                index++;
            }
        }

        private void OnCurrentSongChanged(object sender, EventArgs e)
        {
            var current = _playerController.Playlist.Current;

            if (current != null)
            {
                var playlistTrack = current.Track as Infrastructure.Interfaces.IPlaylistTrack;

                ForEach(NavigationItems, item =>
                                             {
                                                 if (playlistTrack != null)
                                                 {
                                                     item.HasTrackPlaying = item.Playlist == playlistTrack.Playlist;
                                                 }
                                                 else
                                                 {
                                                     item.HasTrackPlaying = false;
                                                 }
                                             });
            }
        }

        private void OnPlaylistAdded(object sender, Infrastructure.Interfaces.PlaylistEventArgs e)
        {
            _dispatcher.BeginInvoke((Action)InitializeNavigationItems);
        }

        private void OnPlaylistMoved(object sender, Infrastructure.Interfaces.PlaylistMovedEventArgs e)
        {
            _dispatcher.BeginInvoke((Action)InitializeNavigationItems);
        }

        private void OnPlaylistRemoved(object sender, Infrastructure.Interfaces.PlaylistEventArgs e)
        {
            _dispatcher.BeginInvoke((Action)InitializeNavigationItems);
        }

        private void ForEach(IEnumerable<PlaylistNavigationItem> list, Action<PlaylistNavigationItem> action)
        {
            foreach (var navigationItem in list)
            {
                action(navigationItem);

                if (navigationItem is FolderPlaylistNavigationItem)
                {
                    ForEach(((FolderPlaylistNavigationItem)navigationItem).Children, action);
                }
            }
        }

        #endregion Methods
    }
}