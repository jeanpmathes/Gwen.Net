using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;
using Gwen.Net.Tests.Unit.New.Input;
using Gwen.Net.Tests.Unit.New.Rendering;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Controls;

public class ControlTests
{
    [Fact]
    public void ContextStyle_IsAppliedWhenControlGetsVisualized()
    {
        using var registry = new ResourceRegistry();
        registry.AddStyle<Border>(builder => builder.Set(c => c.MinimumWidth, value: 25f));

        using var canvas = Canvas.Create(new MockRenderer(), registry);
        var border = new Border();

        canvas.Child = border;

        Assert.Equal(expected: 25f, border.MinimumWidth.GetValue());
    }

    [Fact]
    public void LocalStyle_IsAppliedAfterOuterStyles()
    {
        using var registry = new ResourceRegistry();
        registry.AddStyle<Border>(builder => builder.Set(c => c.MinimumWidth, value: 50f));

        using var canvas = Canvas.Create(new MockRenderer(), registry);
        var border = new Border
        {
            Style =
            {
                Value = Styling.Create<Border>(builder => builder.Set(c => c.MinimumWidth, value: 10f))
            }
        };

        canvas.Child = border;

        Assert.Equal(expected: 10f, border.MinimumWidth.GetValue());
    }

    [Fact]
    public void ChangingTemplate_RevisualizesControlWithNewTemplate()
    {
        using var registry = new ResourceRegistry();
        using var canvas = Canvas.Create(new MockRenderer(), registry);

        var control = new MockControl();
        var template1Calls = 0;
        var template2Calls = 0;

        control.Template.Value = ControlTemplate.Create<MockControl>(_ =>
        {
            template1Calls++;
            return new MockVisual();
        });

        canvas.Child = control;

        control.Template.Value = ControlTemplate.Create<MockControl>(_ =>
        {
            template2Calls++;
            return new MockVisual();
        });

        Assert.Equal(expected: 1, template1Calls);
        Assert.Equal(expected: 1, template2Calls);
    }
    
    [Fact]
    public void IsHovered_BeforeVisualized_ReturnsFalse()
    {
        MockControl control = new();

        Assert.False(control.IsHovered.GetValue());
    }

    [Fact]
    public void IsHovered_WhenPointerMovesOver_ReturnsTrue()
    {
        using var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator translator = new(canvas);

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        translator.PointerMove(control);

        Assert.True(control.IsHovered.GetValue());
    }

    [Fact]
    public void IsHovered_WhenPointerMovesOff_ReturnsFalse()
    {
        using var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator translator = new(canvas);

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        translator.PointerMove(control);
        translator.PointerMove(new PointF(x: -100f, y: -100f));

        Assert.False(control.IsHovered.GetValue());
    }
}
