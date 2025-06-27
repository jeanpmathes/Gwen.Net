using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Gwen.Net.Control.Layout
{
    /// <summary>
    ///     GridLayout column widths or row heights.
    /// </summary>
    /// <remarks>
    ///     Cell size can be one of
    ///     a) Single.NaN: Auto sized. Size is the smallest size the control can be drawn.
    ///     b) 0.0 - 1.0: Remaining space filled proportionally.
    ///     c) More than 1.0: Absolute cell size.
    /// </remarks>
    public class GridCellSizes : List<Single>
    {
        public GridCellSizes(IEnumerable<Single> sizes)
            : base(sizes) {}

        public GridCellSizes(Int32 count)
            : base(count) {}

        public GridCellSizes(params Single[] sizes)
            : base(sizes) {}
    }

    /// <summary>
    ///     Arrange child controls into columns and rows by adding them in column and row order.
    ///     Add every column of the first row, then every column of the second row etc.
    /// </summary>
    public class GridLayout : ControlBase
    {
        public const Single AutoSize = Single.NaN;
        public const Single Fill = 1.0f;
        
        private Int32 columnCount;
        
        private Int32[] columnWidths;
        private Int32[] rowHeights;

        private Single[] requestedColumnWidths;
        private Single[] requestedRowHeights;
        
        private Size totalAutoFixedSize;
        private Size totalFixedSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GridLayout" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public GridLayout(ControlBase parent)
            : base(parent)
        {
            columnCount = 1;
        }

        /// <summary>
        ///     Number of columns. This can be used when all cells are auto size.
        /// </summary>
        public Int32 ColumnCount
        {
            get => columnCount;
            set
            {
                columnCount = value;
                Invalidate();
            }
        }

        private Boolean isColumnWidth100Percent;

        /// <summary>
        ///     Set column widths. <see cref="GridCellSizes" />
        /// </summary>
        /// <param name="widths">Array of widths.</param>
        public void SetColumnWidths(params Single[] widths)
        {
            totalFixedSize.Width = 0;
            var relTotalWidth = 0.0f;

            foreach (Single w in widths)
            {
                if (w is >= 0.0f and <= 1.0f)
                {
                    relTotalWidth += w;
                }
                else if (w > 1.0f)
                {
                    totalFixedSize.Width += (Int32) w;
                }
            }

            if (relTotalWidth > 1.0f)
            {
                throw new ArgumentException("Relative widths exceed total value of 1.0 (100%).");
            }
            
            isColumnWidth100Percent = Math.Abs(relTotalWidth - 1.0f) < 0.0001f;

            requestedColumnWidths = widths;
            columnCount = widths.Length;
            Invalidate();
        }
        
        private Boolean isRowHeight100Percent;

        /// <summary>
        ///     Set row heights. <see cref="GridCellSizes" />
        /// </summary>
        /// <param name="heights">Array of heights.</param>
        public void SetRowHeights(params Single[] heights)
        {
            totalFixedSize.Height = 0;
            var relTotalHeight = 0.0f;

            foreach (Single h in heights)
            {
                if (h is >= 0.0f and <= 1.0f)
                {
                    relTotalHeight += h;
                }
                else if (h > 1.0f)
                {
                    totalFixedSize.Height += (Int32) h;
                }
            }

            if (relTotalHeight > 1.0f)
            {
                throw new ArgumentException("Relative heights exceed total value of 1.0 (100%).");
            }

            isRowHeight100Percent = Math.Abs(relTotalHeight - 1.0f) < 0.0001f;

            requestedRowHeights = heights;
            Invalidate();
        }

        private static Int32[] GetSizeArray(Int32[] currentArray, Int32 requiredSize)
        {
            if (currentArray == null || currentArray.Length != requiredSize)
            {
                return new Int32[requiredSize];
            }
            
            Array.Fill(currentArray, value: 0);
            
            return currentArray;
        }

        protected override Size Measure(Size availableSize)
        {
            availableSize -= Padding;
            
            columnWidths = GetSizeArray(columnWidths, columnCount);

            Int32 rowCount = (Children.Count + columnCount - 1) / columnCount;
            rowHeights = GetSizeArray(rowHeights, rowCount);

            Size cellAvailableSize = availableSize;
            
            // We use this to ensure that relative sized cells are capable of filling all space.
            // Otherwise, rounding errors would lose some pixels.
            var usedWidthForRelativeSize = 0;
            var usedHeightForRelativeSize = 0;
            var currentHeightForRelativeSize = 0;
            
            var columnIndex = 0;
            var rowIndex = 0;

            foreach (ControlBase child in Children)
            {
                Size size;

                if (child.IsCollapsed)
                {
                    size = Size.Zero;
                }
                else
                {
                    size = cellAvailableSize;

                    if (requestedColumnWidths != null)
                    {
                        Single w = requestedColumnWidths[columnIndex];

                        if (w is >= 0.0f and <= 1.0f)
                        {
                            size.Width = (Int32) (w * (availableSize.Width - totalFixedSize.Width));
                            
                            if (isColumnWidth100Percent && columnIndex == columnCount - 1)
                            {
                                size.Width = availableSize.Width - usedWidthForRelativeSize;
                            }
                            
                            usedWidthForRelativeSize += size.Width;
                        }
                        else if (w > 1.0f)
                        {
                            size.Width = (Int32) w;
                        }
                    }

                    if (requestedRowHeights != null)
                    {
                        Single h = requestedRowHeights[rowIndex];

                        if (h is >= 0.0f and <= 1.0f)
                        {
                            size.Height = (Int32) (h * (availableSize.Height - totalFixedSize.Height));
                            
                            if (isRowHeight100Percent && rowIndex == rowCount - 1)
                            {
                                size.Height = availableSize.Height - usedHeightForRelativeSize;
                            }
                            
                            currentHeightForRelativeSize = Math.Max(currentHeightForRelativeSize, size.Height);
                        }
                        else if (h > 1.0f)
                        {
                            size.Height = (Int32) h;
                        }
                    }

                    size = child.DoMeasure(size);
                }
                
                columnWidths[columnIndex] = Math.Max(columnWidths[columnIndex], size.Width);
                rowHeights[rowIndex] = Math.Max(rowHeights[rowIndex], size.Height);

                cellAvailableSize.Width -= columnWidths[columnIndex];

                columnIndex++;

                if (columnIndex != columnCount) continue;

                cellAvailableSize.Width = availableSize.Width;
                cellAvailableSize.Height -= rowHeights[rowIndex];
                
                usedWidthForRelativeSize = 0;
                usedHeightForRelativeSize += currentHeightForRelativeSize;
                currentHeightForRelativeSize = 0;
                
                columnIndex = 0;
                rowIndex++;
            }

            totalAutoFixedSize = Size.Zero;

            var width = 0;

            for (columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                if (requestedColumnWidths != null)
                {
                    Single w = requestedColumnWidths[columnIndex];

                    if (w > 1.0f)
                    {
                        if (columnWidths[columnIndex] < w)
                        {
                            columnWidths[columnIndex] = (Int32) w;
                        }

                        totalAutoFixedSize.Width += columnWidths[columnIndex];
                    }
                    else if (Single.IsNaN(w))
                    {
                        totalAutoFixedSize.Width += columnWidths[columnIndex];
                    }
                }
                else
                {
                    totalAutoFixedSize.Width += columnWidths[columnIndex];
                }

                width += columnWidths[columnIndex];
            }

            var height = 0;

            for (rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                if (requestedRowHeights != null)
                {
                    Single h = requestedRowHeights[rowIndex];

                    if (h > 1.0f)
                    {
                        if (rowHeights[rowIndex] < h)
                        {
                            rowHeights[rowIndex] = (Int32) h;
                        }

                        totalAutoFixedSize.Height += rowHeights[rowIndex];
                    }
                    else if (Single.IsNaN(h))
                    {
                        totalAutoFixedSize.Height += rowHeights[rowIndex];
                    }
                }
                else
                {
                    totalAutoFixedSize.Height += rowHeights[rowIndex];
                }

                height += rowHeights[rowIndex];
            }

            return new Size(width, height) + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            Int32 y = Padding.Top;
            Int32 x = Padding.Left;
            var columnIndex = 0;
            var rowIndex = 0;

            foreach (ControlBase child in Children)
            {
                Int32 width = columnWidths[columnIndex];
                Int32 height = rowHeights[rowIndex];

                if (!child.IsCollapsed)
                {
                    if (requestedColumnWidths != null)
                    {
                        Single w = requestedColumnWidths[columnIndex];

                        if (w is >= 0.0f and <= 1.0f)
                        {
                            width = Math.Max(val1: 0, (Int32) (w * (finalSize.Width - totalAutoFixedSize.Width)));
                            
                            if (isColumnWidth100Percent && columnIndex == columnWidths.Length - 1)
                            {
                                width = finalSize.Width - x - Padding.Right;
                            }
                        }
                        else if (w > 1.0f)
                        {
                            width = (Int32) w;
                        }
                    }

                    if (requestedRowHeights != null)
                    {
                        Single h = requestedRowHeights[rowIndex];

                        if (h is >= 0.0f and <= 1.0f)
                        {
                            height = Math.Max(val1: 0, (Int32) (h * (finalSize.Height - totalAutoFixedSize.Height)));
                            
                            if (isRowHeight100Percent && rowIndex == rowHeights.Length - 1)
                            {
                                height = finalSize.Height - y - Padding.Bottom;
                            }
                        }
                        else if (h > 1.0f)
                        {
                            height = (Int32) h;
                        }
                    }

                    child.DoArrange(new Rectangle(x, y, width, height));
                }

                x += width;
                columnIndex++;

                if (columnIndex == columnCount)
                {
                    x = Padding.Left;
                    y += height;
                    columnIndex = 0;
                    rowIndex++;
                }
            }

            return finalSize;
        }
    }
}
