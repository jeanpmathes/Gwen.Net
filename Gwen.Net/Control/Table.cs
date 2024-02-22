using System;
using System.Linq;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Base class for multi-column tables.
    /// </summary>
    public class Table : ControlBase
    {
        private int[] columnWidth;
        
        /// <summary>
        /// For auto-sizing, if nonzero - fills last cell up to this size.
        /// </summary>
        private int maxWidth;
        
        private bool rowMeasurementDirty;
        private bool sizeToContents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Table" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Table(ControlBase parent) : base(parent)
        {
            columnWidth = new int[1];

            for (var i = 0; i < columnWidth.Length; i++)
            {
                columnWidth[i] = 20;
            }

            AutoSizeToContent = false;
            sizeToContents = false;
            rowMeasurementDirty = false;
        }

        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            base.OnBoundsChanged(oldBounds);
            
            rowMeasurementDirty = true;
        }

        /// <summary>
        ///     Column count (default 1).
        /// </summary>
        public int ColumnCount
        {
            get => columnWidth.Length;
            set
            {
                SetColumnCount(value);
                Invalidate();
            }
        }

        /// <summary>
        ///     Row count.
        /// </summary>
        public int RowCount => Children.Count;

        public bool AutoSizeToContent { get; set; }

        public bool AlternateColor { get; set; }

        /// <summary>
        ///     Returns specific row of the table.
        /// </summary>
        /// <param name="index">Row index.</param>
        /// <returns>Row at the specified index.</returns>
        public TableRow this[int index] => (TableRow) Children[index];

        protected override void OnChildAdded(ControlBase child)
        {
            if (child is not TableRow)
            {
                throw new ArgumentException("Table can only have TableRow children.");
            }
            
            base.OnChildAdded(child);
        }

        /// <summary>
        ///     Sets the number of columns.
        /// </summary>
        /// <param name="count">Number of columns.</param>
        public void SetColumnCount(int count)
        {
            if (ColumnCount == count)
                return;
            
            Array.Resize(ref columnWidth, count);

            foreach (TableRow row in Children.Cast<TableRow>())
            {
                row.ColumnCount = count;
            }
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            if (columnWidth[column] == width)
            {
                return;
            }

            columnWidth[column] = width;
            Invalidate();
        }

        /// <summary>
        ///     Gets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column width.</returns>
        public int GetColumnWidth(int column)
        {
            return columnWidth[column];
        }

        /// <summary>
        ///     Adds a new empty row.
        /// </summary>
        /// <returns>Newly created row.</returns>
        public TableRow AddRow()
        {
            TableRow row = new(this);
            row.ColumnCount = ColumnCount;
            
            rowMeasurementDirty = true;

            return row;
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="row">Row to add.</param>
        public void AddRow(TableRow row)
        {
            row.Parent = this;
            row.ColumnCount = ColumnCount;
            
            rowMeasurementDirty = true;
        }

        /// <summary>
        ///     Adds a new row with specified text in first column.
        /// </summary>
        /// <param name="text">Text to add.</param>
        /// <returns>New row.</returns>
        public TableRow AddRow(string text)
        {
            TableRow row = AddRow();
            
            row.SetCellText(columnIndex: 0, text);

            return row;
        }

        /// <summary>
        ///     Removes a row by reference.
        /// </summary>
        /// <param name="row">Row to remove.</param>
        public void RemoveRow(TableRow row)
        {
            RemoveChild(row, dispose: true);
            
            rowMeasurementDirty = true;
        }

        /// <summary>
        ///     Removes a row by index.
        /// </summary>
        /// <param name="idx">Row index.</param>
        public void RemoveRow(int idx)
        {
            var row = Children[idx] as TableRow;
            
            RemoveRow(row!);
        }

        /// <summary>
        ///     Removes all rows.
        /// </summary>
        public void RemoveAll()
        {
            while (RowCount > 0)
            {
                RemoveRow(idx: 0);
            }
        }

        /// <summary>
        ///     Gets the index of a specified row.
        /// </summary>
        /// <param name="row">Row to search for.</param>
        /// <returns>Row index if found, -1 otherwise.</returns>
        public int GetRowIndex(TableRow row)
        {
            return Children.IndexOf(row);
        }

        protected override Size Measure(Size availableSize)
        {
            if (rowMeasurementDirty && (AutoSizeToContent || sizeToContents))
            {
                sizeToContents = false;

                return DoSizeToContents(availableSize);
            }

            var height = 0;
            var width = 0;

            foreach (TableRow row in Children.Cast<TableRow>())
            {
                row.DoMeasure(availableSize);

                width = Math.Max(width, row.MeasuredSize.Width);
                height += row.MeasuredSize.Height;
            }

            return new Size(width, height);
        }

        protected override Size Arrange(Size finalSize)
        {
            var y = 0;
            var width = 0;
            var even = false;

            foreach (TableRow row in Children.Cast<TableRow>())
            {
                if (AlternateColor)
                {
                    row.EvenRow = even;
                    even = !even;
                }

                row.DoArrange(new Rectangle(x: 0, y, finalSize.Width, row.MeasuredSize.Height));
                width = Math.Max(width, row.MeasuredSize.Width);
                y += row.MeasuredSize.Height;
            }

            return new Size(finalSize.Width, y);
        }

        /// <summary>
        ///     Sizes to fit contents.
        /// </summary>
        public void SizeToContent(int newMaxWidth = 0)
        {
            maxWidth = newMaxWidth;
            sizeToContents = true;
            
            Invalidate();
        }
        
        private void DetermineColumnWidths(Size availableSize)
        {
            foreach (TableRow row in Children.Cast<TableRow>())
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    row.SetColumnWidth(column, Util.Ignore);
                }
                
                row.DoMeasure(availableSize);

                for (var column = 0; column < ColumnCount; column++)
                {
                    ControlBase? cell = row.GetColumn(column);
                    
                    if (cell == null) 
                        continue;

                    columnWidth[column] = Math.Max(columnWidth[column], cell.MeasuredSize.Width);
                }
            }
        }
        
        private void SetColumnWidths(Size availableSize, out int width, out int height)
        {
            width = 0;
            height = 0;
            
            foreach (TableRow row in Children.Cast<TableRow>())
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    if (column < ColumnCount - 1 || maxWidth == 0)
                    {
                        row.SetColumnWidth(column, columnWidth[column]);
                    }
                    else
                    {
                        row.SetColumnWidth(column, columnWidth[column] + Math.Max(val1: 0, maxWidth - width));
                    }
                }

                row.DoMeasure(availableSize);

                width = Math.Max(width, row.MeasuredSize.Width);
                height += row.MeasuredSize.Height;
            }
        }
        
        protected Size DoSizeToContents(Size availableSize)
        {
            Array.Fill(columnWidth, value: 0);

            DetermineColumnWidths(availableSize);
            SetColumnWidths(availableSize, out int width, out int height);

            rowMeasurementDirty = false;

            width = maxWidth == 0 ? width : Math.Max(width, maxWidth);

            return new Size(width, height);
        }
    }
}
