using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

using Torshify.Client.Infrastructure;
using Torshify.Client.Infrastructure.Controls;

namespace Torshify.Client.Modules.EchoNest.Controls
{
    public class Graph : FrameworkElement
    {
        #region Fields

        public static readonly DependencyProperty AttractionProperty = 
            DependencyProperty.Register(
                "Attraction",
                typeof(double),
                typeof(Graph),
                new FrameworkPropertyMetadata(.4, null, CoerceAttractionPropertyCallback));
        public static readonly DependencyProperty CenterObjectProperty = 
            DependencyProperty.Register(
                "CenterObject",
                typeof(object),
                typeof(Graph),
                GetCenterObjectPropertyMetadata());
        public static readonly DependencyProperty DampeningProperty = 
            DependencyProperty.Register(
                "Dampening",
                typeof(double),
                typeof(Graph),
                new FrameworkPropertyMetadata(.9, null, CoerceDampeningPropertyCallback));
        public static readonly DependencyProperty IsCenterProperty;
        public static readonly DependencyProperty LinePenProperty = 
            DependencyProperty.Register(
                "LinePen",
                typeof(Pen),
                typeof(Graph),
                new PropertyMetadata(GetPen()));
        public static readonly DependencyProperty NodesBindingPathProperty = 
            DependencyProperty.Register(
                "NodesBindingPath",
                typeof(string),
                typeof(Graph),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(NodesBindingPathPropertyChanged)));
        public static readonly DependencyProperty NodeTemplateProperty = 
            DependencyProperty.Register(
                "NodeTemplate",
                typeof(DataTemplate),
                typeof(Graph),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty NodeTemplateSelectorProperty = 
            DependencyProperty.Register(
                "NodeTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(Graph),
                new FrameworkPropertyMetadata(null));

        private const double MinDampening = .01, MaxDampening = .99;
        private const double MinVelocity = .05;
        private const double TerminalVelocity = 150;

        private static readonly DependencyPropertyKey IsCentersCenterPropertyKey = 
            DependencyProperty.RegisterAttachedReadOnly(
                "IsCenter",
                typeof(bool),
                typeof(Graph),
                new FrameworkPropertyMetadata(false));
        private static readonly DependencyProperty NodesProperty = 
            DependencyProperty.Register(
                "Nodes",
                typeof(IList),
                typeof(Graph),
                GetNodesPropertyMetadata());
        private static readonly Duration _hideDuration = new Duration(new TimeSpan(0, 0, 1));
        private static readonly TimeSpan _maxSettleTime = new TimeSpan(0, 0, 8);
        private static readonly Duration _showDuration = new Duration(new TimeSpan(0, 0, 0, 0, 500));

        private readonly List<GraphContentPresenter> _fadingGCPList = new List<GraphContentPresenter>();
        private readonly CompositionTargetRenderingListener _listener = new CompositionTargetRenderingListener();
        private readonly List<GraphContentPresenter> _nodePresenters;
        private readonly Binding _nodeTemplateBinding;
        private readonly Binding _nodeTemplateSelectorBinding;

        private static Pen _defaultPen;

        private bool _centerChanged;
        private object _centerDataInUse;
        private GraphContentPresenter _centerGraphContentPresenter;
        private Point _controlCenterPoint;
        private bool _measureInvalidated;
        private int _milliseconds = int.MinValue;
        private bool _nodeCollectionChanged;
        private bool _nodesChanged;
        private IList _nodesInUse;
        private bool _stillMoving;

        #endregion Fields

        #region Constructors

        static Graph()
        {
            IsCenterProperty = IsCentersCenterPropertyKey.DependencyProperty;
            ClipToBoundsProperty.OverrideMetadata(typeof(Graph), new FrameworkPropertyMetadata(true));
        }

        public Graph()
        {
            _listener.WireParentLoadedUnloaded(this);

            _listener.Rendering += compositionTarget_rendering;

            _nodeTemplateBinding = new Binding(NodeTemplateProperty.Name);
            _nodeTemplateBinding.Source = this;

            _nodeTemplateSelectorBinding = new Binding(NodeTemplateSelectorProperty.Name);
            _nodeTemplateSelectorBinding.Source = this;

            _nodePresenters = new List<GraphContentPresenter>();
        }

        #endregion Constructors

        #region Properties

        public double Attraction
        {
            get { return (double)GetValue(AttractionProperty); }
            set
            {
                SetValue(AttractionProperty, value);
            }
        }

        public object CenterObject
        {
            get { return GetValue(CenterObjectProperty); }
            set { SetValue(CenterObjectProperty, value); }
        }

        public double Dampening
        {
            get
            {
                return (double)GetValue(DampeningProperty);
            }
            set
            {
                SetValue(DampeningProperty, value);
            }
        }

        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }

