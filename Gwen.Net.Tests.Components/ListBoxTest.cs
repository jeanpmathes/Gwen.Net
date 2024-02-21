using System;
using System.Linq;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Standard", Order = 205)]
    public class ListBoxTest : GUnit
    {
        public ListBoxTest(ControlBase parent)
            : base(parent)
        {
            HorizontalLayout hLayout = new(this);
            hLayout.Dock = Dock.Top;

            {
                ListBox ctrl = new(hLayout);
                ctrl.AutoSizeToContent = true;
                ctrl.AllowMultiSelect = true;

                ctrl.AddRow("First");
                ctrl.AddRow("Blue");
                ctrl.AddRow("Yellow");
                ctrl.AddRow("Orange");
                ctrl.AddRow("Brown");
                ctrl.AddRow("Black");
                ctrl.AddRow("Green");
                ctrl.AddRow("Dog");
                ctrl.AddRow("Cat Blue");
                ctrl.AddRow("Shoes");
                ctrl.AddRow("Shirts");
                ctrl.AddRow("Chair");
                ctrl.AddRow("I'm auto-sized");
                ctrl.AddRow("Last");

                ctrl.SelectRowsByRegex("Bl.e|Dog");

                ctrl.RowSelected += RowSelected;
                ctrl.RowUnselected += RowUnSelected;
            }

            {
                Table ctrl = new(hLayout);

                ctrl.AddRow("First");
                ctrl.AddRow("Blue");
                ctrl.AddRow("Yellow");
                ctrl.AddRow("Orange");
                ctrl.AddRow("Brown");
                ctrl.AddRow("Black");
                ctrl.AddRow("Green");
                ctrl.AddRow("Dog");
                ctrl.AddRow("Cat Blue");
                ctrl.AddRow("Shoes");
                ctrl.AddRow("Shirts");
                ctrl.AddRow("Chair");
                ctrl.AddRow("I'm auto-sized");
                ctrl.AddRow("Last");

                ctrl.SizeToContent();
            }

            {
                ListBox ctrl = new(hLayout);
                ctrl.AutoSizeToContent = true;
                ctrl.ColumnCount = 3;
                ctrl.RowSelected += RowSelected;
                ctrl.RowUnselected += RowUnSelected;

                {
                    TableRow row = ctrl.AddRow("Baked Beans");
                    row.SetCellText(columnIndex: 1, "Heinz");
                    row.SetCellText(columnIndex: 2, "£3.50");
                }

                {
                    TableRow row = ctrl.AddRow("Bananas");
                    row.SetCellText(columnIndex: 1, "Trees");
                    row.SetCellText(columnIndex: 2, "£1.27");
                }

                {
                    TableRow row = ctrl.AddRow("Chicken");
                    row.SetCellText(columnIndex: 1, "\u5355\u5143\u6D4B\u8BD5");
                    row.SetCellText(columnIndex: 2, "£8.95");
                }

                {
                    TableRow row = ctrl.AddRow("Alignment!");
                    
                    row.SetCellText(columnIndex: 0, "Left", Alignment.Left);
                    row.SetCellText(columnIndex: 1, "Center", Alignment.Center);
                    row.SetCellText(columnIndex: 2, "Right", Alignment.Right);
                }

                {
                    Font small = new(Skin.Renderer, "Arial", size: 5);
                    
                    TableRow row = ctrl.AddRow("Small Font");
                    
                    row.SetCellText(columnIndex: 0, "Normal");
                    // Use big font for first cell to make the row larger.
                    
                    row.SetCellText(columnIndex: 1, "Top", Alignment.Top);
                    row.SetCellFont(columnIndex: 1, small);
                    
                    row.SetCellText(columnIndex: 2, "Bottom", Alignment.Bottom);
                    row.SetCellFont(columnIndex: 2, small);
                }
            }

            VerticalLayout vLayout = new(hLayout);

            {
                // fixed-size list box
                ListBox ctrl = new(vLayout);
                ctrl.AutoSizeToContent = true;
                ctrl.HorizontalAlignment = HorizontalAlignment.Left;
                ctrl.ColumnCount = 3;

                ctrl.SetColumnWidth(column: 0, width: 150);
                ctrl.SetColumnWidth(column: 1, width: 150);
                ctrl.SetColumnWidth(column: 2, width: 150);

                ListBoxRow row1 = ctrl.AddRow("Row 1");
                row1.SetCellText(columnIndex: 1, "R1 cell 1");
                row1.SetCellText(columnIndex: 2, "Row 1 cell 2");

                ctrl.AddRow("Row 2, slightly bigger");
                ctrl[index: 1].SetCellText(columnIndex: 1, "Center cell");

                ctrl.AddRow("Row 3, medium");
                ctrl[index: 2].SetCellText(columnIndex: 2, "Last cell");
            }

            {
                ListBox ctrl = new(vLayout);
                ctrl.AutoSizeToContent = true;
                ctrl.HorizontalAlignment = HorizontalAlignment.Left;
                ctrl.ColumnCount = 3;

                ListBoxRow row1 = ctrl.AddRow("Row 1");
                row1.SetCellText(columnIndex: 1, "R1 cell 1");
                row1.SetCellText(columnIndex: 2, "Row 1 cell 2");

                ctrl.AddRow("Row 2, slightly bigger");
                ctrl[index: 1].SetCellText(columnIndex: 1, "Center cell");

                ctrl.AddRow("Row 3, medium");
                ctrl[index: 2].SetCellText(columnIndex: 2, "Last cell");
            }

            hLayout = new HorizontalLayout(this);
            hLayout.Dock = Dock.Top;

            /* Selecting Rows in Code */
            {
                ListBox ctrl = new(hLayout);
                ctrl.AutoSizeToContent = true;

                ListBoxRow row = ctrl.AddRow("Row");
                ctrl.AddRow("Text");
                ctrl.AddRow("InternalName", "Name");
                ctrl.AddRow("UserData", "Internal", userData: 12);

                LabeledCheckBox multiline = new(this);
                multiline.Dock = Dock.Top;
                multiline.Text = "Enable MultiSelect";

                multiline.CheckChanged += delegate { ctrl.AllowMultiSelect = multiline.IsChecked; };

                vLayout = new VerticalLayout(hLayout);

                //Select by Menu Item
                {
                    Button triangleButton = new(vLayout);
                    triangleButton.Text = "Row";
                    triangleButton.Width = 100;

                    triangleButton.Clicked += delegate { ctrl.SelectedRow = row; };
                }

                //Select by Text
                {
                    Button testButton = new(vLayout);
                    testButton.Text = "Text";
                    testButton.Width = 100;

                    testButton.Clicked += delegate { ctrl.SelectByText("Text"); };
                }

                //Select by Name
                {
                    Button testButton = new(vLayout);
                    testButton.Text = "Name";
                    testButton.Width = 100;

                    testButton.Clicked += delegate { ctrl.SelectByName("Name"); };
                }

                //Select by UserData
                {
                    Button testButton = new(vLayout);
                    testButton.Text = "UserData";
                    testButton.Width = 100;

                    testButton.Clicked += delegate { ctrl.SelectByUserData(userdata: 12); };
                }
            }
        }

        private void RowSelected(ControlBase control, EventArgs args)
        {
            var list = (ListBox) control;

            UnitPrint(
                $"ListBox: RowSelected: {list.SelectedRows.Last().Text} [{list[list.SelectedRowIndex].Text}]");
        }

        private void RowUnSelected(ControlBase control, EventArgs args)
        {
            // todo: how to determine which one was unselected (store somewhere)
            // or pass row as the event param?
            UnitPrint("ListBox: OnRowUnselected");
        }
    }
}
