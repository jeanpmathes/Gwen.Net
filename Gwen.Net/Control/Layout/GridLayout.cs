using System;
using System.Collections.Generic;

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
    public class GridCellSizes : List<float>
    {
        public GridCellSizes(IEnumerable<float> sizes)
            : base(sizes) {}

        public GridCellSizes(int count)
            : base(count) {}

        public GridCellSizes(params float[] sizes)
            : base(sizes) {}
    }

    /// <summary>
    ///     Arrange child controls into columns and rows by adding them in column and row order.
    ///     Add every column of the first row, then every column of the second row etc.
    /// </summary>
    public class GridLayout : ControlBase
    {
        public const float AutoSize = float.NaN;
        public const float Fill = 1.0f;
        
        private int columnCount;
        
        private int[] columnWidths;
        private int[] rowHeights;

        private float[] requestedColumnWidths;
        private float[] requestedRowHeights;
        
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
        public int ColumnCount
        {
            get => columnCount;
            set
            {
                columnCount = value;
                Invalidate();
            }
        }

        private bool isColumnWidth100Percent;

        /// <summary>
        ///     Set column widths. <see cref="GridCellSizes" />
        /// </summary>
        /// <param name="widths">Array of widths.</param>
        public void SetColumnWidths(params float[] widths)
        {
            totalFixedSize.Width = 0;
            var relTotalWidth = 0.0f;

            foreach (float w in widths)
            {
                if (w is >= 0.0f and <= 1.0f)
                {
                    relTotalWidth += w;
                }
                else if (w > 1.0f)
                {
                    totalFixedSize.Width += (int) w;
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
        
        private bool isRowHeight100Percent;

        /// <summary>
        ///     Set row heights. <see cref="GridCellSizes" />
        /// </summary>
        /// <param name="heights">Array of heights.</param>
        public void SetRowHeights(params float[] heights)
        {
            totalFixedSize.Height = 0;
            var relTotalHeight = 0.0f;

            foreach (float h in heights)
            {
                if (h is >= 0.0f and <= 1.0f)
                {
                    relTotalHeight += h;
                }
                else if (h > 1.0f)
                {
                    totalFixedSize.Height += (int) h;
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

        protected override Size Measure(Size availableSize)
        {
            availableSize -= Padding;

            if (columnWidths == null || columnWidths.Length != columnCount)
            {
                columnWidths = new int[columnCount];
            }

            int rowCount = (Children.Count + columnCount - 1) / columnCount;

            if (rowHeights == null || rowHeights.Length != rowCount)
            {
                rowHeights = new int[rowCount];
            }

            int columnIndex;

            for (columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                columnWidths[columnIndex] = 0;
            }

            int rowIndex;

            for (rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                rowHeights[rowIndex] = 0;
            }

            Size cellAvailableSize = availableSize;
            Size usedRelativeSize = Size.Zero;
            columnIndex = 0;
            rowIndex = 0;

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
                        float w = requestedColumnWidths[columnIndex];

                        if (w is >= 0.0f and <= 1.0f)
                        {
                            size.Width = (int) (w * (availableSize.Width - totalFixedSize.Width));
                            
                            if (isColumnWidth100Percent && columnIndex == columnCount - 1)
                            {
                                size.Width = availableSize.Width - usedRelativeSize.Width;
                            }
                            
                            usedRelativeSize.Width += size.Width;
                        }
                        else if (w > 1.0f)
                        {
                            size.Width = (int) w;
                        }
                    }

                    if (requestedRowHeights != null)
                    {
                        float h = requestedRowHeights[rowIndex];

                        if (h is >= 0.0f and <= 1.0f)
                        {
                            size.Height = (int) (h * (availableSize.Height - totalFixedSize.Height));
                            
                            if (isRowHeight100Percent && rowIndex == rowCount - 1)
                            {
                                size.Height = availableSize.Height - usedRelativeSize.Height;
                            }
                            
                            usedRelativeSize.Height += size.Height;
                        }
                        else if (h > 1.0f)
                        {
                            size.Height = (int) h;
                        }
                    }

                    size = child.DoMeasure(size);
                }

                if (columnWidths[columnIndex] < size.Width)
                {
                    columnWidths[columnIndex] = size.Width;
                }

                if (rowHeights[rowIndex] < size.Height)
                {
                    rowHeights[rowIndex] = size.Height;
                }

                cellAvailableSize.Width -= columnWidths[columnIndex];

                columnIndex++;

                if (columnIndex != columnCount) continue;

                cellAvailableSize.Width = availableSize.Width;
                cellAvailableSize.Height -= rowHeights[rowIndex];
                
                usedRelativeSize.Width = 0;
                
                columnIndex = 0;
                rowIndex++;
            }

            totalAutoFixedSize = Size.Zero;

            var width = 0;

            for (columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                if (requestedColumnWidths != null)
                {
                    float w = requestedColumnWidths[columnIndex];

                    if (w > 1.0f)
                    {
                        if (columnWidths[columnIndex] < w)
                        {
                            columnWidths[columnIndex] = (int) w;
                        }

                        totalAutoFixedSize.Width += columnWidths[columnIndex];
                    }
                    else if (float.IsNaN(w))
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
                    float h = requestedRowHeights[rowIndex];

                    if (h > 1.0f)
                    {
                        if (rowHeights[rowIndex] < h)
                        {
                            rowHeights[rowIndex] = (int) h;
                        }

                        totalAutoFixedSize.Height += rowHeights[rowIndex];
                    }
                    else if (float.IsNaN(h))
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
            int y = Padding.Top;
            int x = Padding.Left;
            var columnIndex = 0;
            var rowIndex = 0;

            foreach (ControlBase child in Children)
            {
                int width = columnWidths[columnIndex];
                int height = rowHeights[rowIndex];

                if (!child.IsCollapsed)
                {
                    if (requestedColumnWidths != null)
                    {
                        float w = requestedColumnWidths[columnIndex];

                        if (w is >= 0.0f and <= 1.0f)
                        {
                            width = Math.Max(val1: 0, (int) (w * (finalSize.Width - totalAutoFixedSize.Width)));
                            
                            if (isColumnWidth100Percent && columnIndex == columnWidths.Length - 1)
                            {
                                width = finalSize.Width - x - Padding.Right;
                            }
                        }
                        else if (w > 1.0f)
                        {
                            width = (int) w;
                        }
                    }

                    if (requestedRowHeights != null)
                    {
                        float h = requestedRowHeights[rowIndex];

                        if (h is >= 0.0f and <= 1.0f)
                        {
                            height = Math.Max(val1: 0, (int) (h * (finalSize.Height - totalAutoFixedSize.Height)));
                            
                            if (isRowHeight100Percent && rowIndex == rowHeights.Length - 1)
                            {
                                height = finalSize.Height - y - Padding.Bottom;
                            }
                        }
                        else if (h > 1.0f)
                        {
                            height = (int) h;
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
