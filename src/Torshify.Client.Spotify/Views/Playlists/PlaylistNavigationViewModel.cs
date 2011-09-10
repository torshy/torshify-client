using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Threading;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;
using Torshify.Client.Spotify.Services;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class PlaylistNavigationViewModel : NavigationViewModelBase<PlaylistNavigationItem>
    {
        #region Fields

        private readonly Dispatcher _dispatcher;
        private readonly ILoggerFacade _logger;
        private readonly IPlayerController _playerController;
        private readonly IPlaylistProvider _playlistProvider;
        private readonly ISession _session;

        #endregion Fields

        #region Constructors

        public PlaylistNavigationViewModel(
            ISession session,
            IPlaylistProvider playlistProvider,
            IRegionManager regionManager,
            IPlayerController playerController,
            ILoggerFacade logger,
            Dispatcher dispatcher)
            : base(regionManager)
        {
            _session = session;
            _session.LoginComplete += OnSessionLoggedIn;
            _playlistProvider = playlistProvider;
            _playerController = playerController;
            _logger = logger;
            _dispatcher = dispatcher;
            _playerController.Playlist.CurrentChanged += OnCurrentSongChanged;
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

        protected override void UpdateIsSelected(IRegionNavigationJournalEntry entry)
        {
            ForEach(NavigationItems, item=>
                                         {
                                             item.IsSelected = IsFromThisItem(entry, item);
                                         });
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

        private void InitializeNavigationItems()
        {
            _logger.Log("InitializeNavigationItems", Category.Debug, Priority.Low); 

            try
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
            catch(Exception ex)
            {
                _logger.Log(ex.ToString(), Category.Exception, Priority.High);
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
            _logger.Log("OnPlaylistAdded", Category.Debug, Priority.Low);
            _dispatcher.BeginInvoke((Action)InitializeNavigationItems);
        }

        private void OnPlaylistContainerLoaded(object sender, EventArgs e)
        {
            _logger.Log("OnPlaylistContainerLoaded", Category.Debug, Priority.Low);
            _dispatcher.BeginInvoke((Action)InitializeNavigationItems);
            _session.PlaylistContainer.Loaded -= OnPlaylistContainerLoaded;
        }

        private void OnPlaylistMoved(object sender, Infrastructure.Interfaces.PlaylistMovedEventArgs e)
        {
            _logger.Log("OnPlaylistMoved", Category.Debug, Priority.Low);
            _dispatcher.BeginInvoke((Action)InitializeNavigationItems);
        }

        private void OnPlaylistRemoved(object sender, Infrastructure.Interfaces.PlaylistEventArgs e)
        {
            _logger.Log("OnPlaylistRemoved", Category.Debug, Priority.Low);
            _dispatcher.BeginInvoke((Action)InitializeNavigationItems);
        }

        private void OnSessionLoggedIn(object sender, SessionEventArgs e)
        {
            if (e.Status == Error.OK)
            {
                _playlistProvider.PlaylistAdded += OnPlaylistAdded;
                _playlistProvider.PlaylistMoved += OnPlaylistMoved;
                _playlistProvider.PlaylistRemoved += OnPlaylistRemoved;

                if (_session.PlaylistContainer.IsLoaded)
                {
                    _dispatcher.BeginInvoke((Action) InitializeNavigationItems);
                }
                else
                {
                    _session.PlaylistContainer.Loaded += OnPlaylistContainerLoaded;
                }

                _session.LoginComplete -= OnSessionLoggedIn;
            }
        }

        #endregion Methods
    }
}