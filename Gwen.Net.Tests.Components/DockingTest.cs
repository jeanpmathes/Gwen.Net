using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Layout", Order = 400)]
    public class DockingTest : GUnit
    {
        private readonly Font font;
        private readonly ControlBase outer;

        public DockingTest(ControlBase parent)
            : base(parent)
        {
            font = Skin.DefaultFont.Copy();
            font.Size *= 2;

            Label inner1, inner2, inner3, inner4, inner5;

            HorizontalLayout hlayout = new(this);

            {
                VerticalLayout vlayout = new(hlayout);

                {
                    outer = new DockLayout(vlayout);
                    outer.Size = new Size(width: 400, height: 400);

                    {
                        inner1 = new Label(outer);
                        inner1.Alignment = Alignment.Center;
                        inner1.Text = "1";
                        inner1.Font = font;
                        inner1.Size = new Size(width: 100, Util.Ignore);
                        inner1.Dock = Dock.Left;

                        inner2 = new Label(outer);
                        inner2.Alignment = Alignment.Center;
                        inner2.Text = "2";
                        inner2.Font = font;
                        inner2.Size = new Size(Util.Ignore, height: 100);
                        inner2.Dock = Dock.Top;

                        inner3 = new Label(outer);
                        inner3.Alignment = Alignment.Center;
                        inner3.Text = "3";
                        inner3.Font = font;
                        inner3.Size = new Size(width: 100, Util.Ignore);
                        inner3.Dock = Dock.Right;

                        inner4 = new Label(outer);
                        inner4.Alignment = Alignment.Center;
                        inner4.Text = "4";
                        inner4.Font = font;
                        inner4.Size = new Size(Util.Ignore, height: 100);
                        inner4.Dock = Dock.Bottom;

                        inner5 = new Label(outer);
                        inner5.Alignment = Alignment.Center;
                        inner5.Text = "5";
                        inner5.Font = font;
                        inner5.Size = new Size(Util.Ignore, Util.Ignore);
                        inner5.Dock = Dock.Fill;
                    }

                    outer.DrawDebugOutlines = true;

                    HorizontalLayout hlayout2 = new(vlayout);

                    {
                        Label l_padding = new(hlayout2);
                        l_padding.Text = "Padding:";

                        HorizontalSlider padding = new(hlayout2);
                        padding.Min = 0;
                        padding.Max = 200;
                        padding.Value = 10;
                        padding.Width = 100;
                        padding.ValueChanged += PaddingChanged;
                    }
                }

                GridLayout controlsLayout = new(hlayout);
                controlsLayout.ColumnCount = 2;

                {
                    inner1.UserData = CreateControls(inner1, Dock.Left, "Control 1", controlsLayout);
                    inner2.UserData = CreateControls(inner2, Dock.Top, "Control 2", controlsLayout);
                    inner3.UserData = CreateControls(inner3, Dock.Right, "Control 3", controlsLayout);
                    inner4.UserData = CreateControls(inner4, Dock.Bottom, "Control 4", controlsLayout);
                    inner5.UserData = CreateControls(inner5, Dock.Fill, "Control 5", controlsLayout);
                }
            }
            //DrawDebugOutlines = true;
        }

        private ControlBase CreateControls(ControlBase subject, Dock docking, string name, ControlBase container)
        {
            GroupBox gb = new(container);
            gb.Text = name;

            {
                HorizontalLayout hlayout = new(gb);

                {
                    GroupBox dgb = new(hlayout);
                    dgb.Text = "Dock";

                    {
                        RadioButtonGroup dock = new(dgb);
                        dock.UserData = subject;
                        dock.AddOption("Left", optionName: null, Dock.Left);
                        dock.AddOption("Top", optionName: null, Dock.Top);
                        dock.AddOption("Right", optionName: null, Dock.Right);
                        dock.AddOption("Bottom", optionName: null, Dock.Bottom);
                        dock.AddOption("Fill", optionName: null, Dock.Fill);
                        dock.SelectByUserData(docking);
                        dock.SelectionChanged += DockChanged;
                    }

                    VerticalLayout vlayout = new(hlayout);

                    {
                        HorizontalLayout hlayout2 = new(vlayout);

                        {
                            GroupBox hgb = new(hlayout2);
                            hgb.Text = "H. Align";

                            {
                                RadioButtonGroup halign = new(hgb);
                                halign.UserData = subject;
                                halign.AddOption("Left", optionName: null, HorizontalAlignment.Left);
                                halign.AddOption("Center", optionName: null, HorizontalAlignment.Center);
                                halign.AddOption("Right", optionName: null, HorizontalAlignment.Right);
                                halign.AddOption("Stretch", optionName: null, HorizontalAlignment.Stretch);
                                halign.SelectByUserData(subject.HorizontalAlignment);
                                halign.SelectionChanged += HAlignChanged;
                            }

                            GroupBox vgb = new(hlayout2);
                            vgb.Text = "V. Align";

                            {
                                RadioButtonGroup valign = new(vgb);
                                valign.UserData = subject;
                                valign.AddOption("Top", optionName: null, VerticalAlignment.Top);
                                valign.AddOption("Center", optionName: null, VerticalAlignment.Center);
                                valign.AddOption("Bottom", optionName: null, VerticalAlignment.Bottom);
                                valign.AddOption("Stretch", optionName: null, VerticalAlignment.Stretch);
                                valign.SelectByUserData(subject.VerticalAlignment);
                                valign.SelectionChanged += VAlignChanged;
                            }
                        }

                        GridLayout glayout = new(vlayout);
                        glayout.SetColumnWidths(GridLayout.AutoSize, GridLayout.Fill);

                        {
                            Label l_width = new(glayout);
                            l_width.Text = "Width:";

                            HorizontalSlider width = new(glayout);
                            width.Name = "Width";
                            width.UserData = subject;
                            width.Min = 50;
                            width.Max = 350;
                            width.Value = 100;
                            width.ValueChanged += WidthChanged;

                            Label l_height = new(glayout);
                            l_height.Text = "Height:";

                            HorizontalSlider height = new(glayout);
                            height.Name = "Height";
                            height.UserData = subject;
                            height.Min = 50;
                            height.Max = 350;
                            height.Value = 100;
                            height.ValueChanged += HeightChanged;

                            Label l_margin = new(glayout);
                            l_margin.Text = "Margin:";

                            HorizontalSlider margin = new(glayout);
                            margin.Name = "Margin";
                            margin.UserData = subject;
                            margin.Min = 0;
                            margin.Max = 50;
                            margin.Value = 0;
                            margin.ValueChanged += MarginChanged;
                        }
                    }
                }
            }

            return gb;
        }

        private void PaddingChanged(ControlBase control, EventArgs args)
        {
            Slider val = control as Slider;
            int i = (int)val.Value;
            outer.Padding = new Padding(i, i, i, i);
        }

        private void MarginChanged(ControlBase control, EventArgs args)
        {
            ControlBase inner = control.UserData as ControlBase;
            Slider val = control as Slider;
            int i = (int)val.Value;
            inner.Margin = new Margin(i, i, i, i);
        }

        private void WidthChanged(ControlBase control, EventArgs args)
        {
            ControlBase inner = control.UserData as ControlBase;
            Slider val = control as Slider;

            if (inner.HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                inner.Width = (int)val.Value;
            }
        }

        private void HeightChanged(ControlBase control, EventArgs args)
        {
            ControlBase inner = control.UserData as ControlBase;
            Slider val = control as Slider;

            if (inner.VerticalAlignment != VerticalAlignment.Stretch)
            {
                inner.Height = (int)val.Value;
            }
        }

        private void HAlignChanged(ControlBase control, EventArgs args)
        {
            ControlBase inner = control.UserData as ControlBase;
            RadioButtonGroup rbg = (RadioButtonGroup)control;
            inner.HorizontalAlignment = (HorizontalAlignment)rbg.Selected.UserData;

            if (inner.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                inner.Width = Util.Ignore;
            }
        }

        private void VAlignChanged(ControlBase control, EventArgs args)
        {
            ControlBase inner = control.UserData as ControlBase;
            RadioButtonGroup rbg = (RadioButtonGroup)control;
            inner.VerticalAlignment = (VerticalAlignment)rbg.Selected.UserData;

            if (inner.VerticalAlignment == VerticalAlignment.Stretch)
            {
                inner.Height = Util.Ignore;
            }
        }

        private void DockChanged(ControlBase control, EventArgs args)
        {
            ControlBase inner = (ControlBase)control.UserData;
            RadioButtonGroup rbg = (RadioButtonGroup)control;
            ControlBase gb = inner.UserData as ControlBase;
            int w = (int)(gb.FindChildByName("Width", recursive: true) as Slider).Value;
            int h = (int)(gb.FindChildByName("Height", recursive: true) as Slider).Value;
            inner.Dock = (Dock)rbg.Selected.UserData;

            switch (inner.Dock)
            {
                case Dock.Left:
                    inner.Size = new Size(w, Util.Ignore);

                    break;
                case Dock.Top:
                    inner.Size = new Size(Util.Ignore, h);

                    break;
                case Dock.Right:
                    inner.Size = new Size(w, Util.Ignore);

                    break;
                case Dock.Bottom:
                    inner.Size = new Size(Util.Ignore, h);

                    break;
                case Dock.Fill:
                    inner.Size = new Size(Util.Ignore, Util.Ignore);

                    break;
            }
        }

        public override void Dispose()
        {
            font.Dispose();
            base.Dispose();
        }
    }
}