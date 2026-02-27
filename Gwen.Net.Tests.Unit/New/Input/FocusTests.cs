using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Input;
using Gwen.Net.New.Resources;
using Gwen.Net.Tests.Unit.New.Controls;
using Gwen.Net.Tests.Unit.New.Rendering;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Input;

public sealed class FocusTests
{
    private readonly Focus focus = new(() => {});
    
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
    
    #region CALLBACK

    [Fact]
    public void Set_Visual_InvokesCallback()
    {
        var callbackCount = 0;
        Focus callbackFocus = new(() => callbackCount++);
        MockVisual visual = new();

        callbackFocus.Set(visual);

        Assert.Equal(expected: 1, callbackCount);
    }

    [Fact]
    public void Set_Visual_TwiceCalls_InvokesCallbackTwice()
    {
        var callbackCount = 0;
        Focus callbackFocus = new(() => callbackCount++);
        MockVisual first = new();
        MockVisual second = new();

        callbackFocus.Set(first);
        callbackFocus.Set(second);

        Assert.Equal(expected: 2, callbackCount);
    }

    [Fact]
    public void Clear_InvokesCallback()
    {
        var callbackCount = 0;
        Focus callbackFocus = new(() => callbackCount++);

        callbackFocus.Clear();

        Assert.Equal(expected: 1, callbackCount);
    }

    [Fact]
    public void Unset_FocusedVisual_InvokesCallback()
    {
        var callbackCount = 0;
        // ReSharper disable once AccessToModifiedClosure
        Focus callbackFocus = new(() => callbackCount++);
        MockVisual visual = new();

        callbackFocus.Set(visual);
        callbackCount = 0;

        callbackFocus.Unset(visual);

        Assert.Equal(expected: 1, callbackCount);
    }

    [Fact]
    public void Unset_UnfocusedVisual_DoesNotInvokeCallback()
    {
        var callbackCount = 0;
        // ReSharper disable once AccessToModifiedClosure
        Focus callbackFocus = new(() => callbackCount++);
        MockVisual focused = new();
        MockVisual other = new();

        callbackFocus.Set(focused);
        callbackCount = 0;

        callbackFocus.Unset(other);

        Assert.Equal(expected: 0, callbackCount);
    }

    [Fact]
    public void Set_Control_InvokesCallback()
    {
        var callbackCount = 0;
        Focus callbackFocus = new(() => callbackCount++);

        using var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        callbackFocus.Set(control);

        Assert.Equal(expected: 1, callbackCount);
    }

    [Fact]
    public void ControlVisualizationChange_InvokesCallback()
    {
        var callbackCount = 0;
        // ReSharper disable once AccessToModifiedClosure
        Focus callbackFocus = new(() => callbackCount++);

        using var canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        callbackFocus.Set(control);
        callbackCount = 0;

        control.Template.Value = ControlTemplate.Create<MockControl>(_ => new MockVisual());

        Assert.True(callbackCount >= 1);
    }

    #endregion CALLBACK
}
