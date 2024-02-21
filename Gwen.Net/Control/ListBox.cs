using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Gwen.Net.Input;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     ListBox control.
    /// </summary>
    public class ListBox : ScrollControl
    {
        private readonly List<ListBoxRow> selectedRows;
        private readonly Table table;

        private bool multiSelect;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ListBox(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.One;

            selectedRows = new List<ListBoxRow>();

            MouseInputEnabled = true;
            EnableScroll(horizontal: false, vertical: true);
            AutoHideBars = true;

            table = new Table(this);
            table.AutoSizeToContent = true;
            table.ColumnCount = 1;

            multiSelect = false;
            IsToggle = false;
        }

        /// <summary>
        ///     Determines whether multiple rows can be selected at once.
        /// </summary>
        public bool AllowMultiSelect
        {
            get => multiSelect;
            set
            {
                multiSelect = value;

                if (value)
                {
                    IsToggle = true;
                }
            }
        }
        
        /// <summary>
        /// Whether to require the shift key to be held down to multi-select.
        /// </summary>
        public bool RequireShiftToMultiSelect { get; set; }

        public bool AlternateColor
        {
            get => table.AlternateColor;
            set => table.AlternateColor = value;
        }

        /// <summary>
        ///     Determines whether rows can be unselected by clicking on them again.
        /// </summary>
        public bool IsToggle { get; set; }

        /// <summary>
        ///     Number of rows in the list box.
        /// </summary>
        public int RowCount => table.RowCount;

        /// <summary>
        ///     Returns specific row of the ListBox.
        /// </summary>
        /// <param name="index">Row index.</param>
        /// <returns>Row at the specified index.</returns>
        public ListBoxRow this[int index] => table[index] as ListBoxRow;

        /// <summary>
        ///     List of selected rows.
        /// </summary>
        public IEnumerable<TableRow> SelectedRows => selectedRows;

        /// <summary>
        ///     First selected row (and only if list is not multiselectable).
        /// </summary>
        public ListBoxRow SelectedRow
        {
            get
            {
                if (selectedRows.Count == 0)
                {
                    return null;
                }

                return selectedRows[index: 0];
            }
            set
            {
                if (table.Children.Contains(value))
                {
                    if (AllowMultiSelect)
                    {
                        SelectRow(value);
                    }
                    else
                    {
                        SelectRow(value, clearOthers: true);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the selected row number.
        /// </summary>
        public int SelectedRowIndex
        {
            get
            {
                ListBoxRow selected = SelectedRow;

                if (selected == null)
                {
                    return -1;
                }

                return table.GetRowIndex(selected);
            }
            set => SelectRow(value);
        }

        /// <summary>
        ///     Column count of table rows.
        /// </summary>
        public int ColumnCount
        {
            get => table.ColumnCount;
            set
            {
                table.ColumnCount = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Invoked when a row has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs<ListBoxRow>> RowSelected;

        /// <summary>
        ///     Invoked when a row has been unselected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs<ListBoxRow>> RowUnselected;

        /// <summary>
        ///     Invoked when a row has been double clicked.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs<ListBoxRow>> RowDoubleClicked;

        /// <summary>
        ///     Selects the specified row(s) by text.
        /// </summary>
        /// <param name="rowText">Text to search for (exact match).</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRows(string rowText, bool clearOthers = false)
        {
            IEnumerable<ListBoxRow> rows = table.Children.OfType<ListBoxRow>().Where(x => x.Text == rowText);

            foreach (ListBoxRow row in rows)
            {
                SelectRow(row, clearOthers);
            }
        }

        /// <summary>
        ///     Selects the specified row(s) by regex text search.
        /// </summary>
        /// <param name="pattern">Regex pattern to search for.</param>
        /// <param name="regexOptions">Regex options.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRowsByRegex(string pattern, 
            RegexOptions regexOptions = RegexOptions.None,
            bool clearOthers = false)
        {
            IEnumerable<ListBoxRow> rows = table.Children.OfType<ListBoxRow>()
                .Where(x => Regex.IsMatch(x.Text, pattern, regexOptions));

            foreach (ListBoxRow row in rows)
            {
                SelectRow(row, clearOthers);
            }
        }
        
        /// <summary>
        ///     Selects the specified row by index.
        /// </summary>
        /// <param name="index">Row to select.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRow(int index, bool clearOthers = false)
        {
            if (index < 0 || index >= table.RowCount)
            {
                return;
            }

            SelectRow(table.Children[index], clearOthers);
        }

        /// <summary>
        ///     Selects the specified row.
        /// </summary>
        /// <param name="control">Row to select.</param>
        /// <param name="clearOthers">Determines whether to deselect previously selected rows.</param>
        public void SelectRow(ControlBase control, bool clearOthers = false)
        {
            if (control is not ListBoxRow row)
            {
                throw new ArgumentException("Invalid control type, expected ListBoxRow", nameof(control));
            }
            
            if (control.Parent != table)
            {
                throw new ArgumentException("Control is not a child of this ListBox", nameof(control));
            }
            
            if (!AllowMultiSelect || clearOthers)
            {
                UnselectAll();
            }
            
            row.IsSelected = true;
            selectedRows.Add(row);

            RowSelected?.Invoke(this, new ItemSelectedEventArgs<ListBoxRow>(row));
        }

        /// <summary>
        ///     Removes the all rows from the ListBox
        /// </summary>
        public void RemoveAllRows()
        {
            table.DeleteAllChildren();
        }

        /// <summary>
        ///     Removes the specified row by index.
        /// </summary>
        /// <param name="idx">Row index.</param>
        public void RemoveRow(int idx)
        {
            // This will call Dispose() on the row.
            table.RemoveRow(idx);
        }

        /// <summary>
        ///     Adds a new row.
        /// </summary>
        /// <param name="label">Row text.</param>
        /// <param name="name">Internal control name.</param>
        /// <param name="userData">User data for newly created row</param>
        /// <returns>Newly created control.</returns>
        public ListBoxRow AddRow(string label, string name = "", object userData = null)
        {
            ListBoxRow row = new(this);
            table.AddRow(row);

            row.SetCellText(columnIndex: 0, label);
            row.Name = name;
            row.UserData = userData;

            row.Selected += OnRowSelected;
            row.DoubleClicked += OnRowDoubleClicked;

            Invalidate();

            return row;
        }

        /// <summary>
        ///     Add row.
        /// </summary>
        /// <param name="row">Row.</param>
        public void AddRow(ListBoxRow row)
        {
            row.Parent = this;

            table.AddRow(row);

            row.Selected += OnRowSelected;
            row.DoubleClicked += OnRowDoubleClicked;

            Invalidate();
        }

        /// <summary>
        ///     Sets the column width (in pixels).
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="width">Column width.</param>
        public void SetColumnWidth(int column, int width)
        {
            table.SetColumnWidth(column, width);
            Invalidate();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawListBox(this);
        }

        /// <summary>
        ///     Deselects all rows.
        /// </summary>
        public virtual void UnselectAll()
        {
            foreach (ListBoxRow row in selectedRows)
            {
                row.IsSelected = false;

                RowUnselected?.Invoke(this, new ItemSelectedEventArgs<ListBoxRow>(row));
            }

            selectedRows.Clear();
        }

        /// <summary>
        ///     Unselects the specified row.
        /// </summary>
        /// <param name="row">Row to unselect.</param>
        public void UnselectRow(ListBoxRow row)
        {
            row.IsSelected = false;
            selectedRows.Remove(row);

            RowUnselected?.Invoke(this, new ItemSelectedEventArgs<ListBoxRow>(row));
        }

        /// <summary>
        ///     Handler for the row selection event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnRowSelected(ControlBase control, ItemSelectedEventArgs args)
        {
            bool clear = RequireShiftToMultiSelect && !InputHandler.IsShiftDown;

            if (args.SelectedItem is not ListBoxRow row)
            {
                return;
            }

            if (row.IsSelected)
            {
                if (IsToggle)
                {
                    UnselectRow(row);
                }
            }
            else
            {
                SelectRow(row, clear);
            }
        }

        /// <summary>
        ///     Handler for the row double click event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnRowDoubleClicked(ControlBase control, ClickedEventArgs args)
        {
            if (control is not ListBoxRow row)
                return;

            RowDoubleClicked?.Invoke(this, new ItemSelectedEventArgs<ListBoxRow>(row));
        }

        /// <summary>
        ///     Removes all rows.
        /// </summary>
        public virtual void Clear()
        {
            UnselectAll();
            table.RemoveAll();
            Invalidate();
        }

        /// <summary>
        ///     Selects the first menu item with the given text it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="text">The label to look for, this is what is shown to the user.</param>
        public void SelectByText(string text)
        {
            foreach (ListBoxRow item in table.Children.Cast<ListBoxRow>())
            {
                if (item.Text != text) continue;

                SelectedRow = item;

                return;
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given internal name it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="name">The internal name to look for. To select by what is displayed to the user, use "SelectByText".</param>
        public void SelectByName(string name)
        {
            foreach (ListBoxRow item in table.Children.Cast<ListBoxRow>())
            {
                if (item.Name != name) continue;

                SelectedRow = item;

                return;
            }
        }

        /// <summary>
        ///     Selects the first menu item with the given user data it finds.
        ///     If a menu item can not be found that matches input, nothing happens.
        /// </summary>
        /// <param name="userdata">
        ///     The UserData to look for. The equivalency check uses "param.Equals(item.UserData)".
        ///     If null is passed in, it will look for null/unset UserData.
        /// </param>
        public void SelectByUserData(object userdata)
        {
            foreach (ListBoxRow item in table.Children.Cast<ListBoxRow>())
            {
                if (userdata == null)
                {
                    if (item.UserData != null) continue;

                    SelectedRow = item;

                    return;
                }

                if (!userdata.Equals(item.UserData)) continue;

                SelectedRow = item;

                return;
            }
        }
    }
}
