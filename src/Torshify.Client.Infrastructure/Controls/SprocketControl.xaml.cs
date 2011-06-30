#region Header

// -------------------------------------------------------------------------------
//
// This file is part of the WPFSpark project: http://wpfspark.codeplex.com/
//
// Copyright (c) 2009, Ratish Philip
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation
// and/or other materials provided with the distribution.
//
// * Neither the name of WPFSpark nor the names of its contributors may be used
// to endorse or promote products derived from this software without specific
// prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
// AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
// TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// -------------------------------------------------------------------------------

#endregion Header

using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Torshify.Client.Infrastructure.Controls
{
    public partial class SprocketControl : UserControl
    {
        #region Fields

        public static readonly DependencyProperty IntervalProperty;
        public static readonly DependencyProperty IsBusyProperty = 
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(SprocketControl),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnIsBusyChanged)));
        public static readonly DependencyProperty RotationProperty;
        public static readonly DependencyProperty StartAngleProperty;
        public static readonly DependencyProperty TickColorProperty = 
            DependencyProperty.Register("TickColor", typeof(Color), typeof(SprocketControl),
                new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsRender));

        private const double DEFAULT_INTERVAL = 60;
        private const double DEFAULT_START_ANGLE = 270;
        private const double DEFAULT_TICK_WIDTH = 2;
        private const double INNER_RADIUS_FACTOR = 0.175;
        private const double MINIMUM_INNER_RADIUS = 5;
        private const double MINIMUM_OUTER_RADIUS = 8;
        private const double MINIMUM_PEN_WIDTH = 2;
        private const double OUTER_RADIUS_FACTOR = 0.3125;

        private Size MINIMUM_CONTROL_SIZE = new Size(28, 28);
        double m_AlphaChange = 0;
        double m_AlphaLowerLimit = 0;
        double m_AngleIncrement = 0;
        Point m_CentrePt = new Point();
        double m_InnerRadius = 0;
        double m_Interval = 0;
        double m_OuterRadius = 0;
        double m_PenThickness = 0;
        List<Spoke> m_Spokes = null;
        double m_SpokesCount = 0;
        double m_StartAngle = 0;
        System.Timers.Timer m_Timer = null;

        #endregion Fields

        #region Constructors

        static SprocketControl()
        {
            // Register the Dependency Properties
            IntervalProperty = DependencyProperty.Register("Interval", typeof(double), typeof(SprocketControl),
                                new FrameworkPropertyMetadata(DEFAULT_INTERVAL, FrameworkPropertyMetadataOptions.AffectsRender, OnIntervalChanged));
            RotationProperty = DependencyProperty.Register("Rotation", typeof(Direction), typeof(SprocketControl),
                                new FrameworkPropertyMetadata(Direction.CLOCKWISE, FrameworkPropertyMetadataOptions.AffectsRender, OnRotationChanged));
            StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(double), typeof(SprocketControl),
                                new FrameworkPropertyMetadata(DEFAULT_START_ANGLE, FrameworkPropertyMetadataOptions.AffectsRender, OnStartAngleChanged));
        }

        public SprocketControl()
        {
            InitializeComponent();
            // Default number of Spokes in this control is 12
            m_SpokesCount = 12;
            // Set the Lower limit of the Alpha value (The spokes will be shown in
            // alpha values ranging from 255 to m_AlphaLowerLimit)
            m_AlphaLowerLimit = 15;

            m_Timer = new System.Timers.Timer(this.Interval);
            m_Timer.Elapsed += new ElapsedEventHandler(OnTimerElapsed);

            this.MinWidth = MINIMUM_CONTROL_SIZE.Width;
            this.MinWidth = MINIMUM_CONTROL_SIZE.Height;

            CalculateSpokesPoints();
        }

        #endregion Constructors

        #region Enumerations

        public enum Direction
        {
            CLOCKWISE,
            ANTICLOCKWISE
        }

        #endregion Enumerations

        #region Properties

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set
            {
                SetValue(IntervalProperty, value);
                m_Interval = value;
            }
        }

        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public Direction Rotation
        {
            get { return (Direction)GetValue(RotationProperty); }
            set
            {
                SetValue(RotationProperty, value);
            }
        }

        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set
            {
                SetValue(StartAngleProperty, value);
                m_StartAngle = value;
            }
        }

        public Color TickColor
        {
            get { return (Color)GetValue(TickColorProperty); }
            set { SetValue(TickColorProperty, value); }
        }

        #endregion Properties

        #region Methods

        public static void OnIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        public static void OnRotationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl ctrl = (SprocketControl)sender;
            ctrl.CalculateSpokesPoints();
        }

        public static void OnStartAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Start the Tick Control rotation
        /// </summary>
        public void Start()
        {
            if (m_Timer != null)
            {
                m_Timer.Interval = this.Interval;
                m_Timer.Enabled = true;
            }
        }

        /// <summary>
        /// Stop the Tick Control rotation
        /// </summary>
        public void Stop()
        {
            if (m_Timer != null)
            {
                m_Timer.Enabled = false;
            }
        }

        protected virtual void OnIsBusyChanged(bool oldIsBusy, bool newIsBusy)
        {
            if(newIsBusy)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            //dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 2), new Rect(10, 10, 20,20));
            if (m_Spokes == null)
                return;

            TranslateTransform translate = new TranslateTransform(m_CentrePt.X, m_CentrePt.Y);
            dc.PushTransform(translate);
            RotateTransform rotate = new RotateTransform(m_StartAngle);
            dc.PushTransform(rotate);

            byte alpha = (byte)255;

            // Render the spokes
            for (int i = 0; i < m_SpokesCount; i++)
            {
                Pen p = new Pen(new SolidColorBrush(Color.FromArgb(alpha, TickColor.R, TickColor.G, TickColor.B)), m_PenThickness);
                p.StartLineCap = p.EndLineCap = PenLineCap.Round;
                dc.DrawLine(p, m_Spokes[i].StartPoint, m_Spokes[i].EndPoint);

                alpha -= (byte)m_AlphaChange;
                if (alpha < m_AlphaLowerLimit)
                    alpha = (byte)(255 - m_AlphaChange);
            }

            // Perform a reverse Rotation and Translation to obtain the original Transformation
            dc.Pop();
            dc.Pop();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            CalculateSpokesPoints();
        }

        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SprocketControl target = (SprocketControl)d;
            bool oldIsBusy = (bool)e.OldValue;
            bool newIsBusy = target.IsBusy;
            target.OnIsBusyChanged(oldIsBusy, newIsBusy);
        }

        /// <summary>
        /// Calculate the Spoke Points and store them
        /// </summary>
        private void CalculateSpokesPoints()
        {
            m_Spokes = new List<Spoke>();

            // Calculate the angle between adjacent spokes
            m_AngleIncrement = (360 / (float)m_SpokesCount);
            // Calculate the change in alpha between adjacent spokes
            m_AlphaChange = (int)((255 - m_AlphaLowerLimit) / m_SpokesCount);

            m_StartAngle = StartAngle;

            // Calculate the location around which the spokes will be drawn
            double width = (this.Width < this.Height) ? this.Width : this.Height;
            m_CentrePt = new Point(this.Width / 2, this.Height / 2);
            // Calculate the width of the pen which will be used to draw the spokes
            m_PenThickness = width / 15;
            if (m_PenThickness < MINIMUM_PEN_WIDTH)
                m_PenThickness = MINIMUM_PEN_WIDTH;
            // Calculate the inner and outer radii of the control. The radii should not be less than the
            // Minimum values
            m_InnerRadius = (int)(width * INNER_RADIUS_FACTOR);
            if (m_InnerRadius < MINIMUM_INNER_RADIUS)
                m_InnerRadius = MINIMUM_INNER_RADIUS;
            m_OuterRadius = (int)(width * OUTER_RADIUS_FACTOR);
            if (m_OuterRadius < MINIMUM_OUTER_RADIUS)
                m_OuterRadius = MINIMUM_OUTER_RADIUS;

            double angle = 0;

            for (int i = 0; i < m_SpokesCount; i++)
            {
                Point pt1 = new Point(m_InnerRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), m_InnerRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));
                Point pt2 = new Point(m_OuterRadius * (float)Math.Cos(ConvertDegreesToRadians(angle)), m_OuterRadius * (float)Math.Sin(ConvertDegreesToRadians(angle)));

                // Create a spoke based on the points generated
                Spoke spoke = new Spoke(pt1, pt2);
                // Add the spoke to the List
                m_Spokes.Add(spoke);

                if (Rotation == Direction.CLOCKWISE)
                {
                    angle -= m_AngleIncrement;
                }
                else if (Rotation == Direction.ANTICLOCKWISE)
                {
                    angle += m_AngleIncrement;
                }
            }
        }

        /// <summary>
        /// Converts Degrees to Radians
        /// </summary>
        /// <param name="degrees">Degrees</param>
        /// <returns></returns>
        private double ConvertDegreesToRadians(double degrees)
        {
            return ((Math.PI / (double)180) * degrees);
        }

        void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    if (Rotation == Direction.CLOCKWISE)
                    {
                        m_StartAngle += m_AngleIncrement;

                        if (m_StartAngle >= 360)
                            m_StartAngle = 0;
                    }
                    else if (Rotation == Direction.ANTICLOCKWISE)
                    {
                        m_StartAngle -= m_AngleIncrement;

                        if (m_StartAngle <= -360)
                            m_StartAngle = 0;
                    }

                    InvalidateVisual();
                }
                );
        }

        #endregion Methods

        #region Nested Types

        struct Spoke
        {
            #region Fields

            public Point EndPoint;
            public Point StartPoint;

            #endregion Fields

            #region Constructors

            public Spoke(Point pt1, Point pt2)
            {
                StartPoint = pt1;
                EndPoint = pt2;
            }

            #endregion Constructors
        }

        #endregion Nested Types
    }
}