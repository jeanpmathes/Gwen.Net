﻿using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Containers", Order = 300)]
    public class WindowTest : GUnit
    {
        private readonly Random random;
        private int windowCount;

        public WindowTest(ControlBase parent)
            : base(parent)
        {
            random = new Random();

            VerticalLayout layout = new(this);
            layout.HorizontalAlignment = HorizontalAlignment.Left;

            Button button;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a Window";
            button.Clicked += OpenWindow;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a Window (with menu)";
            button.Clicked += OpenWindowWithMenuAndStatusBar;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a Window (auto size)";
            button.Clicked += OpenWindowAutoSizing;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a Window (modal)";
            button.Clicked += OpenWindowModal;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a MessageBox";
            button.Clicked += OpenMsgbox;

            button = new Button(layout);
            button.Margin = Margin.Five;
            button.Text = "Open a Long MessageBox";
            button.Clicked += OpenLongMsgbox;

            windowCount = 0;
        }

        private void OpenWindow(ControlBase control, EventArgs args)
        {
            Window window = new(this);
            window.Title = $"Window ({++windowCount})";

            window.Size = new Size(
                random.Next(minValue: 200, maxValue: 400),
                random.Next(minValue: 200, maxValue: 400));

            window.Left = random.Next(maxValue: 700);
            window.Top = random.Next(maxValue: 400);
            window.Padding = new Padding(left: 6, top: 3, right: 6, bottom: 6);

            RadioButtonGroup rbg = new(window);
            rbg.Dock = Dock.Top;
            rbg.AddOption("Resize disabled", "None").Checked += (c, a) => window.Resizing = Resizing.None;
            rbg.AddOption("Resize width", "Width").Checked += (c, a) => window.Resizing = Resizing.Width;
            rbg.AddOption("Resize height", "Height").Checked += (c, a) => window.Resizing = Resizing.Height;
            rbg.AddOption("Resize both", "Both").Checked += (c, a) => window.Resizing = Resizing.Both;
            rbg.SetSelectionByName("Both");

            LabeledCheckBox dragging = new(window);
            dragging.Dock = Dock.Top;
            dragging.Text = "Dragging";
            dragging.IsChecked = true;
            dragging.CheckChanged += (c, a) => window.IsDraggingEnabled = dragging.IsChecked;
        }

        private void OpenWindowWithMenuAndStatusBar(ControlBase control, EventArgs args)
        {
            Window window = new(this);
            window.Title = $"Window ({++windowCount})";

            window.Size = new Size(
                random.Next(minValue: 200, maxValue: 400),
                random.Next(minValue: 200, maxValue: 400));

            window.Left = random.Next(maxValue: 700);
            window.Top = random.Next(maxValue: 400);
            window.Padding = new Padding(left: 1, top: 0, right: 1, bottom: 1);

            DockLayout layout = new(window);

            MenuStrip menuStrip = new(layout);
            menuStrip.Dock = Dock.Top;

            /* File */
            {
                MenuItem root = menuStrip.AddItem("File");
                root.Menu.AddItem("Load", "test16.png", "Ctrl+L");
                root.Menu.AddItem("Save", string.Empty, "Ctrl+S");
                root.Menu.AddItem("Save As..", string.Empty, "Ctrl+A");
                root.Menu.AddItem("Quit", string.Empty, "Ctrl+Q").SetAction((c, a) => window.Close());
            }

            /* Resizing */
            {
                MenuItem root = menuStrip.AddItem("Resizing");
                root.Menu.AddItem("Disabled").SetAction((c, a) => window.Resizing = Resizing.None);
                root.Menu.AddItem("Width").SetAction((c, a) => window.Resizing = Resizing.Width);
                root.Menu.AddItem("Height").SetAction((c, a) => window.Resizing = Resizing.Height);
                root.Menu.AddItem("Both").SetAction((c, a) => window.Resizing = Resizing.Both);
            }

            StatusBar statusBar = new(layout);
            statusBar.Dock = Dock.Bottom;
            statusBar.Text = "Status bar";

            {
                Button br = new(statusBar);
                br.Text = "Right button";
                statusBar.AddControl(br, right: true);
            }
        }

        private void OpenWindowAutoSizing(ControlBase control, EventArgs args)
        {
            Window window = new(this);
            window.Title = string.Format("Window ({0})", ++windowCount);
            window.Left = random.Next(maxValue: 700);
            window.Top = random.Next(maxValue: 400);
            window.Padding = new Padding(left: 6, top: 3, right: 6, bottom: 6);
            window.HorizontalAlignment = HorizontalAlignment.Left;
            window.VerticalAlignment = VerticalAlignment.Top;
            window.Resizing = Resizing.None;

            VerticalLayout layout = new(window);

            GroupBox grb = new(layout);
            grb.Text = "Auto size";
            layout = new VerticalLayout(grb);

            {
                Label label = new(layout);
                label.Margin = Margin.Six;
                label.Text = "Label text";

                Button button = new(layout);
                button.Margin = Margin.Six;
                button.Text = "Click Me";
                button.Width = 200;

                label = new Label(layout);
                label.Margin = Margin.Six;
                label.Text = "Hide / Show Label";
                //label.IsCollapsed = true;

                button.Clicked += (s, a) => label.IsCollapsed = !label.IsCollapsed;
            }
        }

        private void OpenWindowModal(ControlBase control, EventArgs args)
        {
            Window window = new(this);
            window.Title = string.Format("Modal Window ({0})", ++windowCount);
            window.Left = random.Next(maxValue: 700);
            window.Top = random.Next(maxValue: 400);
            window.Padding = new Padding(left: 6, top: 3, right: 6, bottom: 6);
            window.HorizontalAlignment = HorizontalAlignment.Left;
            window.VerticalAlignment = VerticalAlignment.Top;
            window.Resizing = Resizing.None;
            window.MakeModal(dim: true);

            VerticalLayout layout = new(window);

            GroupBox grb = new(layout);
            grb.Text = "Auto size";
            layout = new VerticalLayout(grb);

            {
                Label label = new(layout);
                label.Margin = Margin.Six;
                label.Text = "Label text";

                Button button = new(layout);
                button.Margin = Margin.Six;
                button.Text = "Button";
                button.Width = 200;
            }
        }

        private void OpenMsgbox(ControlBase control, EventArgs args)
        {
            MessageBox window = new(this, "Message box test text.");
            window.Dismissed += OnDismissed;
            window.SetPosition(random.Next(maxValue: 700), random.Next(maxValue: 400));
        }

        private void OpenLongMsgbox(ControlBase control, EventArgs args)
        {
            MessageBox window =
                new(
                    this,
                    @"In olden times when wishing still helped one, there lived a king whose daughters were all beautiful, but the youngest was so beautiful that the sun itself, which has seen so much, was astonished whenever it shone in her face. Close by the king's castle lay a great dark forest, and under an old lime-tree in the forest was a well, and when the day was very warm, the king's child went out into the forest and sat down by the side of the cool fountain, and when she was bored she took a golden ball, and threw it up on high and caught it, and this ball was her favorite plaything.",
                    "Long Text",
                    MessageBoxButtons.AbortRetryIgnore);

            window.Dismissed += OnDismissed;
            window.SetPosition(random.Next(maxValue: 700), random.Next(maxValue: 400));
        }

        private void OnDismissed(ControlBase sender, MessageBoxResultEventArgs args)
        {
            UnitTest.PrintText("Message box result: " + args.Result);
        }
    }
}
