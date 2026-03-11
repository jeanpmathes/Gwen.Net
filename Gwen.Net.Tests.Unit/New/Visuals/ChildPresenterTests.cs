using System.Drawing;
using Gwen.Net.New;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Visuals;
using Gwen.Net.Tests.Unit.New.Controls;
using Gwen.Net.Tests.Unit.New.Rendering;
using Canvas = Gwen.Net.New.Controls.Canvas;
using BorderVisual = Gwen.Net.New.Visuals.Border;
using ControlBorder = Gwen.Net.New.Controls.Border;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class ChildPresenterTests() : VisualTestBase<ChildPresenter>(() => new ChildPresenter())
{
    [Fact]
    public void OnAttach_WithoutChild_IsCollapsed()
    {
        using ResourceRegistry registry = new();
        using Canvas canvas = Canvas.Create(new MockRenderer(), registry);

        ControlBorder control = new();

        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 200, height: 200));
        canvas.Render();

        ChildPresenter presenter = GetPresenter(control);

        Assert.Equal(Visibility.Collapsed, presenter.Visibility.GetValue());
    }

    [Fact]
    public void ChildAddedAndRemoved_TogglesVisibility()
    {
        using ResourceRegistry registry = new();
        using Canvas canvas = Canvas.Create(new MockRenderer(), registry);

        ControlBorder control = new();

        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 200, height: 200));
        canvas.Render();

        ChildPresenter presenter = GetPresenter(control);

        Assert.Equal(Visibility.Collapsed, presenter.Visibility.GetValue());

        control.Child = new MockControl();
        Assert.Equal(Visibility.Visible, presenter.Visibility.GetValue());

        control.Child = null;
        Assert.Equal(Visibility.Collapsed, presenter.Visibility.GetValue());
    }

    private static ChildPresenter GetPresenter(ControlBorder control)
    {
        BorderVisual rootVisualization = Assert.IsType<BorderVisual>(control.Visualization.GetValue());
        return Assert.IsType<ChildPresenter>(rootVisualization.Child);
    }
}
