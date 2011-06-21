using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Commands;
using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.NowPlaying
{
    public class NowPlayingViewModel : NotificationObject, INavigationAware
    {
        private readonly IPlayer _player;
        private IRegionNavigationService _navigationService;

        #region Constructors

        public NowPlayingViewModel(IPlayer player)
        {
            _player = player;
            _player.Playlist.CurrentChanged += OnCurrentSongChanged;
            
            NavigateBackCommand = new StaticCommand(ExecuteNavigateBack);
        }

        private void OnCurrentSongChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("CurrentTrack");
        }

        #endregion Constructors

        #region Properties

        public StaticCommand NavigateBackCommand
        {
            get;
            private set;
        }

        public ITrack CurrentTrack
        {
            get
            {
                if (_player.Playlist.Current != null)
                {
                    return _player.Playlist.Current.Track;
                }

                return null;
            }
        }

        public IEnumerable<PlayerQueueItem> Playlist
        {
            get
            {
                return _player.Playlist.Left;
            }
        }

        #endregion Properties

        #region Public Methods

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _navigationService = navigationContext.NavigationService;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion Public Methods

        #region Private Methods

        private void ExecuteNavigateBack()
        {
            if (_navigationService != null)
            {
                _navigationService.Journal.GoBack();
            }
        }

        #endregion Private Methods
    }
}