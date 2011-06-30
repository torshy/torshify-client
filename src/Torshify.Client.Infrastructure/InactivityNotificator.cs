using System;
using System.Timers;
using System.Windows.Input;

using Microsoft.Practices.Prism.Events;

using Torshify.Client.Infrastructure.Events;
using Torshify.Client.Infrastructure.Interfaces;

namespace Torshify.Client.Infrastructure
{
    public class InactivityNotificator : IStartable
    {
        #region Fields

        private readonly IEventAggregator _eventAggregator;

        private bool _isSystemInactive;
        private bool _isApplicationInactive;
        private DateTime _lastAppicationInputActivity;
        private Timer _timer;

        #endregion Fields

        #region Constructors

        public InactivityNotificator(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        #endregion Constructors

        #region Public Methods

        public void Start()
        {
            _timer = new Timer();
            _timer.Interval = 200;
            _timer.Elapsed += OnInactivityCheckTick;
            _timer.Start();
            _lastAppicationInputActivity = DateTime.Now;
            InputManager.Current.PostProcessInput += OnPreProcess;
        }

        public void Stop()
        {
            _timer.Elapsed -= OnInactivityCheckTick;
            _timer.Stop();

            InputManager.Current.PostProcessInput -= OnPreProcess;
        }

        #endregion Public Methods

        #region Private Methods

        private void OnPreProcess(object sender, ProcessInputEventArgs e)
        {
            _lastAppicationInputActivity = DateTime.Now;
        }

        private void OnInactivityCheckTick(object sender, EventArgs e)
        {
            if (IdleTimeDetector.GetIdleTimeInfo().IdleTime > TimeSpan.FromSeconds(60))
            {
                if (!_isSystemInactive)
                {
                    _isSystemInactive = true;
                    PublishSystemActivityEvent();
                }
            }
            else
            {
                if (_isSystemInactive)
                {
                    _isSystemInactive = false;
                    PublishSystemActivityEvent();
                }
            }

            if (DateTime.Now.Subtract(_lastAppicationInputActivity) > TimeSpan.FromSeconds(10))
            {
                if (!_isApplicationInactive)
                {
                    _isApplicationInactive = true;
                    PublishApplicationActivityEvent();
                }
            }
            else
            {
                if (_isApplicationInactive)
                {
                    _isApplicationInactive = false;
                    PublishApplicationActivityEvent();
                }
            }
        }

        private void PublishSystemActivityEvent()
        {
            _eventAggregator
                .GetEvent<SystemInactivityEvent>()
                .Publish(_isSystemInactive);
        }

        private void PublishApplicationActivityEvent()
        {
            _eventAggregator
                .GetEvent<ApplicationInactivityEvent>()
                .Publish(_isApplicationInactive);
        }

        #endregion Private Methods
    }
}