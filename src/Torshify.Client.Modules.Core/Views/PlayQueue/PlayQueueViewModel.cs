using System;
using System.Collections.Generic;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Interfaces;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.PlayQueue
{
    public class PlayQueueViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IPlayer _player;

        #endregion Fields

        #region Constructors

        public PlayQueueViewModel(IPlayer player)
        {
            _player = player;
            _player.Playlist.Changed += OnPlaylistChanged;
        }

        #endregion Constructors

        #region Properties

        public IEnumerable<PlayerQueueItem> Tracks
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

        private void OnPlaylistChanged(object sender, EventArgs e)
        {
            //RaisePropertyChanged("Tracks");
        }

        #endregion Private Methods
    }
}