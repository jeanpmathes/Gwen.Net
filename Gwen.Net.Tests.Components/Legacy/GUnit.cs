using System;
using Gwen.Net.Legacy.Control;

namespace Gwen.Net.Tests.Components.Legacy
{
    public class GUnit : ControlBase
    {
        public UnitTestHarnessControls UnitTest { get; set; }

        public GUnit(ControlBase parent) : base(parent)
        {
            IsVirtualControl = true;
        }

        public void UnitPrint(String str)
        {
            UnitTest?.PrintText(str);
        }
    }
}
