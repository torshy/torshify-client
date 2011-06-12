using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Playlist
{
    public class PlaylistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private ICollectionView _tracks;
        private IPlaylist _playlist;

        #endregion Fields

        #region Constructors

        public PlaylistViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent);
        }

        #endregion Constructors

        #region Properties

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

        #endregion Properties

        #region Public Methods

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
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

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
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

        #endregion Public Methods

        #region Private Methods

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Track)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track);
        }

        #endregion Private Methods
    }
}