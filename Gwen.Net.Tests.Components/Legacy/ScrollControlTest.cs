using Gwen.Net.Legacy;
using Gwen.Net.Legacy.Control;
using Gwen.Net.Legacy.Control.Layout;

namespace Gwen.Net.Tests.Components.Legacy
{
    [UnitTest(Category = "Containers", Order = 305)]
    public class ScrollControlTest : GUnit
    {
        public ScrollControlTest(ControlBase parent)
            : base(parent)
        {
            GridLayout layout = new(this);
            layout.ColumnCount = 6;

            Button pTestButton;

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Twice As Big";
                pTestButton.Size = new Size(width: 200, height: 200);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Same Size";
                pTestButton.Size = new Size(width: 100, height: 100);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Wide";
                pTestButton.Size = new Size(width: 200, height: 50);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Tall";
                pTestButton.Size = new Size(width: 50, height: 200);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.EnableScroll(horizontal: false, vertical: true);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Vertical";
                pTestButton.Size = new Size(width: 200, height: 200);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.EnableScroll(horizontal: true, vertical: false);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Horizontal";
                pTestButton.Size = new Size(width: 200, height: 200);
            }

            // Bottom Row

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.AutoHideBars = true;

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Twice As Big";
                pTestButton.Size = new Size(width: 200, height: 200);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.AutoHideBars = true;

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Same Size";
                pTestButton.Size = new Size(width: 100, height: 100);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.AutoHideBars = true;

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Wide";
                pTestButton.Size = new Size(width: 200, height: 50);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.AutoHideBars = true;

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Tall";
                pTestButton.Size = new Size(width: 50, height: 200);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.AutoHideBars = true;
                ctrl.EnableScroll(horizontal: false, vertical: true);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Vertical";
                pTestButton.Size = new Size(width: 200, height: 200);
            }

            {
                ScrollControl ctrl = new(layout);
                ctrl.Margin = Margin.Three;
                ctrl.Size = new Size(width: 100, height: 100);
                ctrl.AutoHideBars = true;
                ctrl.EnableScroll(horizontal: true, vertical: false);

                pTestButton = new Button(ctrl);
                pTestButton.Text = "Horinzontal";
                pTestButton.Size = new Size(width: 200, height: 200);
            }
        }
    }
}