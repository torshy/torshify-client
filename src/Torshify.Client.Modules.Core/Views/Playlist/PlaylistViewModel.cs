using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Playlist
{
    public class PlaylistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private IPlaylist _playlist;
        private SubscriptionToken _trackMenuBarToken;
        private ICollectionView _tracks;
        private SubscriptionToken _tracksMenuBarToken;

        #endregion Fields

        #region Constructors

        public PlaylistViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            GoToAlbumCommand = new AutomaticCommand<IAlbum>(ExecuteGoToAlbum, CanExecuteGoToAlbum);
            GoToArtistCommand = new AutomaticCommand<IArtist>(ExecuteGoToArtist, CanExecuteGoToArtist);
        }

        #endregion Constructors

        #region Properties

        public AutomaticCommand<IAlbum> GoToAlbumCommand
        {
            get;
            private set;
        }

        public AutomaticCommand<IArtist> GoToArtistCommand
        {
            get;
            private set;
        }

        public IPlaylist Playlist
        {
            get
            {
                return _playlist;
            }
            set
            {
                _playlist = value;
                RaisePropertyChanged("Playlist");
            }
        }

        public ICollectionView Tracks
        {
            get
            {
                return _tracks;
            }
            private set
            {
                _tracks = value;
                RaisePropertyChanged("Tracks");
            }
        }

        #endregion Properties

        #region Methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<TrackCommandBarEvent>().Unsubscribe(_trackMenuBarToken);
            _eventAggregator.GetEvent<TracksCommandBarEvent>().Unsubscribe(_tracksMenuBarToken);

            if (Playlist != null && Tracks != null)
            {
                var selectedTrack = (ITrack)Tracks.CurrentItem;

                if (selectedTrack != null)
                {
                    UriQuery query = new UriQuery();
                    query.Add("PlaylistSelectedTrackID", selectedTrack.ID.ToString());

                    navigationContext.NavigationService.Journal.CurrentEntry.Uri = new Uri(MusicRegionViewNames.PlaylistView + query, UriKind.Relative);
                }
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);
            _tracksMenuBarToken = _eventAggregator.GetEvent<TracksCommandBarEvent>().Subscribe(OnTracksMenuBarEvent, true);

            Tracks = null;

            Playlist = navigationContext.Tag as IPlaylist;

            if (Playlist != null)
            {
                if (Playlist.Tracks != null)
                {
                    Tracks = new ListCollectionView((IList)Playlist.Tracks);

                    if (navigationContext.Parameters["PlaylistSelectedTrackID"] != null)
                    {
                        int trackId = int.Parse(navigationContext.Parameters["PlaylistSelectedTrackID"]);
                        var track = Playlist.Tracks.FirstOrDefault(t => t.ID == trackId);

                        if (track != null)
                        {
                            Tracks.MoveCurrentTo(track);
                        }
                    }
                }
            }
        }

        private bool CanExecuteGoToAlbum(IAlbum album)
        {
            return album != null;
        }

        private bool CanExecuteGoToArtist(IArtist artist)
        {
            return artist != null;
        }

        private void ExecuteGoToAlbum(IAlbum album)
        {
            Uri uri = new Uri(MusicRegionViewNames.AlbumView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri, album);
        }

        private void ExecuteGoToArtist(IArtist artist)
        {
            Uri uri = new Uri(MusicRegionViewNames.ArtistView, UriKind.Relative);
            _regionManager.RequestNavigate(CoreRegionNames.MainMusicRegion, uri, artist);
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Track)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track);
        }

        private void OnTracksMenuBarEvent(TracksCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Tracks.LastOrDefault())
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Tracks);
        }

        #endregion Methods
    }
}