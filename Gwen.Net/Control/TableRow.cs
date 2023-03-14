using System;
using Gwen.Net.Platform;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Single table row.
    /// </summary>
    public class TableRow : ControlBase
    {
        // [omeg] todo: get rid of this
        public const int MaxColumns = 5;
        private readonly Label[] columns;

        private int columnCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TableRow(ControlBase parent)
            : base(parent)
        {
            columns = new Label[MaxColumns];

            columnCount = parent switch
            {
                ListBox listBox => listBox.ColumnCount,
                Table table => table.ColumnCount,
                _ => columnCount
            };

            KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Column count.
        /// </summary>
        public int ColumnCount
        {
            get => columnCount;
            set => SetColumnCount(value);
        }

        /// <summary>
        ///     Indicates whether the row is even or odd (used for alternate coloring).
        /// </summary>
        public bool EvenRow { get; set; }

        /// <summary>
        ///     Text of the first column.
        /// </summary>
        public string Text
        {
            get => GetText();
            set => SetCellText(columnIndex: 0, value);
        }

        internal Label GetColumn(int index)
        {
            return columns[index];
        }

        /// <summary>
        ///     Invoked when the row has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> Selected;

        /// <summary>
        ///     Sets the number of columns.
        /// </summary>
        /// <param name="newColumnCount">Number of columns.</param>
        protected void SetColumnCount(int newColumnCount)
        {
            if (newColumnCount == columnCount)
            {
                return;
            }

            if (newColumnCount >= MaxColumns)
            {
                throw new ArgumentException("Invalid column count", nameof(newColumnCount));
            }

            for (var i = 0; i < MaxColumns; i++)
            {
                if (i < newColumnCount)
                {
                    if (columns[i] != null) continue;

                    columns[i] = new Label(this);
                    columns[i].Padding = Padding.Three;

                    columns[i].Margin = new Margin(
                        left: 0,
                        top: 0,
                        right: 2,
                        bottom: 0); // to separate them slightly

                    columns[i].TextColor = Skin.colors.listBoxColors.textNormal;
                }
                else if (null != columns[i])
                {
                    RemoveChild(columns[i], dispose: true);
                    columns[i] = null;
                }
            }

            columnCount = newColumnCount;
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            if (null == columns[column])
            {
                return;
            }

            if (columns[column].Width == width)
            {
                return;
            }

            columns[column].Width = width;
        }

        /// <summary>
        ///     Sets the text of a specified cell.
        /// </summary>
        /// <param name="columnIndex">Column number.</param>
        /// <param name="text">Text to set.</param>
        public void SetCellText(int columnIndex, string text)
        {
            if (null == columns[columnIndex])
            {
                columns[columnIndex] = new Label(this);
                columns[columnIndex].Padding = Padding.Three;

                columns[columnIndex].Margin =
                    new Margin(left: 0, top: 0, right: 2, bottom: 0); // to separate them slightly

                columns[columnIndex].TextColor = Skin.colors.listBoxColors.textNormal;
            }

            if (columnIndex >= columnCount)
            {
                throw new ArgumentException("Invalid column index", nameof(columnIndex));
            }

            columns[columnIndex].Text = text;
        }

        /// <summary>
        ///     Sets the font of a specified cell.
        /// </summary>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="font">The font.</param>
        public void SetCellFont(int columnIndex, Font font)
        {
            columns[columnIndex].Font = font;
        }
        
        /// <summary>
        ///     Sets the contents of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="control">Cell contents.</param>
        /// <param name="enableMouseInput">Determines whether mouse input should be enabled for the cell.</param>
        public void SetCellContents(int column, ControlBase control, bool enableMouseInput = false)
        {
            if (null == columns[column])
            {
                return;
            }

            control.Parent = columns[column];
            columns[column].MouseInputEnabled = enableMouseInput;
        }

        /// <summary>
        ///     Gets the contents of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <returns>Control embedded in the cell.</returns>
        public ControlBase GetCellContents(int column)
        {
            return columns[column];
        }

        protected virtual void OnRowSelected()
        {
            if (Selected != null)
            {
                Selected.Invoke(this, new ItemSelectedEventArgs(this));
            }
        }

        protected override Size Measure(Size availableSize)
        {
            var width = 0;
            var height = 0;

            for (var i = 0; i < columnCount; i++)
            {
                if (null == columns[i])
                {
                    continue;
                }

                Size size = columns[i].DoMeasure(new Size(availableSize.Width - width, availableSize.Height));

                width += size.Width;
                height = Math.Max(height, size.Height);
            }

            return new Size(width, height);
        }

        protected override Size Arrange(Size finalSize)
        {
            var x = 0;
            var height = 0;

            for (var i = 0; i < columnCount; i++)
            {
                if (null == columns[i])
                {
                    continue;
                }

                if (i == columnCount - 1)
                {
                    columns[i].DoArrange(
                        new Rectangle(x, y: 0, finalSize.Width - x, columns[i].MeasuredSize.Height));
                }
                else
                {
                    columns[i].DoArrange(
                        new Rectangle(x, y: 0, columns[i].MeasuredSize.Width, columns[i].MeasuredSize.Height));
                }

                x += columns[i].MeasuredSize.Width;
                height = Math.Max(height, columns[i].MeasuredSize.Height);
            }

            return new Size(finalSize.Width, height);
        }

        /// <summary>
        ///     Sets the text color for all cells.
        ///     The color is overridden by the selected color if the row is selected.
        /// </summary>
        /// <param name="color">Text color.</param>
        public void SetTextColor(Color color)
        {
            for (var i = 0; i < columnCount; i++)
            {
                if (null == columns[i])
                {
                    continue;
                }

                columns[i].TextColor = color;
            }
        }

        /// <summary>
        ///     Returns text of a specified row cell (default first).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column cell text.</returns>
        public string GetText(int column = 0)
        {
            return columns[column].Text;
        }

        /// <summary>
        ///     Handler for Copy event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected override void OnCopy(ControlBase from, EventArgs args)
        {
            GwenPlatform.SetClipboardText(Text);
        }
    }
}
