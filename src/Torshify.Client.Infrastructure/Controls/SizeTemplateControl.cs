using System;
using System.Windows;
using System.Windows.Controls;

namespace Torshify.Client.Infrastructure.Controls
{
    /// <summary>
    /// Control that allows a collection of ControlTemplates to be associated with it, to be applied based on size, and selects an
    /// appropriate template based on its layout size.
    /// </summary>
    public class SizeTemplateControl : Control
    {
        #region Fields

        /// <summary>
        /// DependencyProperty for <see cref="Templates" /> property.
        /// </summary>
        public static readonly DependencyProperty TemplatesProperty = 
            DependencyProperty.Register(
                "Templates",
                typeof(SizeControlTemplateCollection),
                typeof(SizeTemplateControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Default height to be used for constraint size if size passed at measure is infinity.
        /// </summary>
        private const double DefaultConstraintHeight = 600;

        /// <summary>
        /// Default width  to be used for constraint size if size passed at measure is infinity.
        /// </summary>
        private const double DefaultConstraintWidth = 480;

        #endregion Fields

        #region Constructors

        public SizeTemplateControl()
        {
            Templates = new SizeControlTemplateCollection();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the collection of SizeControlTemplate objects that provide ControlTemplate selectable for a given Control’s size.
        /// </summary>
        public SizeControlTemplateCollection Templates
        {
            get { return (SizeControlTemplateCollection)GetValue(TemplatesProperty); }
            set { SetValue(TemplatesProperty, value); }
        }

        #endregion Properties

        #region Protected Methods

        /// <summary>
        /// Update current control's tempate based on the size and control’s properties.
        /// </summary>
        /// <param name="size">The size of the current template.</param>
        protected virtual void UpdateCurrentTemplate(Size size)
        {
            ControlTemplate newTemplate = this.SelectTemplate(this.Templates, size);
            if (newTemplate != this.Template)
            {
                this.Template = newTemplate;
            }
        }

        /// <summary>
        /// Select appropriate ControlTemplate for a given size.
        /// </summary>
        /// <param name="templates">The collection of SizeControlTemplate objects.</param>
        /// <param name="size">The size used to determine the current control's template.</param>
        /// <returns>Selected ControlTemplate.</returns>
        protected virtual ControlTemplate SelectTemplate(SizeControlTemplateCollection templates, Size size)
        {
            ControlTemplate template = null;
            if (templates != null && templates.Count > 0)
            {
                for (int i = 0; i < templates.Count; i++)
                {
                    if (templates[i].IsSelectable(size))
                    {
                        template = templates[i].Template;
                        break;
                    }
                }
            }

            return template;
        }

        /// <summary>
        /// Override for logic that determines size required by this object.
        /// </summary>
        /// <param name="availableSize">Available size.</param>
        /// <returns>Desired size for this object.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            // If measure constraint is infinity, snap it to default size
            Size newConstraint = this.GetValidConstraint(availableSize);

            Size desiredSize = base.MeasureOverride(newConstraint);
            return desiredSize;
        }

        /// <summary>
        /// Infinity is not a valid size for SizeTemplateControl.
        /// </summary>
        /// <param name="constraint">The possibly invalid size.</param>
        /// <returns>A Size that is less than infinity.</returns>
        protected virtual Size GetValidConstraint(Size constraint)
        {
            Size size = Size.Empty;
            if (constraint != Size.Empty)
            {
                double height = Double.IsInfinity(constraint.Height) ? DefaultConstraintHeight : constraint.Height;
                double width = Double.IsInfinity(constraint.Width) ? DefaultConstraintWidth : constraint.Width;
                size = new Size(width, height);
            }

            return size;
        }

        /// <summary>
        /// Positions child elements and determines a size for this object. 
        /// <see cref="FrameworkElement.ArrangeOverride"/>
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.UpdateCurrentTemplate(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        #endregion Protected Methods
    }
}