using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Input;
using Gwen.Net.New.Resources;
using Gwen.Net.Tests.Unit.New.Controls;
using Gwen.Net.Tests.Unit.New.Rendering;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Input;

public sealed class FocusTests
{
    private readonly Focus focus = new();
    
    #region SET / GET VISUAL

    [Fact]
    public void Set_Visual_GetFocusedReturnsIt()
    {
        MockVisual visual = new();

        focus.Set(visual);

        Assert.Same(visual, focus.GetFocused());
    }

    [Fact]
    public void Set_DifferentVisual_ReplacesExisting()
    {
        MockVisual first = new();
        MockVisual second = new();

        focus.Set(first);
        focus.Set(second);

        Assert.Same(second, focus.GetFocused());
    }

    #endregion SET / GET VISUAL

    #region UNSET VISUAL

    [Fact]
    public void Unset_FocusedVisual_ClearsFocus()
    {
        MockVisual visual = new();

        focus.Set(visual);
        focus.Unset(visual);

        Assert.Null(focus.GetFocused());
    }

    [Fact]
    public void Unset_UnfocusedVisual_DoesNotClearFocus()
    {
        MockVisual focused = new();
        MockVisual other = new();

        focus.Set(focused);
        focus.Unset(other);

        Assert.Same(focused, focus.GetFocused());
    }

    #endregion UNSET VISUAL

    #region CLEAR

    [Fact]
    public void Clear_WhenVisualFocused_ClearsFocus()
    {
        MockVisual visual = new();

        focus.Set(visual);
        focus.Clear();

        Assert.Null(focus.GetFocused());
    }

    [Fact]
    public void GetFocused_WhenNothingFocused_ReturnsNull()
    {
        Assert.Null(focus.GetFocused());
    }

    #endregion CLEAR

    #region SET CONTROL

    [Fact]
    public void Set_Control_GetFocusedReturnsItsVisualization()
    {
        using var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(control);

        Assert.Same(control.Visualization.GetValue(), focus.GetFocused());
    }

    [Fact]
    public void Unset_FocusedControl_ClearsFocus()
    {
        using var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(control);
        focus.Unset(control);

        Assert.Null(focus.GetFocused());
    }

    [Fact]
    public void Unset_UnfocusedControl_DoesNotClearFocus()
    {
        using var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl focused = new("focused");
        MockControl other = new("other");
        canvas.Child = focused;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(focused);
        focus.Unset(other);

        Assert.NotNull(focus.GetFocused());
    }

    #endregion SET CONTROL
}
