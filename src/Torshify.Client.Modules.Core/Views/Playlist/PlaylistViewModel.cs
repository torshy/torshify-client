using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Playlist
{
    public class PlaylistViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private ICollectionView _tracks;

        #endregion Fields

        #region Properties

        public ICollectionView Tracks
        {
            get { return _tracks; }
            private set
            {
                _tracks = value;
                RaisePropertyChanged("Tracks");
            }
        }

        public IPlaylist Playlist
        {
            get; 
            set;
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
        }

        #endregion Public Methods
    }
}