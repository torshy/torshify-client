using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Torshify.Client.Infrastructure.Commands;

namespace Torshify.Client.Infrastructure.Behaviors
{
    public class ItemsControlDragDropBehavior : Behavior<ItemsControl>
    {
        #region MoveItemCommand

        public static readonly DependencyProperty MoveItemCommandProperty =
            DependencyProperty.Register("MoveItemCommand", typeof(AutomaticCommand<Tuple<int, int>>), typeof(ItemsControlDragDropBehavior),
                new FrameworkPropertyMetadata(null));


        public AutomaticCommand<Tuple<int, int>> MoveItemCommand
        {
            get { return (AutomaticCommand<Tuple<int, int>>)GetValue(MoveItemCommandProperty); }
            set { SetValue(MoveItemCommandProperty, value); }
        }

        #endregion

        #region Fields

        private const int DRAG_WAIT_COUNTER_LIMIT = 10;

        private DragAdorner _dragAdorner;
        private int _dragScrollWaitCounter;
        private InsertAdorner _insertAdorner;

        #endregion Fields

        #region Constructors

        public ItemsControlDragDropBehavior()
        {
            _dragScrollWaitCounter = DRAG_WAIT_COUNTER_LIMIT;
        }

        #endregion Constructors

        #region Properties

        public DataTemplate ItemTemplate
        {
            get; set;
        }

        public Type ItemType
        {
            get; set;
        }

        protected object Data
        {
            get; private set;
        }

        protected Point DragStartPosition
        {
            get; private set;
        }

        protected bool IsDragging
        {
            get; private set;
        }

        protected bool IsMouseDown
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        protected override void OnAttached()
        {
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.PreviewDragEnter += OnPreviewDragEnter;
            AssociatedObject.PreviewDragOver += OnPreviewDragOver;
            AssociatedObject.PreviewQueryContinueDrag += OnPreviewQueryContinueDrag;
            AssociatedObject.PreviewDrop += OnPreviewDrop;
            AssociatedObject.DragLeave += OnDragLeave;
        }

        private void DetachAdorners()
        {
            if (_insertAdorner != null)
            {
                _insertAdorner.Destroy();
                _insertAdorner = null;
            }
            if (_dragAdorner != null)
            {
                _dragAdorner.Destroy();
                _dragAdorner = null;
            }
        }

        private void DragStarted(ItemsControl itemsControl)
        {
            if (!IsCorrectDataType())
            {
                return;
            }

            UIElement draggedItemContainer = Helper.GetItemContainerFromPoint(itemsControl, DragStartPosition);
            IsDragging = true;
            DataObject dObject = new DataObject(ItemType, Data);
            DragDropEffects e = DragDrop.DoDragDrop(itemsControl, dObject, DragDropEffects.Copy | DragDropEffects.Move);
            if ((e & DragDropEffects.Move) != 0)
            {
                if (draggedItemContainer != null)
                {
                    int dragItemIndex = itemsControl.ItemContainerGenerator.IndexFromContainer(draggedItemContainer);

                    //Helper.RemoveItem(itemsControl, dragItemIndex);
                }
                else
                {
                    //Helper.RemoveItem(itemsControl, _data);
                }
            }
            ResetState();
        }

