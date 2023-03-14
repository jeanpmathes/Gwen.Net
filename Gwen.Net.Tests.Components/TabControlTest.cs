using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Containers", Order = 304)]
    public class TabControlTest : GUnit
    {
        private readonly TabControl dockControl;

        public TabControlTest(ControlBase parent)
            : base(parent)
        {
            {
                dockControl = new TabControl(this);
                dockControl.Margin = Margin.Zero;
                dockControl.Width = 200;
                dockControl.Dock = Dock.Top;

                {
                    TabButton button = dockControl.AddPage("Controls");
                    ControlBase page = button.Page;

                    {
                        GroupBox group = new(page);
                        group.Text = "Tab position";
                        RadioButtonGroup radio = new(group);

                        radio.AddOption("Top").Select();
                        radio.AddOption("Bottom");
                        radio.AddOption("Left");
                        radio.AddOption("Right");

                        radio.SelectionChanged += OnDockChange;
                    }
                }

                dockControl.AddPage("Red");
                dockControl.AddPage("Green");
                dockControl.AddPage("Blue");
                dockControl.AddPage("Blue");
                dockControl.AddPage("Blue");
            }

            {
                TabControl dragMe = new(this);
                dragMe.Margin = Margin.Five;
                dragMe.Width = 200;
                dragMe.Dock = Dock.Top;

                dragMe.AddPage("You");
                dragMe.AddPage("Can");
                dragMe.AddPage("Reorder").SetImage("test16.png");
                dragMe.AddPage("These");
                dragMe.AddPage("Tabs");

                dragMe.AllowReorder = true;
            }
        }

        private void OnDockChange(ControlBase control, EventArgs args)
        {
            var rc = (RadioButtonGroup) control;

            if (rc.SelectedLabel == "Top")
            {
                dockControl.TabStripPosition = Dock.Top;
            }

            if (rc.SelectedLabel == "Bottom")
            {
                dockControl.TabStripPosition = Dock.Bottom;
            }

            if (rc.SelectedLabel == "Left")
            {
                dockControl.TabStripPosition = Dock.Left;
            }

            if (rc.SelectedLabel == "Right")
            {
                dockControl.TabStripPosition = Dock.Right;
            }
        }
    }
}
