using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Media;

namespace Torshify.Client.Infrastructure.Controls
{
    public class BlockBarRect : BlockBarBase
    {
        #region Constructors

        public BlockBarRect()
        {
            FlowDirection = FlowDirection.RightToLeft;
        }

        #endregion Constructors

        #region Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect rect;
            int blockCount = BlockCount;
            Size renderSize = this.RenderSize;
            double blockMargin = this.BlockMargin;
            double value = Math.Max(0, 1 - Value);
            for (int i = 0; i < blockCount; i++)
            {
                rect = GetRect(renderSize, blockCount, blockMargin, i, BorderBen.Thickness);

                if (!rect.IsEmpty)
                {
                    int threshold = GetThreshold(value, blockCount);
                    drawingContext.DrawRectangle((i < threshold) ? Foreground : Background, BorderBen, rect);
                }
            }
        }

        private static Rect GetRect(Size targetSize, int blockCount, double blockMargin, int blockNumber, double penThickness)
        {
            Contract.Requires(!targetSize.IsEmpty, "targetSize");
            Contract.Requires(blockCount > 0, "blockCount");
            Contract.Requires(blockCount > blockNumber, "blockNumber");

            double width = (targetSize.Width - (blockCount - 1) * blockMargin - penThickness) / blockCount;
            double left = penThickness / 2 + (width + blockMargin) * blockNumber;
            double height = targetSize.Height - penThickness;

            if (width > 0 && height > 0)
            {
                return new Rect(left, penThickness / 2, width, height);
            }
            else
            {
                return Rect.Empty;
            }
        }

        #endregion Methods
    }
}