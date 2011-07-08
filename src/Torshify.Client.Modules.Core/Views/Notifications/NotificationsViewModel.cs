using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Models;

namespace Torshify.Client.Modules.Core.Views.Notifications
{
    public class NotificationsViewModel : NotificationObject, INavigationAware
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private NotificationMessage _currentNotification;
        private SubscriptionToken _eventToken;

        #endregion Fields

        #region Constructors

        public NotificationsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventToken =_eventAggregator.GetEvent<NotificationEvent>().Subscribe(OnNotificationReceived,
                                                                     ThreadOption.PublisherThread, true);
        }

        #endregion Constructors

        #region Properties

        public NotificationMessage CurrentNotification
        {
            get { return _currentNotification; }
            set
            {
                if (_currentNotification != value)
                {
                    _currentNotification = value;
                    RaisePropertyChanged("CurrentNotification");
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
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        private void OnNotificationReceived(NotificationMessage message)
        {
            CurrentNotification = message;
        }

        #endregion Methods
    }
}