using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Containers", Order = 301)]
    public class ToolWindowTest : GUnit
    {
        public ToolWindowTest(ControlBase parent)
            : base(parent)
        {
            VerticalLayout layout = new(this);
            layout.HorizontalAlignment = HorizontalAlignment.Left;

            Button button;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a ToolBar";
            button.Clicked += OpenToolBar;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a tool window";
            button.Clicked += OpenToolWindow;
        }

        private void OpenToolBar(ControlBase control, EventArgs args)
        {
            ToolWindow window = new(this);
            window.Padding = Padding.Five;
            window.HorizontalAlignment = HorizontalAlignment.Left;
            window.VerticalAlignment = VerticalAlignment.Top;
            window.StartPosition = StartPosition.CenterCanvas;

            HorizontalLayout layout = new(window);

            for (int i = 0; i < 5; i++)
            {
                Button button = new(layout);
                button.Size = new Size(width: 36, height: 36);
                button.UserData = window;
                button.Clicked += Close;
            }
        }

        private void OpenToolWindow(ControlBase control, EventArgs args)
        {
            ToolWindow window = new(this);
            window.Padding = Padding.Five;
            window.HorizontalAlignment = HorizontalAlignment.Left;
            window.VerticalAlignment = VerticalAlignment.Top;
            window.StartPosition = StartPosition.CenterParent;
            window.Vertical = true;

            GridLayout layout = new(window);
            layout.ColumnCount = 2;

            Button button = new(layout);
            button.Size = new Size(width: 100, height: 40);
            button.UserData = window;
            button.Clicked += Close;

            button = new Button(layout);
            button.Size = new Size(width: 100, height: 40);
            button.UserData = window;
            button.Clicked += Close;

            button = new Button(layout);
            button.Size = new Size(width: 100, height: 40);
            button.UserData = window;
            button.Clicked += Close;

            button = new Button(layout);
            button.Size = new Size(width: 100, height: 40);
            button.UserData = window;
            button.Clicked += Close;
        }

        private void Close(ControlBase control, EventArgs args)
        {
            ToolWindow window = control.UserData as ToolWindow;
            window.Close();
            window.Parent.RemoveChild(window, dispose: true);
        }
    }
}