using System;
using System.Collections.Generic;
using System.Linq;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Skin;

namespace Gwen.Net.Tests.Components
{
    public class UnitTestHarnessControls : ControlBase
    {
        private readonly ControlBase center;
        private readonly LabeledCheckBox debugCheck;
        private readonly Button decScale;
        private readonly Button incScale;
        private readonly CollapsibleList list;
        private readonly StatusBar statusBar;
        private readonly ListBox textOutput;
        private readonly TextBoxNumeric uIScale;
        private readonly Label uIScaleText;
        private ControlBase lastControl;
        public string Note { get; set; }

        public double RenderFps { get; set; }
        public double UpdateFps { get; set; }

        public UnitTestHarnessControls(ControlBase parent) : base(parent)
        {
            Dock = Dock.Fill;

            DockBase dock = new(this);
            dock.Dock = Dock.Fill;

            list = new CollapsibleList(this);

            dock.LeftDock.TabControl.AddPage("Unit tests", list);
            dock.LeftDock.Width = 150;

            textOutput = new ListBox(this);
            textOutput.AlternateColor = false;

            dock.BottomDock.TabControl.AddPage("Output", textOutput);
            dock.BottomDock.Height = 200;

            statusBar = new StatusBar(this);
            statusBar.Dock = Dock.Bottom;

            debugCheck = new LabeledCheckBox(statusBar);
            debugCheck.Text = "Debug outlines";
            debugCheck.CheckChanged += DebugCheckChanged;

            incScale = new Button(statusBar);
            incScale.HorizontalAlignment = HorizontalAlignment.Left;
            incScale.VerticalAlignment = VerticalAlignment.Stretch;
            incScale.Width = 30;
            incScale.Margin = new Margin(left: 0, top: 0, right: 8, bottom: 0);
            incScale.TextPadding = new Padding(left: 5, top: 0, right: 5, bottom: 0);
            incScale.Text = "+";
            
            uIScale = new TextBoxNumeric(statusBar);
            uIScale.VerticalAlignment = VerticalAlignment.Stretch;
            uIScale.Width = 70;
            uIScale.Value = GetCanvas().Scale;
            uIScale.TextChanged += (_, _) => { GetCanvas().Scale = uIScale.Value; };

            incScale.Clicked += (_, _) =>
            {
                uIScale.Value = Math.Min(uIScale.Value + 0.25f, val2: 3.0f);
            };

            decScale = new Button(statusBar);
            decScale.HorizontalAlignment = HorizontalAlignment.Left;
            decScale.VerticalAlignment = VerticalAlignment.Stretch;
            decScale.Width = 30;
            decScale.Margin = new Margin(left: 4, top: 0, right: 0, bottom: 0);
            decScale.TextPadding = new Padding(left: 5, top: 0, right: 5, bottom: 0);
            decScale.Text = "-";

            decScale.Clicked += (_, _) =>
            {
                uIScale.Value = Math.Max(uIScale.Value - 0.25f, val2: 1.0f);
            };

            uIScaleText = new Label(statusBar);
            uIScaleText.VerticalAlignment = VerticalAlignment.Stretch;
            uIScaleText.Alignment = Alignment.Left | Alignment.CenterV;
            uIScaleText.Text = "Scale:";

            center = new DockLayout(dock);
            center.Dock = Dock.Fill;

            List<Type> tests = typeof(UnitTestHarnessControls).Assembly.GetTypes()
                .Where(t => t.IsDefined(typeof(UnitTestAttribute), inherit: false)).ToList();

            tests.Sort(
                (t1, t2) =>
                {
                    object[] a1S = t1.GetCustomAttributes(typeof(UnitTestAttribute), inherit: false);
                    object[] a2S = t2.GetCustomAttributes(typeof(UnitTestAttribute), inherit: false);

                    if (a1S.Length > 0 && a2S.Length > 0)
                    {
                        var a1 = a1S[0] as UnitTestAttribute;
                        var a2 = a2S[0] as UnitTestAttribute;

                        if (a1.Order == a2.Order)
                        {
                            if (a1.Category == a2.Category)
                            {
                                return string.Compare(
                                    a1.Name ?? t1.Name,
                                    a2.Name ?? t2.Name,
                                    StringComparison.OrdinalIgnoreCase);
                            }

                            return string.Compare(a1.Category, a2.Category, StringComparison.OrdinalIgnoreCase);
                        }

                        return a1.Order - a2.Order;
                    }

                    return 0;
                });

            foreach (Type type in tests)
            {
                object[] attribs = type.GetCustomAttributes(typeof(UnitTestAttribute), inherit: false);

                if (attribs.Length > 0)
                {
                    var attrib = attribs[0] as UnitTestAttribute;

                    if (attrib != null)
                    {
                        var cat = list.FindChildByName(attrib.Category) as CollapsibleCategory;

                        if (cat == null)
                        {
                            cat = list.Add(attrib.Category, attrib.Category);
                        }

                        var test = Activator.CreateInstance(type, center) as GUnit;
                        RegisterUnitTest(attrib.Name ?? type.Name, cat, test);
                    }
                }
            }

            statusBar.SendToBack();
            PrintText("Unit Test started!");
        }

        public void RegisterUnitTest(string name, CollapsibleCategory cat, GUnit test)
        {
            Button btn = cat.Add(name);
            test.Dock = Dock.Fill;
            test.Collapse();
            test.UnitTest = this;
            btn.UserData = test;
            btn.Clicked += OnCategorySelect;
        }

        private void DebugCheckChanged(ControlBase control, EventArgs args)
        {
            center.DrawDebugOutlines = debugCheck.IsChecked;

            foreach (ControlBase c in GetCanvas().Children.Where(x => x is WindowBase))
            {
                var win = c as WindowBase;

                if (win != null)
                {
                    win.Content.DrawDebugOutlines = debugCheck.IsChecked;
                }
            }
        }

        private void OnCategorySelect(ControlBase control, EventArgs args)
        {
            if (lastControl != null)
            {
                lastControl.Collapse();
            }

            var test = control.UserData as ControlBase;
            test.Show();
            lastControl = test;
        }

        public void PrintText(string str)
        {
            textOutput.AddRow(str);
            textOutput.ScrollToBottom();
        }

        protected override void Render(SkinBase currentSkin)
        {
            statusBar.Text = string.Format(
                "GWEN.Net Unit Test - {0} Render Frames, {1} Update Frames. {2}",
                RenderFps,
                UpdateFps,
                Note);

            base.Render(currentSkin);
        }
    }
}
