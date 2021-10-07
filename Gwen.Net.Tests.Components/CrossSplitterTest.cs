using System;
using Gwen.Net.Control;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Layout", Order = 403)]
    public class CrossSplitterTest : GUnit
    {
        private int m_CurZoom;
        private readonly CrossSplitter m_Splitter;

        public CrossSplitterTest(ControlBase parent)
            : base(parent)
        {
            m_CurZoom = 0;

            m_Splitter = new CrossSplitter(this);
            m_Splitter.Dock = Dock.Fill;

            {
                VerticalSplitter splitter = new(m_Splitter);
                Button button1 = new(splitter);
                button1.Text = "Vertical left";
                Button button2 = new(splitter);
                button2.Text = "Vertical right";
                splitter.SetPanel(index: 0, button1);
                splitter.SetPanel(index: 1, button2);
                m_Splitter.SetPanel(index: 0, splitter);
            }

            {
                HorizontalSplitter splitter = new(m_Splitter);
                Button button1 = new(splitter);
                button1.Text = "Horizontal up";
                Button button2 = new(splitter);
                button2.Text = "Horizontal down";
                splitter.SetPanel(index: 0, button1);
                splitter.SetPanel(index: 1, button2);
                m_Splitter.SetPanel(index: 1, splitter);
            }

            {
                HorizontalSplitter splitter = new(m_Splitter);
                Button button1 = new(splitter);
                button1.Text = "Horizontal up";
                Button button2 = new(splitter);
                button2.Text = "Horizontal down";
                splitter.SetPanel(index: 0, button1);
                splitter.SetPanel(index: 1, button2);
                m_Splitter.SetPanel(index: 2, splitter);
            }

            {
                VerticalSplitter splitter = new(m_Splitter);
                Button button1 = new(splitter);
                button1.Text = "Vertical left";
                Button button2 = new(splitter);
                button2.Text = "Vertical right";
                splitter.SetPanel(index: 0, button1);
                splitter.SetPanel(index: 1, button2);
                m_Splitter.SetPanel(index: 3, splitter);
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
            m_Splitter.Zoom(m_CurZoom);
            m_CurZoom++;

            if (m_CurZoom == 4)
            {
                m_CurZoom = 0;
            }
        }

        private void UnZoomTest(ControlBase control, EventArgs args)
        {
            m_Splitter.UnZoom();
        }

        private void CenterPanels(ControlBase control, EventArgs args)
        {
            m_Splitter.CenterPanels();
            m_Splitter.UnZoom();
        }

        private void ToggleSplitters(ControlBase control, EventArgs args)
        {
            m_Splitter.SplittersVisible = !m_Splitter.SplittersVisible;
        }

#if false
		protected override void Layout(Skin.Base skin)
        {
            
        }
#endif
    }
}