using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Visuals;
using Gwen.Net.Tests.Unit.New.Rendering;
using Canvas = Gwen.Net.New.Controls.Canvas;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public abstract class VisualTestBase<TVisual>(Func<TVisual> factory) where TVisual : Visual
{
    [Fact]
    public void Visual_CanBeUsed()
    {
        var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        
        var isCreated = false;
        canvas.Child = new MockControl(() =>
        {
            isCreated = true;
            return factory();
        });
        
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
        
        canvas.SetDebugOutlines(true);
        canvas.Render();
        
        Assert.True(isCreated);
    }
    
    private class MockControl(Func<TVisual> factory) : Control<MockControl>
    {
        protected override ControlTemplate<MockControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<MockControl>(_ => factory());
        }
    }
}
