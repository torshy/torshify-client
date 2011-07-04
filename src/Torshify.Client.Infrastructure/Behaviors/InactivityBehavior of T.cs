using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Threading;

using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;

using Torshify.Client.Infrastructure.Events;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public abstract class InactivityBehavior<T> : Behavior<T>
        where T : FrameworkElement
    {
        #region Fields

        public static readonly DependencyProperty BeginTimeProperty = 
            DependencyProperty.Register(
                "BeginTime",
                typeof(TimeSpan?),
                typeof(InactivityBehavior<T>),
                new FrameworkPropertyMetadata(null));

        #endregion Fields

        #region Properties

        public TimeSpan? BeginTime
        {
            get { return (TimeSpan?)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            ServiceLocator
                .Current
                .TryResolve<IEventAggregator>()
                .GetEvent<ApplicationInactivityEvent>()
                .Subscribe(OnInactivityChanged);

            ServiceLocator
                .Current
                .TryResolve<IEventAggregator>()
                .GetEvent<SystemInactivityEvent>()
                .Subscribe(OnInactivityChanged);

            TriggerFadeIn();

            base.OnAttached();
        }

        protected abstract void TriggerFadeIn();

        protected abstract void TriggerFadeOut();

        private void OnInactivityChanged(bool isInactive)
        {
            if (Dispatcher.CheckAccess())
            {
                if (isInactive)
                {
                    TriggerFadeOut();
                }
                else
                {
                    TriggerFadeIn();
                }
            }
            else
            {
                Dispatcher.BeginInvoke((Action<bool>)OnInactivityChanged, DispatcherPriority.Background, isInactive);
            }
        }

        #endregion Methods
    }
}