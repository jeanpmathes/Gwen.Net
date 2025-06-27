using System;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Layout", Order = 402)]
    public class AnchorLayoutTest : GUnit
    {
        private readonly Font font;

        public AnchorLayoutTest(ControlBase parent)
            : base(parent)
        {
            font = new Font(Skin.Renderer, "Arial");

            AnchorLayout layout = new(this);
            layout.Size = new Size(width: 445, height: 165);
            layout.Padding = Padding.Five;
            layout.AnchorBounds = new Rectangle(x: 0, y: 0, width: 445, height: 165);

            Button button = new(layout);
            button.Font = font;
            button.Text = "Left Top";
            button.AnchorBounds = new Rectangle(x: 10, y: 10, width: 100, height: 20);
            button.Anchor = Anchor.LeftTop;

            button = new Button(layout);
            button.Font = font;
            button.Text = "Center Top";
            button.AnchorBounds = new Rectangle(x: 150, y: 10, width: 100, height: 20);
            button.Anchor = new Anchor(left: 50, top: 0, right: 50, bottom: 0);

            button = new Button(layout);
            button.Font = font;
            button.Text = "Right Top";
            button.AnchorBounds = new Rectangle(x: 290, y: 10, width: 100, height: 20);
            button.Anchor = Anchor.RightTop;

            button = new Button(layout);
            button.Font = font;
            button.Text = "Left Center";
            button.AnchorBounds = new Rectangle(x: 10, y: 50, width: 100, height: 20);
            button.Anchor = new Anchor(left: 0, top: 50, right: 0, bottom: 50);

            button = new Button(layout);
            button.Font = font;
            button.Text = "Center";
            button.AnchorBounds = new Rectangle(x: 150, y: 50, width: 100, height: 20);
            button.Anchor = new Anchor(left: 50, top: 50, right: 50, bottom: 50);

            button = new Button(layout);
            button.Font = font;
            button.Text = "Right Center";
            button.AnchorBounds = new Rectangle(x: 290, y: 50, width: 100, height: 20);
            button.Anchor = new Anchor(left: 100, top: 50, right: 100, bottom: 50);

            button = new Button(layout);
            button.Font = font;
            button.Text = "Left Bottom";
            button.AnchorBounds = new Rectangle(x: 10, y: 90, width: 100, height: 20);
            button.Anchor = Anchor.LeftBottom;

            button = new Button(layout);
            button.Font = font;
            button.Text = "Center Bottom";
            button.AnchorBounds = new Rectangle(x: 150, y: 90, width: 100, height: 20);
            button.Anchor = new Anchor(left: 50, top: 100, right: 50, bottom: 100);

            button = new Button(layout);
            button.Font = font;
            button.Text = "Right Bottom";
            button.AnchorBounds = new Rectangle(x: 290, y: 90, width: 100, height: 20);
            button.Anchor = Anchor.RightBottom;

            HorizontalSlider horizontal = new(layout);
            horizontal.AnchorBounds = new Rectangle(x: 10, y: 125, width: 380, height: 25);
            horizontal.Anchor = new Anchor(left: 0, top: 100, right: 100, bottom: 100);

            VerticalSlider vert = new(layout);
            vert.AnchorBounds = new Rectangle(x: 405, y: 10, width: 25, height: 100);
            vert.Anchor = new Anchor(left: 100, top: 0, right: 100, bottom: 100);

            HorizontalSlider width = new(this);
            width.Min = 445;
            width.Max = 800;
            width.Height = 25;
            width.Dock = Dock.Bottom;
            width.Padding = Padding.Five;
            width.ValueChanged += (control, _) => { layout.Width = (Int32)((HorizontalSlider) control).Value; };

            VerticalSlider height = new(this);
            height.Min = 165;
            height.Max = 400;
            height.Width = 25;
            height.Dock = Dock.Right;
            height.Padding = Padding.Five;
            height.ValueChanged += (control, _) => { layout.Height = (Int32)((VerticalSlider) control).Value; };
        }

        public override void Dispose()
        {
            font.Dispose();
            base.Dispose();
        }
    }
}
