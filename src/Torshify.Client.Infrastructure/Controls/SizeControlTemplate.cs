using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Torshify.Client.Infrastructure.Controls
{
    /// <summary>
    /// A class that allows to specify a ControlTemplate which is dependent of the Control’s size.
    /// Basically Min/Max Width/Height values specify an area for which the ControlTemplate is applicable.
    /// </summary>
    [ContentProperty("Template")]
    public class SizeControlTemplate
    {
        #region Fields

        /// <summary>
        /// The ControlTemplate applicable for area defined by Min/Max Width/Height values.
        /// </summary>
        private ControlTemplate _controlTemplate;

        /// <summary>
        /// The minimum width for which ControlTemplate is selectable.
        /// </summary>
        private double _minWidth;

        /// <summary>
        /// The maximum width for which ControlTemplate is selectable.
        /// </summary>
        private double _maxWidth;

        /// <summary>
        /// The minimum height for which ControlTemplate is selectable.
        /// </summary>
        private double _minHeight;

        /// <summary>
        /// The maximum height for which ControlTemplate is selectable.
        /// </summary>
        private double _maxHeight;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor. Initially sets area to maximum available range.
        /// </summary>
        public SizeControlTemplate()
        {
            _maxWidth = Double.PositiveInfinity;
            _maxHeight = Double.PositiveInfinity;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the ControlTemplate applicable for area defined by Min/Max Width/Height values.
        /// </summary>
        public ControlTemplate Template
        {
            get { return _controlTemplate; }
            set { _controlTemplate = value; }
        }

        /// <summary>
        /// Gets or sets the minimum width for which ControlTemplate is selectable.
        /// </summary>
        public double MinWidth
        {
            get { return _minWidth; }
            set { _minWidth = value; }
        }

        /// <summary>
        /// Gets or sets the maximum width for which ControlTemplate is selectable.
        /// </summary>
        public double MaxWidth
        {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }

        /// <summary>
        /// Gets or sets the minimum height for which ControlTemplate is selectable.
        /// </summary>
        public double MinHeight
        {
            get { return _minHeight; }
            set { _minHeight = value; }
        }

        /// <summary>
        /// Gets or sets the maximum height for which ControlTemplate is selectable.
        /// </summary>
        public double MaxHeight
        {
            get { return _maxHeight; }
            set { _maxHeight = value; }
        }

        #endregion Properties

        #region Public Methods

        /// <summary>
        /// Return whether the ControlTemplate can be selected for the specified size.
        /// </summary>
        /// <param name="size">The size the SizeControlTemplate is tested against.</param>
        /// <returns>Whether the ControlTemplate can be selected for the specified size.</returns>
        public bool IsSelectable(Size size)
        {
            return (
                   	DoubleUtilities.LessThanOrClose(_minWidth, size.Width) &&
                   	DoubleUtilities.GreaterThanOrClose(_maxWidth, size.Width) &&
                   	DoubleUtilities.LessThanOrClose(_minHeight, size.Height) &&
                   	DoubleUtilities.GreaterThanOrClose(_maxHeight, size.Height));
        }

        #endregion Public Methods
    }
}