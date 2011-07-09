using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Torshify.Client.Modules.Core.Controls
{
    public partial class KenBurnsPhotoFrame : UserControl
    {
        #region Fields

        private Shape[] _imageContainers;
        private int _index;
        private Rect[] _randomRects = new[]
                                          {
                                              new Rect(0.125, 0.125, 0.75, 0.75),
                                              new Rect(0.25, 0.25, 0.5, 0.5),
                                              new Rect(0, 0, 0.75, 0.75)
                                          };

        #endregion Fields

        #region Constructors

        public KenBurnsPhotoFrame()
        {
            InitializeComponent();

            _imageContainers = new[] { _image1, _image2 };
        }

        #endregion Constructors

        #region Methods

        public void SetImageSource(ImageSource imageSource)
        {
            var fadeOutElement = _index == 0 ? _imageContainers[1] : _imageContainers[0];
            var fadeInElement = _imageContainers[_index];
            var imageBrush = new ImageBrush();
            fadeInElement.Fill = imageBrush;

            imageBrush.ImageSource = imageSource;
            imageBrush.Stretch = Stretch.UniformToFill;

            _index = _index == 0 ? 1 : 0;

            Start(fadeInElement, fadeOutElement);
        }

        private void Start(Shape elementFadeIn, Shape elementFadeOut)
        {
            FadeOutElement(elementFadeOut, TimeSpan.FromMilliseconds(500));
            FadeInElement(elementFadeIn, TimeSpan.FromMilliseconds(500));
            StartKenBurnsAnimation(elementFadeIn);
        }

        private void FadeInElement(Shape elementFadeIn, TimeSpan duration)
        {
            Duration animationDuration = new Duration(duration);
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 1.0;
            opacityAnimation.Duration = animationDuration;
            Timeline.SetDesiredFrameRate(opacityAnimation, 10);
            Storyboard.SetTarget(opacityAnimation, elementFadeIn);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            Storyboard s = new Storyboard();
            s.Children.Add(opacityAnimation);
            s.Begin();
        }

        private void FadeOutElement(Shape elementFadeOut, TimeSpan duration)
        {
            Duration animationDuration = new Duration(duration);
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            opacityAnimation.To = 0.0;
            opacityAnimation.Duration = animationDuration;
            Timeline.SetDesiredFrameRate(opacityAnimation, 10);
            Storyboard.SetTarget(opacityAnimation, elementFadeOut);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            Storyboard s = new Storyboard();
            s.Children.Add(opacityAnimation);
            s.Begin();
        }

        private void StartKenBurnsAnimation(Shape elementFadeIn)
        {
            ImageBrush brush = (ImageBrush)elementFadeIn.Fill;

            RectAnimation rectAnimation = new RectAnimation();
            rectAnimation.From = _randomRects[_index];
            rectAnimation.To = _randomRects[_index + 1];
            rectAnimation.Duration = new Duration(TimeSpan.FromSeconds(30));
            rectAnimation.AutoReverse = true;
            rectAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Timeline.SetDesiredFrameRate(rectAnimation, 20);

            brush.BeginAnimation(TileBrush.ViewboxProperty, rectAnimation);
        }

        #endregion Methods
    }
}