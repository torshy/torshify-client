using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class InactivityScaleBehavior : InactivityBehavior<FrameworkElement>
    {
        #region Fields

        public static readonly DependencyProperty InDurationProperty = 
            DependencyProperty.Register("InDuration", typeof(TimeSpan), typeof(InactivityScaleBehavior),
                new FrameworkPropertyMetadata((TimeSpan)TimeSpan.FromMilliseconds(200)));
        public static readonly DependencyProperty InitialScaleXProperty = 
            DependencyProperty.Register("InitialScaleX", typeof(double), typeof(InactivityScaleBehavior),
                new FrameworkPropertyMetadata(0.9));
        public static readonly DependencyProperty InitialScaleYProperty = 
            DependencyProperty.Register("InitialScaleY", typeof(double), typeof(InactivityScaleBehavior),
                new FrameworkPropertyMetadata(0.9));
        public static readonly DependencyProperty OutDurationProperty = 
            DependencyProperty.Register("OutDuration", typeof(TimeSpan), typeof(InactivityScaleBehavior),
                new FrameworkPropertyMetadata(TimeSpan.FromMilliseconds(200)));

        #endregion Fields

        #region Properties

        public TimeSpan InDuration
        {
            get { return (TimeSpan)GetValue(InDurationProperty); }
            set { SetValue(InDurationProperty, value); }
        }

        public double InitialScaleX
        {
            get { return (double)GetValue(InitialScaleXProperty); }
            set { SetValue(InitialScaleXProperty, value); }
        }

        public double InitialScaleY
        {
            get { return (double)GetValue(InitialScaleYProperty); }
            set { SetValue(InitialScaleYProperty, value); }
        }

        public TimeSpan OutDuration
        {
            get { return (TimeSpan)GetValue(OutDurationProperty); }
            set { SetValue(OutDurationProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.RenderTransform = new ScaleTransform(InitialScaleX, InitialScaleY);
            AssociatedObject.RenderTransformOrigin = new Point(0.5, 0.5);

            base.OnAttached();
        }

        protected override void TriggerFadeIn()
        {
            Duration animationDuration = new Duration(InDuration);

            DoubleAnimation scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.To = 1.0;
            scaleXAnimation.Duration = animationDuration;
            scaleXAnimation.EasingFunction = new PowerEase();

            DoubleAnimation scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.To = 1.0;
            scaleYAnimation.Duration = animationDuration;
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
            Duration animationDuration = new Duration(OutDuration);

            DoubleAnimation scaleXAnimation = new DoubleAnimation();
            scaleXAnimation.To = InitialScaleX;
            scaleXAnimation.Duration = animationDuration;
            scaleXAnimation.EasingFunction = new PowerEase();

            DoubleAnimation scaleYAnimation = new DoubleAnimation();
            scaleYAnimation.To = InitialScaleY;
            scaleYAnimation.Duration = animationDuration;
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
            s.Begin();
        }

        #endregion Methods
    }
}