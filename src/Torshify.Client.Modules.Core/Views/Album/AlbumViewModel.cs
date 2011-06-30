using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Modules.Core.Views.Album
{
    public class AlbumViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private IAlbum _album;
        private SubscriptionToken _trackMenuBarToken;

        #endregion Fields

        #region Constructors

        public AlbumViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Properties

        public IAlbum Album
        {
            get
            {
                return _album;
            }
            set
            {
                if (_album != value)
                {
                    _album = value;
                    RaisePropertyChanged("Album");
                }
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
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _trackMenuBarToken = _eventAggregator.GetEvent<TrackCommandBarEvent>().Subscribe(OnTrackMenuBarEvent, true);

            Album = navigationContext.Tag as IAlbum;
        }

        private void OnTrackMenuBarEvent(TrackCommandBarModel model)
        {
            model.CommandBar
                .AddCommand("Play", CoreCommands.PlayTrackCommand, model.Track)
                .AddCommand("Queue", CoreCommands.QueueTrackCommand, model.Track);
        }

        #endregion Methods
    }
}