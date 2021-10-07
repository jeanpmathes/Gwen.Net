using Gwen.Net.Control;

namespace Gwen.Net.Tests.Components
{
    [UnitTest(Category = "Non-Interactive", Order = 106)]
    public class StatusBarTest : GUnit
    {
        public StatusBarTest(ControlBase parent)
            : base(parent)
        {
            StatusBar sb = new(this);
            Label left = new(sb);
            left.Text = "Label added to left";
            sb.AddControl(left, right: false);

            Label right = new(sb);
            right.Text = "Label added to right";
            sb.AddControl(right, right: true);

            Button bl = new(sb);
            bl.Text = "Left button";
            sb.AddControl(bl, right: false);

            Button br = new(sb);
            br.Text = "Right button";
            sb.AddControl(br, right: true);
        }
    }
}