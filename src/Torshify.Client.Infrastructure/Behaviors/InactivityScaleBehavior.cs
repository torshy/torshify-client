using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class InactivityScaleBehavior : InactivityBehavior<FrameworkElement>
    {
        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.RenderTransform = new ScaleTransform(0.8, 0.8);
            AssociatedObject.RenderTransformOrigin = new Point(0.5, 0.5);

            base.OnAttached();
        }

        protected override void TriggerFadeIn()
        {
            Duration animationDuration = new Duration(TimeSpan.FromMilliseconds(200));

            DoubleAnimation scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.To = 1.0;
            scaleXAnimation.Duration = animationDuration.Add(new Duration(TimeSpan.FromMilliseconds(500))); ;
            scaleXAnimation.EasingFunction = new PowerEase();

            DoubleAnimation scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.To = 1.0;
            scaleYAnimation.Duration = animationDuration.Add(new Duration(TimeSpan.FromMilliseconds(500)));
            scaleYAnimation.EasingFunction = new PowerEase();

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

        protected override void TriggerFadeOut()
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

        #endregion Methods
    }
}