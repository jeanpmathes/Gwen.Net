using System.Drawing;
using Gwen.Net.New;
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
    private readonly Focus focus = new((_, _) => {});

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

    [Fact]
    public void Set_Control_GetFocusedReturnsItsVisualization()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(control);

        Assert.Same(control.Visualization.GetValue(), focus.GetFocused());
    }

    [Fact]
    public void Set_HiddenControl_DoesNotThrowAndDoesNotFocus()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        control.Visibility.Value = Visibility.Hidden;

        Exception? exception = Record.Exception(() => focus.Set(control));

        Assert.Null(exception);
        Assert.Null(focus.GetFocused());
    }

    [Fact]
    public void Unset_FocusedControl_ClearsFocus()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

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
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl focused = new("focused");
        MockControl other = new("other");
        canvas.Child = focused;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(focused);
        focus.Unset(other);

        Assert.NotNull(focus.GetFocused());
    }

    [Fact]
    public void Set_Visual_InvokesCallback()
    {
        Int32 callbackCount = 0;
        Focus callbackFocus = new((_, _) => callbackCount++);
        MockVisual visual = new();

        callbackFocus.Set(visual);

        Assert.Equal(expected: 1, callbackCount);
    }

    [Fact]
    public void Set_Visual_TwiceCalls_InvokesCallbackTwice()
    {
        Int32 callbackCount = 0;
        Focus callbackFocus = new((_, _) => callbackCount++);
        MockVisual first = new();
        MockVisual second = new();

        callbackFocus.Set(first);
        callbackFocus.Set(second);

        Assert.Equal(expected: 2, callbackCount);
    }

    [Fact]
    public void Clear_DoesNotInvokeCallbackIfAlreadyClear()
    {
        Int32 callbackCount = 0;
        Focus callbackFocus = new((_, _) => callbackCount++);

        callbackFocus.Clear();

        Assert.Equal(expected: 0, callbackCount);
    }

    [Fact]
    public void Clear_InvokesCallbackIfFocused()
    {
        Int32 callbackCount = 0;
        Focus callbackFocus = new((_, _) => callbackCount++);
        MockVisual visual = new();

        callbackFocus.Set(visual);
        callbackFocus.Clear();

        Assert.Equal(expected: 2, callbackCount);
    }

    [Fact]
    public void Unset_FocusedVisual_InvokesCallback()
    {
        Int32 callbackCount = 0;
        // ReSharper disable once AccessToModifiedClosure
        Focus callbackFocus = new((_, _) => callbackCount++);
        MockVisual visual = new();

        callbackFocus.Set(visual);
        callbackCount = 0;

        callbackFocus.Unset(visual);

        Assert.Equal(expected: 1, callbackCount);
    }

    [Fact]
    public void Unset_UnfocusedVisual_DoesNotInvokeCallback()
    {
        Int32 callbackCount = 0;
        // ReSharper disable once AccessToModifiedClosure
        Focus callbackFocus = new((_, _) => callbackCount++);
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
        Int32 callbackCount = 0;
        Focus callbackFocus = new((_, _) => callbackCount++);

        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
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
        Int32 callbackCount = 0;
        // ReSharper disable once AccessToModifiedClosure
        Focus callbackFocus = new((_, _) => callbackCount++);

        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        callbackFocus.Set(control);
        callbackCount = 0;

        control.Template.Value = ControlTemplate.Create<MockControl>(_ => new MockVisual());

        Assert.True(callbackCount >= 1);
    }

    [Fact]
    public void Focus_IsClearedWhenControlIsHidden()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(control);
        Assert.NotNull(focus.GetFocused());

        control.Visibility.Value = Visibility.Hidden;

        Assert.Null(focus.GetFocused());
    }

    [Fact]
    public void Focus_IsClearedWhenVisualIsHidden()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(control);
        Assert.NotNull(focus.GetFocused());

        control.Visualization.GetValue()?.Visibility.Value = Visibility.Hidden;

        Assert.Null(focus.GetFocused());
    }

    [Fact]
    public void Focus_IsClearedWhenControlIsDisabled()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(control);
        Assert.NotNull(focus.GetFocused());

        control.Enablement.Value = Enablement.Disabled;

        Assert.Null(focus.GetFocused());
    }

    [Fact]
    public void Focus_IsClearedWhenVisualIsDisabled()
    {
        using Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        MockControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        focus.Set(control);
        Assert.NotNull(focus.GetFocused());

        control.Visualization.GetValue()?.Enablement.Value = Enablement.Disabled;

        Assert.Null(focus.GetFocused());
    }
}
