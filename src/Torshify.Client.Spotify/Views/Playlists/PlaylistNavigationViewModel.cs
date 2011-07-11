using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

using Microsoft.Practices.Prism.Regions;
using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;
using Torshify.Client.Spotify.Services;

namespace Torshify.Client.Spotify.Views.Playlists
{
    public class PlaylistNavigationViewModel : NavigationViewModelBase<PlaylistNavigationItem>
    {
        #region Fields

        private readonly IPlaylistProvider _playlistProvider;

        #endregion Fields

        #region Constructors

        public PlaylistNavigationViewModel(IPlaylistProvider playlistProvider, IRegionManager regionManager)
            : base(regionManager)
        {
            _playlistProvider = playlistProvider;

            Playlists = new ListCollectionView(NavigationItems);
            InitializeNavigationItems();
        }

        #endregion Constructors

        #region Properties

        public ICollectionView Playlists
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

        private void InitializeNavigationItems()
        {
            Uri navigationUri = new Uri(MusicRegionViewNames.PlaylistView, UriKind.Relative);

            ObservableCollection<PlaylistNavigationItem> currentList = NavigationItems;

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
                                            ? new PlaylistSeparatorNavigationItem(p, navigationUri)
                                            : new PlaylistNavigationItem(p, navigationUri));
                        break;
                    case PlaylistType.StartFolder:
                        var folder = new FolderPlaylistNavigationItem(p, navigationUri);
                        var children = new ObservableCollection<PlaylistNavigationItem>();
                        folder.Children = children;
                        currentList.Add(folder);
                        currentList = children;
                        break;
                    case PlaylistType.EndFolder:
                        currentList = NavigationItems;
                        break;
                    case PlaylistType.Placeholder:
                        currentList.Add(new UnknownPlaylistNavigationItem(p, navigationUri));
                        break;
                }
            }
        }

        #endregion Methods
    }
}