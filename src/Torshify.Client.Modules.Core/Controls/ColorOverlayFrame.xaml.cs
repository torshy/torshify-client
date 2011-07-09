using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Torshify.Client.Modules.Core.Controls
{
    public partial class ColorOverlayFrame : UserControl
    {
        #region Fields

        private List<ColorCombination> _colorCombinations;
        private int _currentColorCombinationIndex;

        #endregion Fields

        #region Constructors

        public ColorOverlayFrame()
        {
            InitializeComponent();
            InitializeColorAnimation();
        }

        #endregion Constructors

        #region Methods

        private void InitializeColorAnimation()
        {
            var overlayBrush = new LinearGradientBrush();

            _colorCombinations = new List<ColorCombination>();

            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(80, 98, 61, 5),
                    Color.FromArgb(80, 118, 0, 27)));
            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(80, 16, 98, 5),
                    Color.FromArgb(80, 118, 0, 102)));
            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(80, 5, 66, 98),
                    Color.FromArgb(80, 0, 20, 118)));
            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(80, 77, 5, 98),
                    Color.FromArgb(80, 0, 97, 118)));
            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(80, 98, 5, 40),
                    Color.FromArgb(80, 0, 118, 49)));

            var colorCombination = GetNextColorCombination();
            overlayBrush.GradientStops.Add(new GradientStop(colorCombination.Item1, 0));
            overlayBrush.GradientStops.Add(new GradientStop(colorCombination.Item2, 0.5));

            Background = overlayBrush;
            
            GetColorAnimation(
                colorCombination.Item1,
                colorCombination.Item2,
                GetNextColorCombination())
                .Begin();

            GetColorMovementAnimation().Begin();
        }

        private Storyboard GetColorAnimation(Color fromColor1, Color fromColor2, ColorCombination toColors)
        {
            ColorAnimation colorAnimation1 = new ColorAnimation();
            colorAnimation1.From = fromColor1;
            colorAnimation1.To = toColors.Item1;
            colorAnimation1.Duration = TimeSpan.FromSeconds(20);

            ColorAnimation colorAnimation2 = new ColorAnimation();
            colorAnimation2.From = fromColor2;
            colorAnimation2.To = toColors.Item2;
            colorAnimation2.Duration = TimeSpan.FromSeconds(20);

            Storyboard.SetTarget(colorAnimation1, this);
            Storyboard.SetTarget(colorAnimation2, this);
            Storyboard.SetTargetProperty(colorAnimation1, new PropertyPath("Background.GradientStops[0].Color"));
            Storyboard.SetTargetProperty(colorAnimation2, new PropertyPath("Background.GradientStops[1].Color"));

            Storyboard s = new Storyboard();
            s.Children.Add(colorAnimation1);
            s.Children.Add(colorAnimation2);
            s.Completed += OnAnimationCompleted;
            Timeline.SetDesiredFrameRate(s, 15);
            return s;
        }

        private Storyboard GetColorMovementAnimation()
        {
            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.CenterX = 0.5;
            rotateTransform.CenterY = 0.5;
            Background.RelativeTransform = rotateTransform;

            DoubleAnimation angleAnimation = new DoubleAnimation();
            angleAnimation.Duration = new Duration(TimeSpan.FromSeconds(5));
            angleAnimation.From = 0;
            angleAnimation.To = 360;
            angleAnimation.AutoReverse = true;

            Storyboard.SetTarget(angleAnimation, this);
            Storyboard.SetTargetProperty(angleAnimation, new PropertyPath("Background.RelativeTransform.Angle"));

            Storyboard s = new Storyboard();
            s.Children.Add(angleAnimation);
            return s;

            //PointAnimation startPointAnimation = new PointAnimation();
            //startPointAnimation.From = new Point(0, 0);
            //startPointAnimation.To = new Point(1, 1);
            //startPointAnimation.Duration = new Duration(TimeSpan.FromSeconds(10));
            //startPointAnimation.AutoReverse = true;

            //PointAnimation endPointAnimation = new PointAnimation();
            //endPointAnimation.From = new Point(1, 1);
            //endPointAnimation.To = new Point(0, 0);
            //endPointAnimation.Duration = new Duration(TimeSpan.FromSeconds(10));
            //endPointAnimation.AutoReverse = true;

            //Storyboard.SetTarget(startPointAnimation, this);
            //Storyboard.SetTargetProperty(startPointAnimation, new PropertyPath("Background.StartPoint"));

            //Storyboard.SetTarget(endPointAnimation, this);
            //Storyboard.SetTargetProperty(endPointAnimation, new PropertyPath("Background.EndPoint"));

            //Storyboard s = new Storyboard();
            //s.Children.Add(startPointAnimation);
            //s.Children.Add(endPointAnimation);
            //return s;
        }

        private ColorCombination GetNextColorCombination()
        {
            ColorCombination colorCombination = _colorCombinations[_currentColorCombinationIndex++];

            if (_currentColorCombinationIndex >= _colorCombinations.Count)
            {
                _currentColorCombinationIndex = 0;
            }

            return colorCombination;
        }

        private void OnAnimationCompleted(object sender, EventArgs e)
        {
            ClockGroup clockGroup = (ClockGroup)sender;
            clockGroup.Completed -= OnAnimationCompleted;

            ColorAnimation c1 = clockGroup.Timeline.Children[0] as ColorAnimation;
            ColorAnimation c2 = clockGroup.Timeline.Children[1] as ColorAnimation;

            GetColorAnimation(
                c1.To.GetValueOrDefault(),
                c2.To.GetValueOrDefault(),
                GetNextColorCombination())
                .Begin();
        }

        #endregion Methods

        #region Nested Types

        private class ColorCombination : Tuple<Color, Color>
        {
            #region Constructors

            public ColorCombination(Color item1, Color item2)
                : base(item1, item2)
            {
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }
}