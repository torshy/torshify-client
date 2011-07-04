using System;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Torshify.Client.Infrastructure.Events;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class InactivityScaleBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty BeginTimeProperty =
           DependencyProperty.Register(
               "BeginTime",
               typeof(TimeSpan?),
               typeof(InactivityScaleBehavior),
               new FrameworkPropertyMetadata(null));

        public TimeSpan? BeginTime
        {
            get { return (TimeSpan?)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Opacity = 0.0;
            AssociatedObject.RenderTransform = new ScaleTransform(0.8, 0.8);
            AssociatedObject.RenderTransformOrigin = new Point(0.5, 0.5);

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
                Dispatcher.BeginInvoke((Action<bool>)OnInactivityChanged, DispatcherPriority.Background, isInactive);
            }
        }

        private void TriggerFadeIn()
        {
            Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(200));

            DoubleAnimation scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.To = 1.0;
            scaleXAnimation.Duration = animationDuration.Add(new Duration(TimeSpan.FromMilliseconds(500))); ;
            scaleXAnimation.EasingFunction = new ElasticEase { Springiness = 0.1 };

            DoubleAnimation scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.To = 1.0;
            scaleYAnimation.Duration = animationDuration.Add(new Duration(TimeSpan.FromMilliseconds(500)));
            scaleYAnimation.EasingFunction = new ElasticEase { Springiness = 0.1 };

            Timeline.SetDesiredFrameRate(scaleXAnimation, 10);
            Timeline.SetDesiredFrameRate(scaleYAnimation, 10);

            Storyboard.SetTarget(scaleXAnimation, AssociatedObject);
            Storyboard.SetTarget(scaleYAnimation, AssociatedObject);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.ScaleY"));

            Storyboard s = new Storyboard();
            s.Children.Add(scaleXAnimation);
            s.Children.Add(scaleYAnimation);

            if (BeginTime != null)
            {
                s.BeginTime = BeginTime;
            }

            s.Begin();
        }

        private void TriggerFadeOut()
        {
            Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(200));

            DoubleAnimation scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.To = 0.8;
            scaleXAnimation.Duration = animationDuration;
            scaleXAnimation.EasingFunction = new PowerEase();

            DoubleAnimation scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.To = 0.8;
            scaleYAnimation.Duration = animationDuration;
            scaleYAnimation.EasingFunction = new PowerEase();

            Timeline.SetDesiredFrameRate(scaleXAnimation, 24);
            Timeline.SetDesiredFrameRate(scaleYAnimation, 24);

            Storyboard.SetTarget(scaleXAnimation, AssociatedObject);
            Storyboard.SetTarget(scaleYAnimation, AssociatedObject);
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.ScaleY"));

            Storyboard s = new Storyboard();
            s.Children.Add(scaleXAnimation);
            s.Children.Add(scaleYAnimation);
            s.Begin();
        }
    }
}