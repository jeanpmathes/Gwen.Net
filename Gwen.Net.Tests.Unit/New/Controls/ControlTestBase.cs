using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Resources;
using Gwen.Net.Tests.Unit.New.Rendering;

namespace Gwen.Net.Tests.Unit.New.Controls;

public abstract class ControlTestBase<TControl>(Func<TControl> factory) where TControl : Control
{
    [Fact]
    public void Control_CanBeUsed()
    {
        var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        canvas.Child = factory();
        
        canvas.SetRenderingSize(new Size(width: 1000, height: 1000));
        canvas.Render();
        
        canvas.SetRenderingSize(new Size(width: 0, height: 0));
        canvas.Render();
        
        canvas.SetRenderingSize(new Size(width: 1, height: 1));
        canvas.Render();
        
        canvas.SetRenderingSize(new Size(width: 5000, height: 5000));
        canvas.Render();
        
        canvas.SetScale(0.5f);
        canvas.Render();
        
        canvas.SetScale(2.39f);
        canvas.Render();
        
        Assert.True(canvas.Children.OfType<TControl>().Any());
    }
}
