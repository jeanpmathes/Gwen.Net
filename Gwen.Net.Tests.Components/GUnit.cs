using System;
using Gwen.Net.Control;

namespace Gwen.Net.Tests.Components
{
    public class GUnit : ControlBase
    {
        public GUnit(ControlBase parent) : base(parent)
        {
            IsVirtualControl = true;
        }

        public UnitTestHarnessControls UnitTest { get; set; }

        public void UnitPrint(String str)
        {
            UnitTest?.PrintText(str);
        }
    }
}
