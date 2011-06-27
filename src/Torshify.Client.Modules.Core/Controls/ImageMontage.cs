using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WpfShaderEffects;

namespace Torshify.Client.Modules.Core.Controls
{
    public class ImageMontage
    {
        #region Fields

        private Rectangle _imageRectangle;
        private ImageBrush _brush;

        #endregion Fields

        #region Public Properties

        public FrameworkElement UI
        {
            get { return _imageRectangle; }
        }

        #endregion Public Properties

        #region Public Methods
        
        public void Initialize(ImageSource imageSource)
        {
            if (_imageRectangle != null)
            {
                _brush.ImageSource = imageSource;
            }
            else
            {
                _imageRectangle = new Rectangle();

                _brush = new ImageBrush();
                _brush.ImageSource = imageSource;
                //_brush.Viewbox = new Rect(0.125, 0.125, 0.75, 0.75);
                _brush.Viewbox = new Rect(0.125, 0.125, 0.75, 0.75);
                _brush.Stretch = Stretch.UniformToFill;

                RectAnimation rectAnimation = new RectAnimation();
                rectAnimation.From = _brush.Viewbox;
                //rectAnimation.To = new Rect(0, 0, 0.75, 0.75);
                rectAnimation.To = new Rect(0.25, 0.25, 0.5, 0.5);
                rectAnimation.Duration = new Duration(TimeSpan.FromSeconds(30));
                rectAnimation.AutoReverse = true;
                rectAnimation.RepeatBehavior = RepeatBehavior.Forever;

                Timeline.SetDesiredFrameRate(rectAnimation, 10);

                _imageRectangle.Effect = new ColorToneShaderEffect
                                             {
                                                 DarkColor = Colors.Brown, 
                                                 LightColor = Colors.Black,
                                                 Desaturation = 0.5,
                                                 Toned = 1.0
                                             };

                _imageRectangle.Fill = _brush;
                _brush.BeginAnimation(TileBrush.ViewboxProperty, rectAnimation);
            }
        }

        #endregion Public Methods
    }
}