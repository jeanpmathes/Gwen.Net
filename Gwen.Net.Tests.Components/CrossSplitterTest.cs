using System;
using Gwen.Net.Control;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Layout", Order = 403)]
    public class CrossSplitterTest : GUnit
    {
        private int curZoom;
        private readonly CrossSplitter splitter;

        public CrossSplitterTest(ControlBase parent)
            : base(parent)
        {
            curZoom = 0;

            splitter = new CrossSplitter(this);
            splitter.Dock = Dock.Fill;

            {
                VerticalSplitter verticalSplitter = new(splitter);
                Button button1 = new(verticalSplitter);
                button1.Text = "Vertical left";
                Button button2 = new(verticalSplitter);
                button2.Text = "Vertical right";
                verticalSplitter.SetPanel(index: 0, button1);
                verticalSplitter.SetPanel(index: 1, button2);
                splitter.SetPanel(index: 0, verticalSplitter);
            }

            {
                HorizontalSplitter horizontalSplitter = new(splitter);
                Button button1 = new(horizontalSplitter);
                button1.Text = "Horizontal up";
                Button button2 = new(horizontalSplitter);
                button2.Text = "Horizontal down";
                horizontalSplitter.SetPanel(index: 0, button1);
                horizontalSplitter.SetPanel(index: 1, button2);
                splitter.SetPanel(index: 1, horizontalSplitter);
            }

            {
                HorizontalSplitter horizontalSplitter = new(splitter);
                Button button1 = new(horizontalSplitter);
                button1.Text = "Horizontal up";
                Button button2 = new(horizontalSplitter);
                button2.Text = "Horizontal down";
                horizontalSplitter.SetPanel(index: 0, button1);
                horizontalSplitter.SetPanel(index: 1, button2);
                splitter.SetPanel(index: 2, horizontalSplitter);
            }

            {
                VerticalSplitter verticalSplitter = new(splitter);
                Button button1 = new(verticalSplitter);
                button1.Text = "Vertical left";
                Button button2 = new(verticalSplitter);
                button2.Text = "Vertical right";
                verticalSplitter.SetPanel(index: 0, button1);
                verticalSplitter.SetPanel(index: 1, button2);
                splitter.SetPanel(index: 3, verticalSplitter);
            }

            //Status bar to hold unit testing buttons
            StatusBar pStatus = new(this);
            pStatus.Dock = Dock.Bottom;

            {
                Button pButton = new(pStatus);
                pButton.Text = "Zoom";
                pButton.Clicked += ZoomTest;
                pStatus.AddControl(pButton, right: false);
            }

            {
                Button pButton = new(pStatus);
                pButton.Text = "UnZoom";
                pButton.Clicked += UnZoomTest;
                pStatus.AddControl(pButton, right: false);
            }

            {
                Button pButton = new(pStatus);
                pButton.Text = "CenterPanels";
                pButton.Clicked += CenterPanels;
                pStatus.AddControl(pButton, right: true);
            }

            {
                Button pButton = new(pStatus);
                pButton.Text = "Splitters";
                pButton.Clicked += ToggleSplitters;
                pStatus.AddControl(pButton, right: true);
            }
        }

        private void ZoomTest(ControlBase control, EventArgs args)
        {
            splitter.Zoom(curZoom);
            curZoom++;

            if (curZoom == 4)
            {
                curZoom = 0;
            }
        }

        private void UnZoomTest(ControlBase control, EventArgs args)
        {
            splitter.UnZoom();
        }

        private void CenterPanels(ControlBase control, EventArgs args)
        {
            splitter.CenterPanels();
            splitter.UnZoom();
        }

        private void ToggleSplitters(ControlBase control, EventArgs args)
        {
            splitter.SplittersVisible = !splitter.SplittersVisible;
        }
    }
}
