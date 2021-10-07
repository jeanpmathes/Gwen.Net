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
        private readonly ControlBase m_Center;
        private readonly LabeledCheckBox m_DebugCheck;
        private readonly Button m_DecScale;
        private readonly Button m_IncScale;
        private readonly CollapsibleList m_List;
        private readonly StatusBar m_StatusBar;
        private readonly ListBox m_TextOutput;
        private readonly TextBoxNumeric m_UIScale;
        private readonly Label m_UIScaleText;
        private ControlBase m_LastControl;
        public string Note;

        public double RenderFps;
        public double UpdateFps;

        public UnitTestHarnessControls(ControlBase parent) : base(parent)
        {
            Dock = Dock.Fill;

            DockBase dock = new(this);
            dock.Dock = Dock.Fill;

            m_List = new CollapsibleList(this);

            dock.LeftDock.TabControl.AddPage("Unit tests", m_List);
            dock.LeftDock.Width = 150;

            m_TextOutput = new ListBox(this);
            m_TextOutput.AlternateColor = false;

            dock.BottomDock.TabControl.AddPage("Output", m_TextOutput);
            dock.BottomDock.Height = 200;

            m_StatusBar = new StatusBar(this);
            m_StatusBar.Dock = Dock.Bottom;

            m_DebugCheck = new LabeledCheckBox(m_StatusBar);
            m_DebugCheck.Text = "Debug outlines";
            m_DebugCheck.CheckChanged += DebugCheckChanged;

            m_IncScale = new Button(m_StatusBar);
            m_IncScale.HorizontalAlignment = HorizontalAlignment.Left;
            m_IncScale.VerticalAlignment = VerticalAlignment.Stretch;
            m_IncScale.Width = 30;
            m_IncScale.Margin = new Margin(left: 0, top: 0, right: 8, bottom: 0);
            m_IncScale.TextPadding = new Padding(left: 5, top: 0, right: 5, bottom: 0);
            m_IncScale.Text = "+";

            m_IncScale.Clicked += (sender, arguments) =>
            {
                m_UIScale.Value = Math.Min(m_UIScale.Value + 0.25f, val2: 3.0f);
            };

            m_UIScale = new TextBoxNumeric(m_StatusBar);
            m_UIScale.VerticalAlignment = VerticalAlignment.Stretch;
            m_UIScale.Width = 70;
            m_UIScale.Value = GetCanvas().Scale;
            m_UIScale.TextChanged += (sender, arguments) => { GetCanvas().Scale = m_UIScale.Value; };

            m_DecScale = new Button(m_StatusBar);
            m_DecScale.HorizontalAlignment = HorizontalAlignment.Left;
            m_DecScale.VerticalAlignment = VerticalAlignment.Stretch;
            m_DecScale.Width = 30;
            m_DecScale.Margin = new Margin(left: 4, top: 0, right: 0, bottom: 0);
            m_DecScale.TextPadding = new Padding(left: 5, top: 0, right: 5, bottom: 0);
            m_DecScale.Text = "-";

            m_DecScale.Clicked += (sender, arguments) =>
            {
                m_UIScale.Value = Math.Max(m_UIScale.Value - 0.25f, val2: 1.0f);
            };

            m_UIScaleText = new Label(m_StatusBar);
            m_UIScaleText.VerticalAlignment = VerticalAlignment.Stretch;
            m_UIScaleText.Alignment = Alignment.Left | Alignment.CenterV;
            m_UIScaleText.Text = "Scale:";

            m_Center = new DockLayout(dock);
            m_Center.Dock = Dock.Fill;

            List<Type> tests = typeof(UnitTestHarnessControls).Assembly.GetTypes()
                .Where(t => t.IsDefined(typeof(UnitTestAttribute), inherit: false)).ToList();

            tests.Sort(
                (t1, t2) =>
                {
                    object[] a1s = t1.GetCustomAttributes(typeof(UnitTestAttribute), inherit: false);
                    object[] a2s = t2.GetCustomAttributes(typeof(UnitTestAttribute), inherit: false);

                    if (a1s.Length > 0 && a2s.Length > 0)
                    {
                        UnitTestAttribute a1 = a1s[0] as UnitTestAttribute;
                        UnitTestAttribute a2 = a2s[0] as UnitTestAttribute;

                        if (a1.Order == a2.Order)
                        {
                            if (a1.Category == a2.Category)
                            {
                                return String.Compare(
                                    a1.Name != null ? a1.Name : t1.Name,
                                    a2.Name != null ? a2.Name : t2.Name,
                                    StringComparison.OrdinalIgnoreCase);
                            }

                            return String.Compare(a1.Category, a2.Category, StringComparison.OrdinalIgnoreCase);
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
                    UnitTestAttribute attrib = attribs[0] as UnitTestAttribute;

                    if (attrib != null)
                    {
                        CollapsibleCategory cat = m_List.FindChildByName(attrib.Category) as CollapsibleCategory;

                        if (cat == null)
                        {
                            cat = m_List.Add(attrib.Category, attrib.Category);
                        }

                        GUnit test = Activator.CreateInstance(type, m_Center) as GUnit;
                        RegisterUnitTest(attrib.Name != null ? attrib.Name : type.Name, cat, test);
                    }
                }
            }

            m_StatusBar.SendToBack();
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
            m_Center.DrawDebugOutlines = m_DebugCheck.IsChecked;

            foreach (ControlBase c in GetCanvas().Children.Where(x => x is WindowBase))
            {
                WindowBase win = c as WindowBase;

                if (win != null)
                {
                    win.Content.DrawDebugOutlines = m_DebugCheck.IsChecked;
                }
            }
        }

        private void OnCategorySelect(ControlBase control, EventArgs args)
        {
            if (m_LastControl != null)
            {
                m_LastControl.Collapse();
            }

            ControlBase test = control.UserData as ControlBase;
            test.Show();
            m_LastControl = test;
        }

        public void PrintText(string str)
        {
            m_TextOutput.AddRow(str);
            m_TextOutput.ScrollToBottom();
        }

        protected override void Render(SkinBase skin)
        {
            m_StatusBar.Text = String.Format(
                "GWEN.Net Unit Test - {0} Render Frames, {1} Update Frames. {2}",
                RenderFps,
                UpdateFps,
                Note);

            base.Render(skin);
        }
    }
}