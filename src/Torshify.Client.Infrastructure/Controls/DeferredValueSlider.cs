using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Torshify.Client.Infrastructure.Controls
{
    [TemplatePart(Name = "PART_Slider", Type = typeof(Slider))]
    [TemplatePart(Name = "PART_Progress", Type = typeof(ProgressBar))]
    public class DeferredValueSlider : RangeBase
    {
        #region Fields

        public static readonly RoutedEvent RequestValueChangedEvent = 
            EventManager.RegisterRoutedEvent(
                "RequestValueChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(DeferredValueSlider));
        public static readonly DependencyProperty RequestValueProperty = 
            DependencyProperty.Register("RequestValue", typeof(double), typeof(DeferredValueSlider),
                new FrameworkPropertyMetadata((double)0));

        private bool _isDragging;
        private ProgressBar _progressBar;
        private Slider _slider;

        #endregion Fields

        #region Constructors

        static DeferredValueSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DeferredValueSlider), new FrameworkPropertyMetadata(typeof(DeferredValueSlider)));
        }

        #endregion Constructors

        #region Events

        public event RoutedPropertyChangedEventHandler<double> RequestValueChanged
        {
            add { AddHandler(RequestValueChangedEvent, value); }
            remove { RemoveHandler(RequestValueChangedEvent, value); }
        }

        #endregion Events

        #region Properties

        public double RequestValue
        {
            get { return (double)GetValue(RequestValueProperty); }
            set { SetValue(RequestValueProperty, value); }
        }

        #endregion Properties

        #region Methods

        public override void OnApplyTemplate()
        {
            _slider = Template.FindName("PART_Slider", this) as Slider;

            if (_slider != null)
            {
                _slider.AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStarted));
                _slider.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));
                _slider.ValueChanged += OnSliderValueChanged;
            }

            _progressBar = Template.FindName("PART_Progress", this) as ProgressBar;

            if (_progressBar != null)
            {
                _progressBar.ValueChanged += OnProgressValueChanged;
            }
        }

        protected void RaiseRequestValueChangedEvent(RoutedPropertyChangedEventArgs<double> args)
        {
            args.RoutedEvent = RequestValueChangedEvent;
            RaiseEvent(args);
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isDragging = false;
            RequestValue = _slider.Value;
            RaiseRequestValueChangedEvent(new RoutedPropertyChangedEventArgs<double>(Value, _slider.Value));
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _isDragging = true;
        }

        private void OnProgressValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isDragging)
            {
                _slider.Value = e.NewValue;
            }
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isDragging)
            {
                RaiseRequestValueChangedEvent(e);
            }
        }

        #endregion Methods
    }
}