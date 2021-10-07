using Gwen.Net.Control;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Non-Interactive", Order = 101)]
    public class LinkLabelTest : GUnit
    {
        private readonly Font font1;
        private readonly Font fontHover1;

        public LinkLabelTest(ControlBase parent)
            : base(parent)
        {
            {
                LinkLabel label = new(this);
                label.Dock = Dock.Top;
                label.HoverColor = new Color(a: 255, r: 255, g: 255, b: 255);
                label.Text = "Link Label (default font)";
                label.Link = "Test Link";
                label.LinkClicked += OnLinkClicked;
            }

            {
                font1 = new Font(Skin.Renderer, "Comic Sans MS", size: 25);
                fontHover1 = new Font(Skin.Renderer, "Comic Sans MS", size: 25);
                fontHover1.Underline = true;

                LinkLabel label = new(this);
                label.Dock = Dock.Top;
                label.Font = font1;
                label.HoverFont = fontHover1;
                label.TextColor = new Color(a: 255, r: 0, g: 80, b: 205);
                label.HoverColor = new Color(a: 255, r: 0, g: 100, b: 255);
                label.Text = "Custom Font (Comic Sans 25)";
                label.Link = "Custom Font Link";
                label.LinkClicked += OnLinkClicked;
            }
        }

        public override void Dispose()
        {
            font1.Dispose();
            fontHover1.Dispose();
            base.Dispose();
        }

        private void OnLinkClicked(ControlBase control, LinkClickedEventArgs args)
        {
            UnitPrint("Link Clicked: " + args.Link);
        }
    }
}