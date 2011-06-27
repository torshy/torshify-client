using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Torshify.Client.Modules.Core.Controls
{
    public class ImageMapOverlay
    {
        #region Fields

        private List<ColorCombination> _colorCombinations;
        private int _currentColorCombinationIndex;
        private LinearGradientBrush _overlayBrush;
        private Rectangle _overlayUI;

        #endregion Fields

        #region Properties

        public FrameworkElement UI
        {
            get { return _overlayUI; }
        }

        #endregion Properties

        #region Methods

        public void Initialize()
        {
            _overlayUI = new Rectangle();
            _overlayUI.Fill = _overlayBrush = new LinearGradientBrush();

            _colorCombinations = new List<ColorCombination>();

            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(60, 250, 0, 0),
                    Color.FromArgb(60, 0, 0, 0),
                    Color.FromArgb(60, 255, 0, 0)));
            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(60, 0, 0, 0),
                    Color.FromArgb(60, 255, 0, 0),
                    Color.FromArgb(60, 255, 255, 0)));
            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(60, 255, 0, 0),
                    Color.FromArgb(60, 255, 255, 0),
                    Color.FromArgb(60, 200, 100, 0)));
            _colorCombinations.Add(
                new ColorCombination(
                    Color.FromArgb(60, 0, 0, 255),
                    Color.FromArgb(60, 0, 0, 0),
                    Color.FromArgb(60, 100, 50, 0)));

            var colorCombination = GetNextColorCombination();
            _overlayBrush.GradientStops.Add(new GradientStop(colorCombination.Item1, 0));
            _overlayBrush.GradientStops.Add(new GradientStop(colorCombination.Item2, 0.5));
            _overlayBrush.GradientStops.Add(new GradientStop(colorCombination.Item3, 1));

            GetColorAnimation(
                colorCombination.Item1,
                colorCombination.Item2,
                colorCombination.Item3,
                GetNextColorCombination())
                .Begin();
        }

        private Storyboard GetColorAnimation(Color fromColor1, Color fromColor2, Color fromColor3, ColorCombination toColors)
        {
            ColorAnimation colorAnimation1 = new ColorAnimation();
            colorAnimation1.From = fromColor1;
            colorAnimation1.To = toColors.Item1;
            colorAnimation1.Duration = TimeSpan.FromSeconds(20);

            ColorAnimation colorAnimation2 = new ColorAnimation();
            colorAnimation2.From = fromColor2;
            colorAnimation2.To = toColors.Item2;
            colorAnimation2.Duration = TimeSpan.FromSeconds(20);

            ColorAnimation colorAnimation3 = new ColorAnimation();
            colorAnimation3.From = fromColor3;
            colorAnimation3.To = toColors.Item3;
            colorAnimation3.Duration = TimeSpan.FromSeconds(5);

            Storyboard.SetTarget(colorAnimation1, _overlayUI);
            Storyboard.SetTarget(colorAnimation2, _overlayUI);
            Storyboard.SetTarget(colorAnimation3, _overlayUI);
            Storyboard.SetTargetProperty(colorAnimation1, new PropertyPath("Fill.GradientStops[0].Color"));
            Storyboard.SetTargetProperty(colorAnimation2, new PropertyPath("Fill.GradientStops[1].Color"));
            Storyboard.SetTargetProperty(colorAnimation3, new PropertyPath("Fill.GradientStops[2].Color"));

            Storyboard s = new Storyboard();
            s.Children.Add(colorAnimation1);
            s.Children.Add(colorAnimation2);
            s.Children.Add(colorAnimation3);
            s.Completed += OnAnimationCompleted;
            Timeline.SetDesiredFrameRate(s, 10);
            return s;
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
            ClockGroup clockGroup = (ClockGroup) sender;
            clockGroup.Completed -= OnAnimationCompleted;

            ColorAnimation c1 = clockGroup.Timeline.Children[0] as ColorAnimation;
            ColorAnimation c2 = clockGroup.Timeline.Children[1] as ColorAnimation;
            ColorAnimation c3 = clockGroup.Timeline.Children[2] as ColorAnimation;

            GetColorAnimation(
                c1.To.GetValueOrDefault(),
                c2.To.GetValueOrDefault(),
                c3.To.GetValueOrDefault(),
                GetNextColorCombination())
                .Begin();
        }

        #endregion Methods

        #region Nested Types

        private class ColorCombination : Tuple<Color, Color, Color>
        {
            #region Constructors

            public ColorCombination(Color item1, Color item2, Color item3)
                : base(item1, item2, item3)
            {
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }
}