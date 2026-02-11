using System;
using Gwen.Net.New.Bindings;

namespace Gwen.Net.Tests.Components.New;

public class UnitTestHarness
{
    public Slot<Double> RenderFps { get; set; } = new(0.0);
    
    public Slot<Double> UpdateFps { get; set; } = new(0.0);
}
