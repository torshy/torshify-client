using System.Windows;
using System.Windows.Controls;

namespace Torshify.Client.Infrastructure.Controls
{
    public class LayeredPanel : Panel
    {
        #region Protected Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement internalChild in InternalChildren)
            {
                internalChild.Measure(availableSize);
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement internalChild in InternalChildren)
            {
                internalChild.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }

            return base.ArrangeOverride(finalSize);
        }

        #endregion Protected Methods
    }
}