using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Standard", Order = 200)]
    public class ButtonTest : GUnit
    {
        public ButtonTest(ControlBase parent)
            : base(parent)
        {
            HorizontalLayout hlayout = new(this);

            {
                VerticalLayout vlayout = new(hlayout);
                vlayout.Width = 300;

                {
                    Button button;

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Text = "Button";

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Image button (default)";
                    button.SetImage("test16.png");

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Image button (above)";
                    button.SetImage("test16.png", ImageAlign.Above);

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Alignment = Alignment.Left | Alignment.CenterV;
                    button.Text = "Image button (left)";
                    button.SetImage("test16.png");

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Alignment = Alignment.Right | Alignment.CenterV;
                    button.Text = "Image button (right)";
                    button.SetImage("test16.png");

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Image button (image left)";
                    button.SetImage("test16.png", ImageAlign.Left | ImageAlign.CenterV);

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Image button (image right)";
                    button.SetImage("test16.png", ImageAlign.Right | ImageAlign.CenterV);

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Image button (image fill)";
                    button.SetImage("test16.png", ImageAlign.Fill);

                    HorizontalLayout hlayout2 = new(vlayout);

                    {
                        button = new Button(hlayout2);
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.Padding = Padding.Three;
                        button.Margin = Margin.Five;
                        button.SetImage("test16.png");
                        button.ImageSize = new Size(width: 32, height: 32);

                        button = new Button(hlayout2);
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Center;
                        button.Padding = Padding.Three;
                        button.Margin = Margin.Five;
                        button.SetImage("test16.png");

                        button = new Button(hlayout2);
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Center;
                        button.Padding = Padding.Three;
                        button.Margin = Margin.Five;
                        button.SetImage("test16.png");
                        button.ImageTextureRect = new Rectangle(x: 4, y: 4, width: 8, height: 8);

                        button = new Button(hlayout2);
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Center;
                        button.Padding = Padding.Three;
                        button.Margin = Margin.Five;
                        button.SetImage("test16.png");
                        button.ImageColor = Color.DarkGrey;
                    }

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = new Padding(left: 20, top: 20, right: 20, bottom: 20);
                    button.Text = "Toggle me";
                    button.IsToggle = true;
                    button.Toggled += OnToggle;
                    button.ToggledOn += OnToggleOn;
                    button.ToggledOff += OnToggleOff;

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Disabled";
                    button.IsDisabled = true;

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "With Tooltip";
                    button.SetToolTipText("This is tooltip");

                    button = new Button(vlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Autosized";
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                }

                {
                    Button button = new(hlayout);
                    button.Margin = Margin.Five;
                    button.Padding = Padding.Three;
                    button.Text = "Event tester";
                    button.Size = new Size(width: 300, height: 200);
                    button.Pressed += OnButtonAp;
                    button.Clicked += OnButtonAc;
                    button.Released += OnButtonAr;
                }
            }
        }

        private void OnButtonAc(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Clicked");
        }

        private void OnButtonAp(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Pressed");
        }

        private void OnButtonAr(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Released");
        }

        private void OnToggle(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: Toggled");
        }

        private void OnToggleOn(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: ToggleOn");
        }

        private void OnToggleOff(ControlBase control, EventArgs args)
        {
            UnitPrint("Button: ToggledOff");
        }
    }
}
