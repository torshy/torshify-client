using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Torshify.Client.Infrastructure.Controls
{
    public class LayeredStackPanel : Panel
    {
        #region Fields

        public static readonly DependencyProperty HorizontalItemOffsetProperty = 
            DependencyProperty.Register("HorizontalItemOffset", typeof(double), typeof(LayeredStackPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnItemPropertyChanged)));
        public static readonly DependencyProperty ItemRotateAngleOffsetProperty = 
            DependencyProperty.Register("ItemRotateAngleOffset", typeof(double), typeof(LayeredStackPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnItemPropertyChanged)));
        public static readonly DependencyProperty NumberOfItemsToShowProperty = 
            DependencyProperty.Register("NumberOfItemsToShow", typeof(int), typeof(LayeredStackPanel), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnNumberOfItemsToShowPropertyChanged)));
        public static readonly DependencyProperty VerticalItemOffsetProperty = 
            DependencyProperty.Register("VerticalItemOffset", typeof(double), typeof(LayeredStackPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnItemPropertyChanged)));

        #endregion Fields

        #region Properties

        /// <summary>
        /// Used to define how far each item is horizontally offset
        /// from one another.
        /// </summary>
        public double HorizontalItemOffset
        {
            get { return (double)GetValue(HorizontalItemOffsetProperty); }
            set { SetValue(HorizontalItemOffsetProperty, value); }
        }

        /// <summary>
        /// Used to define how far each item's rotational offset is
        /// from one another.  (Hmmm...is "rotational" a real word?)
        /// </summary>
        public double ItemRotateAngleOffset
        {
            get { return (double)GetValue(ItemRotateAngleOffsetProperty); }
            set { SetValue(ItemRotateAngleOffsetProperty, value); }
        }

        /// <summary>
        /// Integer representing the number of items to actually display.
        /// This is for the event that you might have a large list of items
        /// but only desire a select number of them to actually stack.
        /// </summary>
        public int NumberOfItemsToShow
        {
            get { return (int)GetValue(NumberOfItemsToShowProperty); }
            set { SetValue(NumberOfItemsToShowProperty, value); }
        }

        /// <summary>
        /// Used to define how far each item is vertically offset from
        /// one another.
        /// </summary>
        public double VerticalItemOffset
        {
            get { return (double)GetValue(VerticalItemOffsetProperty); }
            set { SetValue(VerticalItemOffsetProperty, value); }
        }

        #endregion Properties

        #region Methods

        protected override Size ArrangeOverride(Size finalSize)
        {
            //Get a count of the number of children we're laying out.
            int numChildrenToCheck = Math.Min(this.NumberOfItemsToShow, this.Children.Count);
            //Get the total space used in offset.
            double totalHorizontalOffsetSpace = Math.Abs((numChildrenToCheck - 1) * this.HorizontalItemOffset);
            double totalVerticalOffsetSpace = Math.Abs((numChildrenToCheck - 1) * this.VerticalItemOffset);

            double currentRotationValue = (ItemRotateAngleOffset * (numChildrenToCheck-1)) % 360;

            //Cycle through and arrange the children to be displayed.
            for (int i = 0; i < numChildrenToCheck; i++)
            {
                double rectX;
                double rectY;

                if (this.HorizontalItemOffset < 0)
                {
                    rectX = Math.Abs(this.HorizontalItemOffset * i);
                }
                else
                {
                    rectX = this.HorizontalItemOffset * (numChildrenToCheck - i - 1);
                }

                if (this.VerticalItemOffset < 0)
                {
                    rectY = Math.Abs(this.VerticalItemOffset * i);
                }
                else
                {
                    rectY = this.VerticalItemOffset * (numChildrenToCheck - i - 1);
                }

                double rectWidth = finalSize.Width - totalHorizontalOffsetSpace;
                double rectHeight = finalSize.Height - totalVerticalOffsetSpace;

                FrameworkElement child = this.Children[i] as FrameworkElement;
                Rect newRect = new Rect(rectX, rectY, rectWidth, rectHeight);

                child.RenderTransformOrigin = new Point(.5, .5);
                child.RenderTransform = new RotateTransform(currentRotationValue);
                currentRotationValue -= ItemRotateAngleOffset;

                child.Arrange(newRect);
            }

            //Cycle through the children we don't want to display
            //and "hide" them.
            for (int j = numChildrenToCheck; j < this.Children.Count; j++)
            {
                //Instead of calling telling the child's visibility
                //to be hidden or collapsed I'm going to give him a
                //lame rect instead.  This way if a user sets up a trigger
                //for the panel's item's that changes visibility I won't
                //be stepping on their toes.
                UIElement child = this.Children[j];
                Rect newRect = new Rect(0, 0, 0, 0);
                child.Arrange(newRect);
            }

            return finalSize;
        }

        protected override Size MeasureOverride(System.Windows.Size availableSize)
        {
            //Get a count of the number of children we're laying out.
            int numChildrenToCheck = Math.Min(this.NumberOfItemsToShow, this.Children.Count);
            //Get the total space used in offset.
            double totalVerticalOffsetSpace = Math.Abs((numChildrenToCheck - 1) * this.VerticalItemOffset);
            double totalHorizontalOffsetSpace = Math.Abs((numChildrenToCheck - 1) * this.HorizontalItemOffset);

            //Values to hold maximum desired height and width.
            double maxItemHeight = 0;
            double maxItemWidth = 0;

            //Cycle through the children that will be displayed
            //and determine the max desired height and width.
            for(int i=0; i<numChildrenToCheck; i++)
            {
                UIElement child = this.Children[i];

                double availableWidth = Math.Max((availableSize.Width - totalHorizontalOffsetSpace), 0);
                double availableHeight = Math.Max((availableSize.Height - totalVerticalOffsetSpace), 0);

                child.Measure(new Size(availableWidth, availableHeight));
                maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
                maxItemWidth = Math.Max(maxItemWidth, child.DesiredSize.Width);
            }

            return new Size(maxItemWidth + totalHorizontalOffsetSpace, maxItemHeight + totalVerticalOffsetSpace);
        }

        private static void OnItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayeredStackPanel currentPanel = d as LayeredStackPanel;

            if (currentPanel != null)
            {
                currentPanel.InvalidateMeasure();
            }
        }

        private static void OnNumberOfItemsToShowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LayeredStackPanel currentPanel = d as LayeredStackPanel;
            if (currentPanel != null)
            {
                currentPanel.InvalidateArrange();
            }
        }

        #endregion Methods

        #region Other

        // ------------------------------------------------------------
        //
        // Constructors
        //
        // ------------------------------------------------------------
        // ------------------------------------------------------------
        //
        // Dependancy Properties
        //
        // ------------------------------------------------------------
        // ------------------------------------------------------------
        //
        // Protected Methods
        //
        // ------------------------------------------------------------
        // ------------------------------------------------------------
        //
        // Private Methods
        //
        // ------------------------------------------------------------

        #endregion Other
    }
}