        public string NodesBindingPath
        {
            get
            {
                return (string)GetValue(NodesBindingPathProperty);
            }
            set
            {
                SetValue(NodesBindingPathProperty, value);
            }
        }

        public DataTemplate NodeTemplate
        {
            get
            {
                return (DataTemplate)GetValue(NodeTemplateProperty);
            }
            set
            {
                SetValue(NodeTemplateProperty, value);
            }
        }

        public DataTemplateSelector NodeTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)GetValue(NodeTemplateSelectorProperty);
            }
            set
            {
                SetValue(NodeTemplateSelectorProperty, value);
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _nodePresenters.Count + _fadingGCPList.Count + ((_centerGraphContentPresenter == null) ? 0 : 1);
            }
        }

        private IEnumerable<GraphContentPresenter> Children
        {
            get
            {
                for (int i = 0; i < VisualChildrenCount; i++)
                {
                    yield return (GraphContentPresenter)GetVisualChild(i);
                }
            }
        }

        private IList Nodes
        {
            get
            {
                return (IList)GetValue(NodesProperty);
            }
        }

        #endregion Properties

        #region Methods

        public static bool GetIsCenter(ContentPresenter element)
        {
            return (bool)element.GetValue(IsCenterProperty);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _controlCenterPoint = (Point)(.5 * (Vector)finalSize);
            Children.ForEach(gcp => gcp.Arrange(new Rect(gcp.DesiredSize)));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < _fadingGCPList.Count)
            {
                return _fadingGCPList[index];
            }
            index -= _fadingGCPList.Count;

            if (index < _nodePresenters.Count)
            {
                return _nodePresenters[index];
            }
            index -= _nodePresenters.Count;

            if (index == 0)
            {
                return _centerGraphContentPresenter;
            }
            else
            {
                throw new ArgumentException("not a valid index");
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            HandleChanges();
            _measureInvalidated = true;

            _listener.StartListening();

            Children.ForEach(gcp => gcp.Measure(GeoHelper.SizeInfinite));

            return new Size();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (LinePen != null && _centerGraphContentPresenter != null)
            {
                var pen = LinePen;
                _nodePresenters.ForEach(gcp => drawingContext.DrawLine(pen, _centerGraphContentPresenter.ActualLocation, gcp.ActualLocation));
            }
        }

        private static void CenterObjectPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            ((Graph)element).CenterObjectPropertyChanged();
        }

        private static object CoerceAttractionPropertyCallback(DependencyObject element, object baseValue)
        {
            return CoerceAttractionPropertyCallback((double)baseValue);
        }

        private static double CoerceAttractionPropertyCallback(double baseValue)
        {
            if (baseValue <= MinDampening)
            {
                return MinDampening;
            }
            else if (baseValue >= MaxDampening)
            {
                return MaxDampening;
            }
            else
            {
                return baseValue;
            }
        }

        private static object CoerceDampeningPropertyCallback(DependencyObject element, object baseValue)
        {
            return CoerceDampeningPropertyCallback((double)baseValue);
        }

        private static double CoerceDampeningPropertyCallback(double baseValue)
        {
            if (baseValue <= MinDampening)
            {
                return MinDampening;
            }
            else if (baseValue >= MaxDampening)
            {
                return MaxDampening;
            }
            else
            {
                return baseValue;
            }
        }

        private static Vector EnsureNonzeroVector(Vector vector)
        {
            if (vector.Length > 0)
            {
                return vector;
            }
            else
            {
                return new Vector(Util.Rnd.NextDouble() - .5, Util.Rnd.NextDouble() - .5);
            }
        }

        private static Vector GetAttractionForce(Vector x)
        {
            Vector force = -.2 * Normalize(x) * x.Length;
            Debug.Assert(force.IsValid());
            return force;
        }

        private static Binding GetBinding(string bindingPath, object source)
        {
            Binding newBinding = null;
            try
            {
                newBinding = new Binding(bindingPath);
                newBinding.Source = source;
                newBinding.Mode = BindingMode.OneWay;
            }
            catch (InvalidOperationException) { }
            return newBinding;
        }

        private static Rect GetCenteredRect(Size elementSize, Point center)
        {
            double x = center.X - elementSize.Width / 2;
            double y = center.Y - elementSize.Height / 2;

            return new Rect(x, y, elementSize.Width, elementSize.Height);
        }

        private static PropertyMetadata GetCenterObjectPropertyMetadata()
        {
            FrameworkPropertyMetadata fpm = new FrameworkPropertyMetadata();
            fpm.AffectsMeasure = true;
            fpm.PropertyChangedCallback = new PropertyChangedCallback(CenterObjectPropertyChanged);
            return fpm;
        }

        private static double GetForce(double x)
        {
            return GetSCurve((x + 100) / 200);
        }

        private static GraphContentPresenter GetGraphContentPresenter(object content, BindingBase nodeTemplateBinding,
            BindingBase nodeTemplateSelectorBinding, bool offsetCenter)
        {
            GraphContentPresenter gcp =
                new GraphContentPresenter(content, nodeTemplateBinding, nodeTemplateSelectorBinding, offsetCenter);
            return gcp;
        }

        private static PropertyMetadata GetNodesPropertyMetadata()
        {
            FrameworkPropertyMetadata fpm = new FrameworkPropertyMetadata();
            fpm.AffectsMeasure = true;
            fpm.PropertyChangedCallback = new PropertyChangedCallback(NodesPropertyChanged);
            return fpm;
        }

        private static Pen GetPen()
        {
            if (_defaultPen == null)
            {
                _defaultPen = new Pen(Brushes.Gray, 1);
                _defaultPen.Freeze();
            }
            return _defaultPen;
        }

        private static Point GetRandomPoint(Size range)
        {
            return new Point(Util.Rnd.NextDouble() * range.Width, Util.Rnd.NextDouble() * range.Height);
        }

        private static Vector GetRepulsiveForce(Vector x)
        {
            Vector force = .1 * Normalize(x) / Math.Pow(x.Length / 1000, 2);
            Debug.Assert(force.IsValid());
            return force;
        }

        private static double GetSCurve(double x)
        {
            return 0.5 + Math.Sin(Math.Abs(x * (Math.PI / 2)) - Math.Abs((x * (Math.PI / 2)) - (Math.PI / 2))) / 2;
        }

        private static Vector GetSpringForce(Vector x)
        {
            Vector force = new Vector();
            //negative is attraction
            force += GetAttractionForce(x);
            //positive is repulsion
            force += GetRepulsiveForce(x);

            Debug.Assert(force.IsValid());

            return force;
        }

        private static void NodesBindingPathPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            Graph g = (Graph)element;
            g.ResetNodesBinding();
        }

        private static void NodesPropertyChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            ((Graph)element).NodesPropertyChanged();
        }

        private static Vector Normalize(Vector v)
        {
            v.Normalize();
            Debug.Assert(v.IsValid());
            return v;
        }

        private static void SetIsCenter(ContentPresenter element, bool value)
        {
            element.SetValue(IsCentersCenterPropertyKey, value);
        }

        private static bool UpdateGraphCp(GraphContentPresenter graphContentPresenter, Vector forceVector,
            double coefficientOfDampening, double frameRate, Point parentCenter)
        {
            bool parentCenterChanged = (graphContentPresenter.ParentCenter != parentCenter);
            if (parentCenterChanged)
            {
                graphContentPresenter.ParentCenter = parentCenter;
            }

            //add system drag
            Debug.Assert(coefficientOfDampening > 0);
            Debug.Assert(coefficientOfDampening < 1);
            graphContentPresenter.Velocity *= (1 - coefficientOfDampening * frameRate);

            //add force
            graphContentPresenter.Velocity += (forceVector * frameRate);

            //apply terminalVelocity
            if (graphContentPresenter.Velocity.Length > TerminalVelocity)
            {
                graphContentPresenter.Velocity *= (TerminalVelocity / graphContentPresenter.Velocity.Length);
            }

            if (graphContentPresenter.Velocity.Length > MinVelocity && forceVector.Length > MinVelocity)
            {
                graphContentPresenter.Location += (graphContentPresenter.Velocity * frameRate);
                return true;
            }
            else
            {
                graphContentPresenter.Velocity = new Vector();
                return false || parentCenterChanged;
            }
        }

        private void BeginRemoveAnimation(GraphContentPresenter graphContentPresenter, bool isCenter)
        {
            Debug.Assert(VisualTreeHelper.GetParent(graphContentPresenter) == this);

            this.InvalidateVisual();

            _fadingGCPList.Add(graphContentPresenter);

            graphContentPresenter.IsHitTestVisible = false;
            if (isCenter)
            {
                graphContentPresenter.WasCenter = true;
            }

            ScaleTransform scaleTransform = graphContentPresenter.ScaleTransform;

            DoubleAnimation doubleAnimation = new DoubleAnimation(0, _hideDuration);
            doubleAnimation.Completed +=
                delegate(object sender, EventArgs e)
                {
                    CleanUpGCP(graphContentPresenter);
                };
            doubleAnimation.FillBehavior = FillBehavior.Stop;
            doubleAnimation.Freeze();

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);
            graphContentPresenter.BeginAnimation(OpacityProperty, doubleAnimation);
        }

        private bool BelowMaxSettleTime()
        {
            Debug.Assert(_milliseconds != int.MinValue);

            return _maxSettleTime > TimeSpan.FromMilliseconds(Environment.TickCount - _milliseconds);
        }

        private void CenterObjectPropertyChanged()
        {
            _centerChanged = true;
            ResetNodesBinding();
        }

        private void CleanUpGCP(GraphContentPresenter contentPresenter)
        {
            if (_fadingGCPList.Contains(contentPresenter))
            {
                Debug.Assert(VisualTreeHelper.GetParent(contentPresenter) == this);

                this.RemoveVisualChild(contentPresenter);
                _fadingGCPList.Remove(contentPresenter);
            }
        }

        private void compositionTarget_rendering(object sender, EventArgs args)
        {
            bool _somethingInvalid = false;
            if (_measureInvalidated || _stillMoving)
            {
                if (_measureInvalidated)
                {
                    _milliseconds = Environment.TickCount;
                }

                #region CenterObject
                if (_centerGraphContentPresenter != null)
                {
                    if (_centerGraphContentPresenter.New)
                    {
                        _centerGraphContentPresenter.ParentCenter = _controlCenterPoint;
                        _centerGraphContentPresenter.New = false;
                        _somethingInvalid = true;
                    }
                    else
                    {
                        Vector forceVector = GetAttractionForce(
                            EnsureNonzeroVector((Vector)_centerGraphContentPresenter.Location));

                        if (UpdateGraphCp(_centerGraphContentPresenter, forceVector, Dampening, Attraction, _controlCenterPoint))
                        {
                            _somethingInvalid = true;
                        }
                    }
                }
                #endregion

                Point centerLocationToUse = (_centerGraphContentPresenter != null) ? _centerGraphContentPresenter.Location : new Point();

                GraphContentPresenter gcp;
                for (int i = 0; i < _nodePresenters.Count; i++)
                {
                    Vector forceVector = new Vector();
                    gcp = _nodePresenters[i];

                    if (gcp.New)
                    {
                        gcp.New = false;
                        _somethingInvalid = true;
                    }

                    for (int j = 0; j < _nodePresenters.Count; j++)
                    {
                        if (j != i)
                        {
                            Vector distance = EnsureNonzeroVector(gcp.Location - _nodePresenters[j].Location);
                            Vector repulsiveForce = GetRepulsiveForce(distance);

                            forceVector += repulsiveForce;
                        }
                    }

                    forceVector += GetSpringForce(EnsureNonzeroVector(_nodePresenters[i].Location - centerLocationToUse));

                    if (UpdateGraphCp(_nodePresenters[i], forceVector, Dampening, Attraction, _controlCenterPoint))
                    {
                        _somethingInvalid = true;
                    }
                }

                #region animate all of the fading ones away
                for (int i = 0; i < _fadingGCPList.Count; i++)
                {
                    if (!_fadingGCPList[i].WasCenter)
                    {
                        Vector centerDiff = EnsureNonzeroVector(_fadingGCPList[i].Location - centerLocationToUse);
                        centerDiff.Normalize();
                        centerDiff *= 20;
                        if (UpdateGraphCp(_fadingGCPList[i], centerDiff, Dampening, Attraction, _controlCenterPoint))
                        {
                            _somethingInvalid = true;
                        }
                    }
                }

                #endregion

                if (_somethingInvalid && BelowMaxSettleTime())
                {
                    _stillMoving = true;
                    InvalidateVisual();
                }
                else
                {
                    _stillMoving = false;
                    _listener.StopListening();
                }
                _measureInvalidated = false;
            }
        }

        private void HandleChanges()
        {
            HandleNodesChangedWiring();

            if (_centerChanged && _nodeCollectionChanged &&
                CenterObject != null &&
                _centerGraphContentPresenter != null
                )
            {
                Debug.Assert(!CenterObject.Equals(_centerDataInUse));
                Debug.Assert(_centerGraphContentPresenter.Content == null || _centerGraphContentPresenter.Content.Equals(_centerDataInUse));

                _centerDataInUse = CenterObject;

                //figure out if we can re-cycle one of the existing children as the center Node
                //if we can, newCenter != null
                GraphContentPresenter newCenterPresenter = null;
                for (int i = 0; i < _nodePresenters.Count; i++)
                {
                    if (_nodePresenters[i].Content.Equals(CenterObject))
                    {
                        //we should re-use this
                        newCenterPresenter = _nodePresenters[i];
                        _nodePresenters[i] = null;
                        break;
                    }
                }

                //figure out if we can re-cycle the exsting center as one of the new child nodes
                //if we can, newChild != null && newChildIndex == indexOf(data in Nodes)
                int newChildIndex = -1;
                GraphContentPresenter newChildPresenter = null;
                for (int i = 0; i < _nodesInUse.Count; i++)
                {
                    if (_nodesInUse[i] != null && _centerGraphContentPresenter.Content != null && _nodesInUse[i].Equals(_centerGraphContentPresenter.Content))
                    {
                        newChildIndex = i;
                        newChildPresenter = _centerGraphContentPresenter;
                        _centerGraphContentPresenter = null;
                        break;
                    }
                }

                //now we potentially have a center (or not) and one edge(or not)
                GraphContentPresenter[] newChildren = new GraphContentPresenter[_nodesInUse.Count];

                //we did all the work to see if the current cernter can be reused.
                //if it can, use it
                if (newChildPresenter != null)
                {
                    newChildren[newChildIndex] = newChildPresenter;
                }

                //now go through all the existing children and place them in newChildren
                //if they match
                for (int i = 0; i < _nodesInUse.Count; i++)
                {
                    if (newChildren[i] == null)
                    {
                        for (int j = 0; j < _nodePresenters.Count; j++)
                        {
                            if (_nodePresenters[j] != null)
                            {
                                if (_nodesInUse[i].Equals(_nodePresenters[j].Content))
                                {
                                    Debug.Assert(newChildren[i] == null);
                                    newChildren[i] = _nodePresenters[j];
                                    _nodePresenters[j] = null;
                                    break;
                                }
                            }
                        }
                    }
                }

                //we've now reused everything we can
                if (_centerGraphContentPresenter == null)
                {
                    //we didn't find anything to recycle
                    //create a new one
                    if (newCenterPresenter == null)
                    {
                        _centerGraphContentPresenter = GetGraphContentPresenter(
                            CenterObject,
                            _nodeTemplateBinding,
                            _nodeTemplateSelectorBinding,
                            false
                            );
                        this.AddVisualChild(_centerGraphContentPresenter);
                    }
                    else
                    { //we did find something to recycle. Use it.
                        _centerGraphContentPresenter = newCenterPresenter;
                        Debug.Assert(VisualTreeHelper.GetParent(newCenterPresenter) == this);
                    }
                }
                else
                {
                    if (newCenterPresenter == null)
                    {
                        _centerGraphContentPresenter.Content = CenterObject;
                    }
                    else
                    {
                        BeginRemoveAnimation(_centerGraphContentPresenter, true);
                        _centerGraphContentPresenter = newCenterPresenter;
                        Debug.Assert(VisualTreeHelper.GetParent(newCenterPresenter) == this);
                    }
                }

                //go through all of the old CPs that are not being used and remove them
                _nodePresenters
                  .Where(gcp => gcp != null)
                  .ForEach(gcp => BeginRemoveAnimation(gcp, false));

                //go through and "fill in" all the new CPs
                for (int i = 0; i < _nodesInUse.Count; i++)
                {
                    if (newChildren[i] == null)
                    {
                        GraphContentPresenter gcp = GetGraphContentPresenter(_nodesInUse[i],
                            _nodeTemplateBinding, _nodeTemplateSelectorBinding, true);
                        this.AddVisualChild(gcp);
                        newChildren[i] = gcp;
                    }
                }

                _nodePresenters.Clear();
                _nodePresenters.AddRange(newChildren);

                _centerChanged = false;
                _nodeCollectionChanged = false;
            }
            else
            {
                if (_centerChanged)
                {
                    _centerDataInUse = CenterObject;
                    if (_centerGraphContentPresenter != null)
                    {
                        Debug.Assert(_centerDataInUse == null);
                        BeginRemoveAnimation(_centerGraphContentPresenter, true);
                        _centerGraphContentPresenter = null;
                    }
                    if (_centerDataInUse != null)
                    {
                        SetUpCleanCenter(_centerDataInUse);
                    }
                    _centerChanged = false;
                }

                if (_nodeCollectionChanged)
                {
                    SetupNodes(Nodes);

                    _nodesInUse = Nodes;

                    _nodeCollectionChanged = false;
                }
            }

            #if DEBUG
              if (CenterObject != null) {
            CenterObject.Equals(_centerDataInUse);
            Debug.Assert(_centerGraphContentPresenter != null);
              }
              else {
            Debug.Assert(_centerDataInUse == null);
              }
              if (Nodes != null) {
            Debug.Assert(_nodePresenters != null);
            Debug.Assert(Nodes.Count == _nodePresenters.Count);
            Debug.Assert(_nodesInUse == Nodes);
              }
              else {
            Debug.Assert(_nodesInUse == null);
            if (_nodePresenters != null) {
              Debug.Assert(_nodePresenters.Count == 0);
            }
              }
            #endif

            Children.ForEach(gcp => SetIsCenter(gcp, gcp == _centerGraphContentPresenter));
        }

        private void HandleNodesChangedWiring()
        {
            if (_nodesChanged)
            {
                INotifyCollectionChanged oldList = _nodesInUse as INotifyCollectionChanged;
                if (oldList != null)
                {
                    oldList.CollectionChanged -= NodesChangedHandler;
                }

                INotifyCollectionChanged newList = Nodes as INotifyCollectionChanged;
                if (newList != null)
                {
                    newList.CollectionChanged += NodesChangedHandler;
                }

                _nodesInUse = Nodes;
                _nodesChanged = false;
            }
        }

        private void NodesChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            VerifyAccess();
            InvalidateMeasure();
            _nodeCollectionChanged = true;
        }

        private void NodesPropertyChanged()
        {
            _nodeCollectionChanged = true;
            _nodesChanged = true;
        }

        private void ResetNodesBinding()
        {
            if (NodesBindingPath == null)
            {
                BindingOperations.ClearBinding(this, NodesProperty);
            }
            else
            {
                Binding theBinding = GetBinding(NodesBindingPath, this.CenterObject);
                if (theBinding == null)
                {
                    BindingOperations.ClearBinding(this, NodesProperty);
                }
                else
                {
                    BindingOperations.SetBinding(this, NodesProperty, theBinding);
                }
            }
        }

        private void SetUpCleanCenter(object newCenter)
        {
            Debug.Assert(_centerGraphContentPresenter == null);

            _centerGraphContentPresenter = GetGraphContentPresenter(newCenter, _nodeTemplateBinding, _nodeTemplateSelectorBinding, false);
            this.AddVisualChild(_centerGraphContentPresenter);
        }

        private void SetupNodes(IList nodes)
        {
            #if DEBUG
              for (int i = 0; i < _nodePresenters.Count; i++) {
            Debug.Assert(_nodePresenters[i] != null);
            Debug.Assert(VisualTreeHelper.GetParent(_nodePresenters[i]) == this);
              }
            #endif

            int nodesCount = (nodes == null) ? 0 : nodes.Count;

            GraphContentPresenter[] newNodes = new GraphContentPresenter[nodesCount];
            for (int i = 0; i < nodesCount; i++)
            {
                for (int j = 0; j < _nodePresenters.Count; j++)
                {
                    if (_nodePresenters[j] != null)
                    {
                        if (nodes[i] == _nodePresenters[j].Content)
                        {
                            newNodes[i] = _nodePresenters[j];
                            _nodePresenters[j] = null;
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < _nodePresenters.Count; i++)
            {
                if (_nodePresenters[i] != null)
                {
                    BeginRemoveAnimation(_nodePresenters[i], false);
                    _nodePresenters[i] = null;
                }
            }

            for (int i = 0; i < newNodes.Length; i++)
            {
                if (newNodes[i] == null)
                {
                    newNodes[i] = GetGraphContentPresenter(nodes[i],
                        _nodeTemplateBinding, _nodeTemplateSelectorBinding, true);
                    this.AddVisualChild(newNodes[i]);
                }
            }

            #if DEBUG
              _nodePresenters.ForEach(item => Debug.Assert(item == null));
              newNodes.CountForEach((item, i) => {
            Debug.Assert(item != null);
            Debug.Assert(VisualTreeHelper.GetParent(item) == this);
            Debug.Assert(item.Content == nodes[i]);
              });
            #endif

            _nodePresenters.Clear();
            _nodePresenters.AddRange(newNodes);
        }

        #endregion Methods

        #region Nested Types

        private class GraphContentPresenter : ContentPresenter
        {
            #region Fields

            public bool New = true;
            public ScaleTransform ScaleTransform;
            public Vector Velocity;
            public bool WasCenter;

            private readonly TranslateTransform m_translateTransform;

            private Vector m_centerVector;
            private Point m_location;
            private Point m_parentCenter;

            #endregion Fields

            #region Constructors

            public GraphContentPresenter(object content,
                BindingBase nodeTemplateBinding, BindingBase nodeTemplateSelectorBinding, bool offsetCenter)
                : base()
            {
                Content = content;

                SetBinding(ContentPresenter.ContentTemplateProperty, nodeTemplateBinding);
                SetBinding(ContentPresenter.ContentTemplateSelectorProperty, nodeTemplateSelectorBinding);

                ScaleTransform = new ScaleTransform();
                if (offsetCenter)
                {
                    m_translateTransform = new TranslateTransform(Util.Rnd.NextDouble() - .5, Util.Rnd.NextDouble() - .5);
                }
                else
                {
                    m_translateTransform = new TranslateTransform();
                }

                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(ScaleTransform);
                transformGroup.Children.Add(m_translateTransform);

                this.RenderTransform = transformGroup;

                var doubleAnimation = new DoubleAnimation(.5, 1, _showDuration);
                this.BeginAnimation(OpacityProperty, doubleAnimation);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, doubleAnimation);
                ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, doubleAnimation);

                LayoutUpdated += (sender, args) =>
                {
                    ScaleTransform.CenterX = RenderSize.Width / 2;
                    ScaleTransform.CenterY = RenderSize.Height / 2;

                    m_centerVector = -.5 * (Vector)RenderSize;
                    UpdateTransform();
                };
            }

            #endregion Constructors

            #region Properties

            public Point ActualLocation
            {
                get
                {
                    return new Point(m_location.X * 1.5 + m_parentCenter.X, m_location.Y * 1.5 + m_parentCenter.Y);
                }
            }

            public Point Location
            {
                get { return m_location; }
                set
                {
                    if (m_location != value)
                    {
                        m_location = value;
                        UpdateTransform();
                    }
                }
            }

            public Point ParentCenter
            {
                get
                {
                    return m_parentCenter;
                }
                set
                {
                    if (m_parentCenter != value)
                    {
                        m_parentCenter = value;
                        UpdateTransform();
                    }
                }
            }

            #endregion Properties

            #region Methods

            private void UpdateTransform()
            {
                m_translateTransform.SetToVector(m_centerVector + (Vector)m_location * 1.5 + (Vector)m_parentCenter);
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}