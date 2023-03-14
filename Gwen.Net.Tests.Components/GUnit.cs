using Gwen.Net.Control;

namespace Gwen.Net.Tests.Components
{
    public class GUnit : ControlBase
    {
        public UnitTestHarnessControls UnitTest { get; set; }

        public GUnit(ControlBase parent) : base(parent)
        {
            IsVirtualControl = true;
        }

        public void UnitPrint(string str)
        {
            UnitTest?.PrintText(str);
        }
    }
}
