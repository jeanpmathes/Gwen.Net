using System.Drawing;
using Gwen.Net.New;
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
        using ResourceRegistry registry = new();
        registry.AddStyle<Border>(builder => builder.Set(c => c.MinimumWidth, value: 25f));

        using Canvas canvas = Canvas.Create(new MockRenderer(), registry);
        Border border = new();

        canvas.Child = border;

        Assert.Equal(expected: 25f, border.MinimumWidth.GetValue());
    }

    [Fact]
    public void LocalStyle_IsAppliedAfterOuterStyles()
    {
        using ResourceRegistry registry = new();
        registry.AddStyle<Border>(builder => builder.Set(c => c.MinimumWidth, value: 50f));

        using Canvas canvas = Canvas.Create(new MockRenderer(), registry);

        Border border = new()
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
        using ResourceRegistry registry = new();
        using Canvas canvas = Canvas.Create(new MockRenderer(), registry);

        MockControl control = new();
        Int32 template1Calls = 0;
        Int32 template2Calls = 0;

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
    public void Visibility_WhenParentIsHidden_IsCoercedToHidden()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockControl child = new();
        canvas.Child = child;

        canvas.Visibility.Value = Visibility.Hidden;

        Assert.Equal(Visibility.Hidden, child.Visibility.GetValue());
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
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
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
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
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
