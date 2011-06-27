using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Torshify.Client.Modules.Core.Controls
{
    public class ImageMap
    {
        #region Fields

        public const int SquareSize = 70;

        public static readonly int CanvasWidth = (int)SystemParameters.FullPrimaryScreenWidth + SquareSize;
        public static readonly int CanvasHeight = (int)SystemParameters.FullPrimaryScreenHeight + SquareSize;

        public readonly int Rows = CanvasHeight / SquareSize;
        public readonly int Columns = CanvasWidth / SquareSize;

        private Dictionary<KeyValuePair<int, int>, ImageMapEntry> _map = new Dictionary<KeyValuePair<int, int>, ImageMapEntry>();
        private Random _random = new Random();
        private Canvas _grid;
        private double[] _sizeDistribution = new[] { 0.7, 0.1, 0.1, 0.05 };
        private int[] _sizeElements = new[] { 1, 2, 3, 4 };

        #endregion Fields

        #region Public Properties

        public FrameworkElement UI
        {
            get { return _grid; }
        }

        #endregion Public Properties

        #region Public Methods

        public void Initialize()
        {
            CreateMapData();
            CreateMap();
        }

        #endregion Public Methods

        #region Private Methods

        private void CreateMapData()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; )
                {
                    int maxBlockSize = _sizeElements.Length;
                    int blockSize = GetBlockSize(maxBlockSize);

                    while (!CanFitBlockInAt(row, column, blockSize))
                    {
                        maxBlockSize -= 1;

                        if (maxBlockSize == 0)
                        {
                            maxBlockSize = -1;
                            break;
                        }

                        blockSize = GetBlockSize(maxBlockSize);
                    }

                    if (maxBlockSize == -1)
                    {
                        column += 1;
                        continue;
                    }

                    ImageMapEntry entry = new ImageMapEntry();
                    entry.Size = blockSize;
                    entry.StartRow = row;
                    entry.StartColumn = column;

                    for (int r = row; r < row + blockSize; r++)
                    {
                        for (int c = column; c < column + blockSize; c++)
                        {
                            _map[new KeyValuePair<int, int>(r, c)] = entry;
                        }
                    }

                    column += blockSize;
                }
            }
        }

        private int GetBlockSize(int maxBlockSize)
        {
            double x = _random.NextDouble();
            double sum = 0;

            for (int i = 0; i < maxBlockSize; i++)
            {
                sum += _sizeDistribution[i];
                if (sum > x)
                {
                    return _sizeElements[i];
                }
            }

            return 1;
        }

        private bool CanFitBlockInAt(int row, int column, int blockSize)
        {
            for (int r = row; r < row + blockSize; r++)
            {
                for (int c = column; c < column + blockSize; c++)
                {
                    if (_map.ContainsKey(new KeyValuePair<int, int>(r, c)))
                        return false;
                }
            }

            return true;
        }

        private void CreateMap()
        {
            string[] imageFiles = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Images"), "*.jpg", SearchOption.AllDirectories);

            _grid = new Canvas();
            //_grid.Effect = new ColorToneEffect {DarkColor = Colors.Black, LightColor = Colors.DarkGray};

            foreach (var entry in _map.Values.Distinct())
            {
                var image = GetImage(imageFiles, entry);

                Canvas.SetLeft(image, entry.StartColumn * SquareSize);
                Canvas.SetTop(image, entry.StartRow * SquareSize);

                _grid.Children.Add(image);
            }
        }

        private Image GetImage(string[] allImages, ImageMapEntry block)
        {
            int index = _random.Next(allImages.Length - 1);

            string imagePath = allImages[index];
            BitmapImage bitmap = MemoryCache.Default.Get(imagePath) as BitmapImage;

            if (bitmap == null)
            {
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.DecodePixelHeight = 3*SquareSize;
                bitmap.DecodePixelWidth = 3*SquareSize;
                //bitmap.DecodePixelHeight = block.Size * SquareSize;
                //bitmap.DecodePixelWidth = block.Size * SquareSize;
                bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;
                bitmap.CacheOption = BitmapCacheOption.OnDemand;
                bitmap.EndInit();

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.SlidingExpiration = TimeSpan.FromMinutes(2);
                MemoryCache.Default.Add(imagePath, bitmap, policy);
            }

            Image image = new Image();
            image.Source = bitmap;
            image.Height = block.Size * SquareSize;
            image.Width = block.Size * SquareSize;
            image.Opacity = 0.0;
            image.RenderTransform = new ScaleTransform(0.3, 0.3);
            image.RenderTransformOrigin = new Point(0.5, 0.5);

            image.Loaded += delegate
                {
                    var animationDuration = new Duration(TimeSpan.FromMilliseconds(_random.Next(100, 1500)));

                    DoubleAnimation opacityAnimation = new DoubleAnimation();
                    opacityAnimation.To = 1.0;
                    opacityAnimation.Duration = animationDuration;

                    DoubleAnimation scaleXAnimation = new DoubleAnimation();
                    scaleXAnimation.To = 1.0;
                    scaleXAnimation.Duration = animationDuration;

                    DoubleAnimation scaleYAnimation = new DoubleAnimation();
                    scaleYAnimation.To = 1.0;
                    scaleYAnimation.Duration = animationDuration;		

                    Timeline.SetDesiredFrameRate(opacityAnimation, 10);
                    Timeline.SetDesiredFrameRate(scaleXAnimation, 10);
                    Timeline.SetDesiredFrameRate(scaleYAnimation, 10);

                    Storyboard.SetTarget(opacityAnimation, image);
                    Storyboard.SetTarget(scaleXAnimation, image);
                    Storyboard.SetTarget(scaleYAnimation, image);
                    Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
                    Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.ScaleX"));
                    Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.ScaleY"));

                    Storyboard s = new Storyboard();
                    s.BeginTime = TimeSpan.FromMilliseconds(_random.Next(100, 1500));
                    s.Children.Add(opacityAnimation);
                    s.Children.Add(scaleXAnimation);
                    s.Children.Add(scaleYAnimation);
                    s.Begin();
                };

            return image;
        }

        #endregion Private Methods

        #region Nested Types

        private class ImageMapEntry
        {
            #region Fields

            public int Size;
            public int StartRow;
            public int StartColumn;

            #endregion Fields

            #region Public Methods

            public override string ToString()
            {
                return string.Format("Size: {0}, StartColumn: {1}, StartRow: {2}", Size, StartColumn, StartRow);
            }

            #endregion Public Methods
        }

        #endregion Nested Types
    }
}