using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using Torshify.Client.Modules.EchoNest.Controls;

namespace Torshify.Client.Modules.EchoNest.Behaviors
{
    public class TiltBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty KeepDraggingProperty =
            DependencyProperty.Register("KeepDragging", typeof(bool), typeof(TiltBehavior), new PropertyMetadata(true));

        public bool KeepDragging
        {
            get { return (bool)GetValue(KeepDraggingProperty); }
            set { SetValue(KeepDraggingProperty, value); }
        }


        public static readonly DependencyProperty TiltFactorProperty =
            DependencyProperty.Register("TiltFactor", typeof(Int32), typeof(TiltBehavior), new PropertyMetadata(20));


        public Int32 TiltFactor
        {
            get { return (Int32)GetValue(TiltFactorProperty); }
            set { SetValue(TiltFactorProperty, value); }
        }

        private FrameworkElement attachedElement;
        private Panel OriginalPanel;
        private Thickness OriginalMargin;
        private Size OriginalSize;
        protected override void OnAttached()
        {
            attachedElement = this.AssociatedObject;

            if (attachedElement as Panel != null)
            {
                (attachedElement as Panel).Loaded += (sl, el) =>
                                                         {
                                                             List<UIElement> elements = new List<UIElement>();

                                                             foreach (UIElement ui in (attachedElement as Panel).Children)
                                                             {
                                                                 elements.Add(ui);
                                                             }
                                                             elements.ForEach((element) => Interaction.GetBehaviors(element).Add(new TiltBehavior() { KeepDragging = this.KeepDragging, TiltFactor = this.TiltFactor }));
                                                         };

                return;
            }

            OriginalPanel = attachedElement.Parent as Panel;
            OriginalMargin = attachedElement.Margin;
            OriginalSize = new Size(attachedElement.Width, attachedElement.Height);
            double left = Canvas.GetLeft(attachedElement);
            double right = Canvas.GetRight(attachedElement);
            double top = Canvas.GetTop(attachedElement);
            double bottom = Canvas.GetBottom(attachedElement);
            int z = Canvas.GetZIndex(attachedElement);
            VerticalAlignment va = attachedElement.VerticalAlignment;
            HorizontalAlignment ha = attachedElement.HorizontalAlignment;

            #region Setting Container Properties
            RotatorParent = new Planerator();
            RotatorParent.Margin = OriginalMargin;
            RotatorParent.Width = OriginalSize.Width;
            RotatorParent.Height = OriginalSize.Height;
            RotatorParent.VerticalAlignment = va;
            RotatorParent.HorizontalAlignment = ha;
            RotatorParent.SetValue(Canvas.LeftProperty, left);
            RotatorParent.SetValue(Canvas.RightProperty, right);
            RotatorParent.SetValue(Canvas.TopProperty, top);
            RotatorParent.SetValue(Canvas.BottomProperty, bottom);
            RotatorParent.SetValue(Canvas.ZIndexProperty, z);
            #endregion

            #region Removing Child Properties
            OriginalPanel.Children.Remove(attachedElement);
            attachedElement.Margin = new Thickness();
            attachedElement.Width = double.NaN;
            attachedElement.Height = double.NaN;
            #endregion

            OriginalPanel.Children.Add(RotatorParent);
            RotatorParent.Child = attachedElement;

            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
        }

        public Planerator RotatorParent { get; set; }


        Point current = new Point(-99, -99);
        bool IsPressed = false;
        Int32 times = -1;
        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (KeepDragging)
            {
                current = Mouse.GetPosition(RotatorParent.Child);
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (current.X > 0 && current.X < (attachedElement as FrameworkElement).ActualWidth && current.Y > 0 && current.Y < (attachedElement as FrameworkElement).ActualHeight)
                    {
                        RotatorParent.RotationY = -1 * TiltFactor + current.X * 2 * TiltFactor / (attachedElement as FrameworkElement).ActualWidth;
                        RotatorParent.RotationX = -1 * TiltFactor + current.Y * 2 * TiltFactor / (attachedElement as FrameworkElement).ActualHeight;
                    }
                }
                else
                {
                    RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                    RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                }
            }
            else
            {

                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {

                    if (!IsPressed)
                    {
                        current = Mouse.GetPosition(RotatorParent.Child);
                        if (current.X > 0 && current.X < (attachedElement as FrameworkElement).ActualWidth && current.Y > 0 && current.Y < (attachedElement as FrameworkElement).ActualHeight)
                        {
                            RotatorParent.RotationY = -1 * TiltFactor + current.X * 2 * TiltFactor / (attachedElement as FrameworkElement).ActualWidth;
                            RotatorParent.RotationX = -1 * TiltFactor + current.Y * 2 * TiltFactor / (attachedElement as FrameworkElement).ActualHeight;
                        }
                        IsPressed = true;
                    }


                    if (IsPressed && times == 7)
                    {
                        RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                        RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                    }
                    else if (IsPressed && times < 7)
                    {
                        times++;
                    }
                }
                else
                {
                    IsPressed = false;
                    times = -1;
                    RotatorParent.RotationY = RotatorParent.RotationY - 5 < 0 ? 0 : RotatorParent.RotationY - 5;
                    RotatorParent.RotationX = RotatorParent.RotationX - 5 < 0 ? 0 : RotatorParent.RotationX - 5;
                }

            }
        }
    }
}