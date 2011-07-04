using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class InactivityFadeBehavior : InactivityBehavior<FrameworkElement>
    {
        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.Opacity = 0.0;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Opacity = 1.0;
            base.OnDetaching();
        }

        protected override void TriggerFadeIn()
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

        protected override void TriggerFadeOut()
        {
            Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(200));

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = animationDuration;

            Timeline.SetDesiredFrameRate(opacityAnimation, 10);

            Storyboard.SetTarget(opacityAnimation, AssociatedObject);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));

            Storyboard s = new Storyboard();
            s.Children.Add(opacityAnimation);
            s.Begin();
        }

        #endregion Methods
    }
}