        private void HandleDragScrolling(ItemsControl itemsControl, DragEventArgs e)
        {
            bool? isMouseAtTop = Helper.IsMousePointerAtTop(itemsControl, e.GetPosition(itemsControl));
            if (isMouseAtTop.HasValue)
            {
                if (_dragScrollWaitCounter == DRAG_WAIT_COUNTER_LIMIT)
                {
                    _dragScrollWaitCounter = 0;
                    ScrollViewer scrollViewer = Helper.FindScrollViewer(itemsControl);
                    if (scrollViewer != null && scrollViewer.CanContentScroll
                        && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                    {
                        if (isMouseAtTop.Value)
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 1.0);

                        }
                        else
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 1.0);
                        }
                        e.Effects = DragDropEffects.Scroll;
                    }
                }
                else
                {
                    _dragScrollWaitCounter++;
                }
            }
            else
            {
                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
                               DragDropEffects.Copy : DragDropEffects.Move;
            }
        }

        private void InitializeDragAdorner(ItemsControl itemsControl, object dragData, Point startPosition)
        {
            if (this.ItemTemplate != null)
            {
                if (_dragAdorner == null)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                    _dragAdorner = new DragAdorner(dragData, ItemTemplate, itemsControl, adornerLayer);
                    _dragAdorner.UpdatePosition(startPosition.X, startPosition.Y);
                }
            }
        }

        private void InitializeInsertAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (_insertAdorner == null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
                UIElement itemContainer = Helper.GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
                if (itemContainer != null)
                {
                    bool isPointInTopHalf = Helper.IsPointInTopHalf(itemsControl, e);
                    bool isItemsControlOrientationHorizontal = Helper.IsItemControlOrientationHorizontal(itemsControl);
                    _insertAdorner = new InsertAdorner(isPointInTopHalf, isItemsControlOrientationHorizontal, itemContainer, adornerLayer);
                }
            }
        }

        private int FindInsertionIndex(ItemsControl itemsControl, DragEventArgs e)
        {
            UIElement dropTargetContainer = Helper.GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
            if (dropTargetContainer != null)
            {
                int index = itemsControl.ItemContainerGenerator.IndexFromContainer(dropTargetContainer);
                if (Helper.IsPointInTopHalf(itemsControl, e))
                    return index;
                else
                    return index + 1;
            }
            return itemsControl.Items.Count;
        }

        private bool IsCorrectDataType()
        {
            // TODO: If the items actually are IEnumerable, we're in trouble
            object data = Data;

            // Assume a multiselect-drag if the data is enumerable (a faulty assumption, but an assumption non the less)
            if (Data is IEnumerable)
            {
                foreach (var dataItem in ((IEnumerable)Data))
                {
                    data = dataItem;
                    break;
                }
            }

            return ItemType.IsAssignableFrom(data.GetType());
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            DetachAdorners();
            e.Handled = true;
        }

        private void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            if (e.Data.GetDataPresent(ItemType))
            {
                object data = e.Data.GetData(ItemType);
                InitializeDragAdorner(itemsControl, data, e.GetPosition(itemsControl));
                InitializeInsertAdorner(itemsControl, e);
            }
            e.Handled = true;
        }

        private void OnPreviewDragOver(object sender, DragEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            if (e.Data.GetDataPresent(ItemType))
            {
                UpdateDragAdorner(e.GetPosition(itemsControl));
                UpdateInsertAdorner(itemsControl, e);
                HandleDragScrolling(itemsControl, e);
            }
            e.Handled = true;
        }

        private void OnPreviewDrop(object sender, DragEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            DetachAdorners();
            e.Handled = true;
            if (e.Data.GetDataPresent(ItemType))
            {
                object itemToAdd = e.Data.GetData(ItemType);
                //if ((e.KeyStates & DragDropKeyStates.ControlKey) != 0 &&
                //    Helper.DoesItemExists(itemsControl, itemToAdd))
                //{
                //    if (MessageBox.Show("Item already exists. Do you want to overwrite it?", "Copy File",
                //        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                //    {
                //        //Helper.RemoveItem(itemsControl, itemToAdd);
                //    }
                //    else
                //    {
                //        e.Effects = DragDropEffects.None;
                //        return;
                //    }
                //}
                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ?
                            DragDropEffects.Copy : DragDropEffects.Move;

                int newIndex = FindInsertionIndex(itemsControl, e);
                int oldIndex = itemsControl.Items.IndexOf(Data);

                Tuple<int, int> t = new Tuple<int, int>(oldIndex, newIndex);
                if (MoveItemCommand != null && MoveItemCommand.CanExecute(t))
                {
                    MoveItemCommand.Execute(t);
                }

                //Helper.AddItem(itemsControl, itemToAdd, FindInsertionIndex(itemsControl, e));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ItemsControl itemsControl = (ItemsControl)sender;
            Point p = e.GetPosition(itemsControl);
            Data = Helper.GetDataObjectFromItemsControl(itemsControl, p);
            if (Data != null)
            {
                IsMouseDown = true;
                DragStartPosition = p;
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResetState();
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                ItemsControl itemsControl = (ItemsControl)sender;
                Point currentPosition = e.GetPosition(itemsControl);
                if ((IsDragging == false) && (Math.Abs(currentPosition.X - DragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(currentPosition.Y - DragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    DragStarted(itemsControl);
                }
            }
        }

        private void OnPreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed)
            {
                e.Action = DragAction.Cancel;
                ResetState();
                DetachAdorners();
                e.Handled = true;
            }
        }

        private void ResetState()
        {
            IsMouseDown = false;
            IsDragging = false;
            Data = null;
            _dragScrollWaitCounter = DRAG_WAIT_COUNTER_LIMIT;
        }

        private void UpdateDragAdorner(Point currentPosition)
        {
            if (_dragAdorner != null)
            {
                _dragAdorner.UpdatePosition(currentPosition.X, currentPosition.Y);
            }
        }

        private void UpdateInsertAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (_insertAdorner != null)
            {
                _insertAdorner.IsTopHalf = Helper.IsPointInTopHalf(itemsControl, e);
                _insertAdorner.InvalidateVisual();
            }
        }

        #endregion Methods

        #region Nested Types

        public class DragAdorner : Adorner
        {
            #region Fields

            private AdornerLayer _adornerLayer;
            private ContentPresenter _contentPresenter;
            private double _leftOffset;
            private double _topOffset;

            #endregion Fields

            #region Constructors

            public DragAdorner(object data, DataTemplate dataTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
                : base(adornedElement)
            {
                _adornerLayer = adornerLayer;
                _contentPresenter = new ContentPresenter() { Content = data, ContentTemplate = dataTemplate, Opacity = 0.75 };
                IsHitTestVisible = false;
                _adornerLayer.Add(this);
            }

            #endregion Constructors

            #region Properties

            protected override int VisualChildrenCount
            {
                get { return 1; }
            }

            #endregion Properties

            #region Methods

            public void Destroy()
            {
                _adornerLayer.Remove(this);
            }

            public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
            {
                GeneralTransformGroup result = new GeneralTransformGroup();
                result.Children.Add(base.GetDesiredTransform(transform));
                result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
                return result;
            }

            public void UpdatePosition(double left, double top)
            {
                _leftOffset = left;
                _topOffset = top;
                if (_adornerLayer != null)
                {
                    _adornerLayer.Update(this.AdornedElement);
                }
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                _contentPresenter.Arrange(new Rect(finalSize));
                return finalSize;
            }

            protected override Visual GetVisualChild(int index)
            {
                return _contentPresenter;
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _contentPresenter.Measure(constraint);
                return _contentPresenter.DesiredSize;
            }

            #endregion Methods
        }

        public static class Helper
        {
            #region Methods

            public static bool DoesItemExists(ItemsControl itemsControl, object item)
            {
                if (itemsControl.Items.Count > 0)
                {
                    return itemsControl.Items.Contains(item);
                }
                return false;
            }

            public static ScrollViewer FindScrollViewer(ItemsControl itemsControl)
            {
                UIElement ele = itemsControl;
                while (ele != null)
                {
                    if (VisualTreeHelper.GetChildrenCount(ele) == 0)
                    {
                        ele = null;
                    }
                    else
                    {
                        ele = VisualTreeHelper.GetChild(ele, 0) as UIElement;
                        if (ele != null && ele is ScrollViewer)
                            return ele as ScrollViewer;
                    }
                }
                return null;
            }

            public static object GetDataObjectFromItemsControl(ItemsControl itemsControl, Point p)
            {
                //MultiSelector multiSelector = itemsControl as MultiSelector;
                //if (multiSelector != null)
                //{
                //    if (multiSelector.SelectedItems.Count == 1)
                //    {
                //        return multiSelector.SelectedItem;
                //    }

                //    return multiSelector.SelectedItems;
                //}

                //Selector selector = itemsControl as Selector;
                //if (selector != null)
                //{
                //    return selector.SelectedItem;
                //}

                UIElement element = itemsControl.InputHitTest(p) as UIElement;
                while (element != null)
                {
                    if (element == itemsControl)
                        return null;

                    object data = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                    if (data != DependencyProperty.UnsetValue)
                    {
                        return data;
                    }
                    else
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }
                }
                return null;
            }

            public static UIElement GetItemContainerFromItemsControl(ItemsControl itemsControl)
            {
                UIElement container = null;
                if (itemsControl != null && itemsControl.Items.Count > 0)
                {
                    container = itemsControl.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
                }
                else
                {
                    container = itemsControl;
                }
                return container;
            }

            public static UIElement GetItemContainerFromPoint(ItemsControl itemsControl, Point p)
            {
                UIElement element = itemsControl.InputHitTest(p) as UIElement;
                while (element != null)
                {
                    if (element == itemsControl)
                        return null;

                    object data = itemsControl.ItemContainerGenerator.ItemFromContainer(element);
                    if (data != DependencyProperty.UnsetValue)
                    {
                        return element;
                    }
                    else
                    {
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                    }
                }
                return null;
            }

            public static bool IsItemControlOrientationHorizontal(ItemsControl itemsControl)
            {
                if (itemsControl is TabControl)
                    return false;
                return true;
            }

            public static bool? IsMousePointerAtTop(ItemsControl itemsControl, Point pt)
            {
                if (pt.Y > 0.0 && pt.Y < 25)
                {
                    return true;
                }
                else if (pt.Y > itemsControl.ActualHeight - 25 && pt.Y < itemsControl.ActualHeight)
                {
                    return false;
                }
                return null;
            }

            public static bool IsPointInTopHalf(ItemsControl itemsControl, DragEventArgs e)
            {
                UIElement selectedItemContainer = GetItemContainerFromPoint(itemsControl, e.GetPosition(itemsControl));
                Point relativePosition = e.GetPosition(selectedItemContainer);
                if (IsItemControlOrientationHorizontal(itemsControl))
                {
                    return relativePosition.Y < ((FrameworkElement)selectedItemContainer).ActualHeight / 2;
                }
                return relativePosition.X < ((FrameworkElement)selectedItemContainer).ActualWidth / 2;
            }

            public static double ScrollOffsetUp(double verticaloffset, double offset)
            {
                return verticaloffset - offset < 0.0 ? 0.0 : verticaloffset - offset;
            }

            #endregion Methods
        }

        public class InsertAdorner : Adorner
        {
            #region Fields

            private AdornerLayer _adornerLayer;
            private bool _drawHorizontal;
            private Pen _pen;

            #endregion Fields

            #region Constructors

            public InsertAdorner(bool isTopHalf, bool drawHorizontal, UIElement adornedElement, AdornerLayer adornerLayer)
                : base(adornedElement)
            {
                IsTopHalf = isTopHalf;
                _adornerLayer = adornerLayer;
                _drawHorizontal = drawHorizontal;
                _adornerLayer.Add(this);
                _pen = new Pen(new SolidColorBrush(Colors.WhiteSmoke), 1.0);
                IsHitTestVisible = false;
                DoubleAnimation animation = new DoubleAnimation(0.5, 1, new Duration(TimeSpan.FromSeconds(0.5)));
                animation.AutoReverse = true;
                animation.RepeatBehavior = RepeatBehavior.Forever;
                _pen.Brush.BeginAnimation(Brush.OpacityProperty, animation);
            }

            #endregion Constructors

            #region Properties

            public bool IsTopHalf
            {
                get; set;
            }

            #endregion Properties

            #region Methods

            public void Destroy()
            {
                _adornerLayer.Remove(this);
            }

            protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
            {
                Point startPoint;
                Point endPoint;

                if (_drawHorizontal)
                    DetermineHorizontalLinePoints(out startPoint, out endPoint);
                else
                    DetermineVerticalLinePoints(out startPoint, out endPoint);

                drawingContext.DrawLine(_pen, startPoint, endPoint);
            }

            private void DetermineHorizontalLinePoints(out Point startPoint, out Point endPoint)
            {
                double width = this.AdornedElement.RenderSize.Width;
                double height = this.AdornedElement.RenderSize.Height;

                if (!this.IsTopHalf)
                {
                    startPoint = new Point(0, height);
                    endPoint = new Point(width, height);
                }
                else
                {
                    startPoint = new Point(0, 0);
                    endPoint = new Point(width, 0);
                }
            }

            private void DetermineVerticalLinePoints(out Point startPoint, out Point endPoint)
            {
                double width = this.AdornedElement.RenderSize.Width;
                double height = this.AdornedElement.RenderSize.Height;

                if (!this.IsTopHalf)
                {
                    startPoint = new Point(width, 0);
                    endPoint = new Point(width, height);
                }
                else
                {
                    startPoint = new Point(0, 0);
                    endPoint = new Point(0, height);
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}