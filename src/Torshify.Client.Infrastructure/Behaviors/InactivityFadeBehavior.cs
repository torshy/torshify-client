using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Torshify.Client.Infrastructure.Events;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class InactivityFadeBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register(
                "BeginTime",
                typeof(TimeSpan?),
                typeof(InactivityFadeBehavior),
                new FrameworkPropertyMetadata(null));

        public TimeSpan? BeginTime
        {
            get { return (TimeSpan?)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Opacity = 0.0;

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
                Dispatcher.BeginInvoke((Action<bool>) OnInactivityChanged, DispatcherPriority.Background, isInactive);
            }
        }

        private void TriggerFadeIn()
        {
            Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(200));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 1.0;
            opacityAnimation.Duration = animationDuration;

            Timeline.SetDesiredFrameRate(opacityAnimation, 10);

            Storyboard.SetTarget(opacityAnimation, AssociatedObject);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));

            Storyboard s = new Storyboard();
            s.Children.Add(opacityAnimation);
            
            if (BeginTime != null)
            {
                s.BeginTime = BeginTime;
            }

            s.Begin();
        }

        private void TriggerFadeOut()
        {
            Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(200));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = animationDuration;

            Timeline.SetDesiredFrameRate(opacityAnimation, 24);

            Storyboard.SetTarget(opacityAnimation, AssociatedObject);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));

            Storyboard s = new Storyboard();
            s.Children.Add(opacityAnimation);
            s.Begin();
        }
    }
}