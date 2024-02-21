﻿using System;
using Gwen.Net.Platform;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Single table row.
    /// </summary>
    public class TableRow : ControlBase
    {
        private Label?[] columns;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TableRow(ControlBase parent)
            : base(parent)
        {
            int columnCount = parent switch
            {
                ListBox listBox => listBox.ColumnCount,
                Table table => table.ColumnCount,
                _ => 0
            };
            
            columns = new Label[columnCount];

            KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Column count.
        /// </summary>
        public int ColumnCount
        {
            get => columns.Length;
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

        internal Label? GetColumn(int index)
        {
            return columns[index];
        }

        /// <summary>
        ///     Invoked when the row has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs>? Selected;

        /// <summary>
        ///     Sets the number of columns.
        /// </summary>
        /// <param name="newColumnCount">Number of columns.</param>
        protected void SetColumnCount(int newColumnCount)
        {
            if (newColumnCount == ColumnCount)
                return;

            if (newColumnCount >= ColumnCount)
            {
                Array.Resize(ref columns, newColumnCount);
            }

            for (var column = 0; column < ColumnCount; column++)
            {
                if (column < newColumnCount)
                {
                    columns[column] ??= CreateLabel();
                }
                else if (columns[column] != null)
                {
                    RemoveChild(columns[column]!, dispose: true);
                    columns[column] = null;
                }
            }
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            Label? label = columns[column];
            
            if (label == null) return;
            if (label.Width == width) return;
            
            label.Width = width;
        }

        /// <summary>
        ///     Sets the text of a specified cell.
        /// </summary>
        /// <param name="columnIndex">Column number.</param>
        /// <param name="text">Text to set.</param>
        /// <param name="alignment">Text alignment.</param>
        public void SetCellText(int columnIndex, string text, Alignment alignment = Alignment.LeftTop)
        {
            columns[columnIndex] ??= CreateLabel();

            if (columnIndex >= ColumnCount)
            {
                throw new ArgumentException("Invalid column index", nameof(columnIndex));
            }
            
            columns[columnIndex]!.Text = text;
            columns[columnIndex]!.Alignment = alignment;
        }

        /// <summary>
        ///     Sets the font of a specified cell.
        /// </summary>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="font">The font.</param>
        public void SetCellFont(int columnIndex, Font font)
        {
            columns[columnIndex]!.Font = font;
        }
        
        /// <summary>
        ///     Sets the contents of a specified cell.
        /// </summary>
        /// <param name="column">Column number.</param>
        /// <param name="control">Cell contents.</param>
        /// <param name="enableMouseInput">Determines whether mouse input should be enabled for the cell.</param>
        public void SetCellContents(int column, ControlBase control, bool enableMouseInput = false)
        {
            if (columns[column] == null) return;

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
            Selected?.Invoke(this, new ItemSelectedEventArgs(this));
        }

        protected override Size Measure(Size availableSize)
        {
            var width = 0;
            var height = 0;

            for (var index = 0; index < ColumnCount; index++)
            {
                Label? column = columns[index];
                
                if (column == null)
                    continue;

                Size size = column.DoMeasure(new Size(availableSize.Width - width, availableSize.Height));

                width += size.Width;
                height = Math.Max(height, size.Height);
            }

            return new Size(width, height);
        }

        protected override Size Arrange(Size finalSize)
        {
            var x = 0;
            var height = 0;

            for (var i = 0; i < ColumnCount; i++)
            {
                Label? column = columns[i];
                
                if (column == null)
                    continue;

                column.DoArrange(i == ColumnCount - 1 
                    ? new Rectangle(x, y: 0, finalSize.Width - x, MeasuredSize.Height) 
                    : new Rectangle(x, y: 0, column.MeasuredSize.Width, MeasuredSize.Height));

                x += column.MeasuredSize.Width;
                height = Math.Max(height, column.MeasuredSize.Height);
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
            for (var i = 0; i < ColumnCount; i++)
            {
                Label? column = columns[i];
                
                if (column == null)
                    continue;
                
                column.TextColor = color;
            }
        }

        /// <summary>
        ///     Returns text of a specified row cell (default first).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns>Column cell text.</returns>
        public string GetText(int column = 0)
        {
            return columns[column]!.Text;
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

        private Label CreateLabel() =>
            new(this)
            {
                Padding = Padding.Three,
                Margin = new Margin(left: 0, top: 0, right: 2, bottom: 0), // To separate them slightly.
                TextColor = Skin.colors.listBoxColors.textNormal
            };
    }
}
