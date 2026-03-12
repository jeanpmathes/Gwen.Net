using System;
using System.Diagnostics;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
///     Class for managing focus. Focus is the state of an element being the target of input events, such as keyboard
///     events. Only one element can be focused at a time.
/// </summary>
public sealed class Focus
{
    private readonly Action<Visual?, Visual?> onFocusChanged;

    private Control? focusedControl;
    private Visual? focusedVisual;

    /// <summary>
    ///     Create a new <seealso cref="Focus" /> instance with the specified callback for when the focus changes. The callback
    ///     will be invoked whenever the focused visual or control changes, including when it is cleared.
    /// </summary>
    /// <param name="onFocusChanged">Callback to invoke when the focus changes, either the control or visual.</param>
    public Focus(Action<Visual?, Visual?> onFocusChanged)
    {
        this.onFocusChanged = onFocusChanged;
    }

    /// <summary>
    ///     Set the focused visual.
    /// </summary>
    /// <param name="visual">The visual to set as focused.</param>
    public void Set(Visual visual)
    {
        Visual? previousFocusedVisual = focusedVisual;

        ClearFocusedControl();
        SetFocusedVisual(visual);

        InvokeFocusChanged(previousFocusedVisual);
    }

    /// <summary>
    ///     Set the focused control. This will focus the template anchor of the control.
    /// </summary>
    /// <param name="control">The control to set as focused.</param>
    public void Set(Control control)
    {
        Visual? previousFocusedVisual = focusedVisual;

        ClearFocusedControl();
        SetFocusedControl(control);

        InvokeFocusChanged(previousFocusedVisual);
    }

    /// <summary>
    ///     Clear the focused visual only if it is the given visual.
    /// </summary>
    /// <param name="visual">The visual to clear focus from if it is currently focused.</param>
    public void Unset(Visual visual)
    {
        if (focusedVisual != visual)
            return;

        Clear();

        // The callback is already invoked in clear.
    }

    /// <summary>
    ///     Clear the focused control only if it is the given control.
    /// </summary>
    /// <param name="control">The control to clear focus from if it is currently focused.</param>
    public void Unset(Control control)
    {
        if (focusedControl != control)
            return;

        Clear();

        // The callback is already invoked in clear.
    }

    /// <summary>
    ///     Clear the focused visual and control. This will unfocus any currently focused element.
    /// </summary>
    public void Clear()
    {
        Visual? previousFocusedVisual = focusedVisual;

        ClearFocusedControl();

        InvokeFocusChanged(previousFocusedVisual);
    }

    /// <summary>
    ///     Get the currently focused visual.
    /// </summary>
    /// <returns>The currently focused visual, or <c>null</c> if no visual is focused.</returns>
    public Visual? GetFocused()
    {
        return focusedVisual;
    }

    private void SetFocusedControl(Control control)
    {
        focusedControl = control;

        focusedControl.Visualization.ValueChanged += OnVisualizationChanged;

        SetFocusedVisual(control.Visualization.GetValue());
    }

    private void ClearFocusedControl()
    {
        if (focusedControl != null)
            focusedControl.Visualization.ValueChanged -= OnVisualizationChanged;

        focusedControl = null;
        ClearFocusedVisual();
    }

    private void SetFocusedVisual(Visual? visual)
    {
        focusedVisual = visual;

        if (focusedVisual == null)
            return;

        if (!CanFocus(focusedVisual))
        {
            focusedVisual = null;

            ClearFocusedControl();
        }
        else
        {
            focusedVisual.Visibility.ValueChanged += OnVisibilityOrEnablementChanged;
            focusedVisual.Enablement.ValueChanged += OnVisibilityOrEnablementChanged;
        }
    }

    private void ClearFocusedVisual()
    {
        if (focusedVisual != null)
        {
            focusedVisual.Visibility.ValueChanged -= OnVisibilityOrEnablementChanged;
            focusedVisual.Enablement.ValueChanged -= OnVisibilityOrEnablementChanged;
        }

        focusedVisual = null;
    }

    private void OnVisualizationChanged(Object? sender, EventArgs e)
    {
        Visual? previousFocusedVisual = focusedVisual;

        ClearFocusedVisual();
        SetFocusedVisual(focusedControl?.Visualization.GetValue());

        InvokeFocusChanged(previousFocusedVisual);
    }

    private void OnVisibilityOrEnablementChanged(Object? sender, EventArgs e)
    {
        Debug.Assert(focusedVisual != null);

        if (!CanFocus(focusedVisual)) Clear();
    }

    private void InvokeFocusChanged(Visual? previousFocusedVisual)
    {
        Visual? newFocusedVisual = focusedVisual;

        if (previousFocusedVisual == newFocusedVisual)
            return;

        onFocusChanged(previousFocusedVisual, newFocusedVisual);
    }

    /// <summary>
    ///     Check whether a visual can currently receive focus.
    ///     Note that this can change dynamically.
    /// </summary>
    /// <param name="visual">The visual to check.</param>
    /// <returns><c>true</c> if the visual can receive focus, <c>false</c> otherwise.</returns>
    public static Boolean CanFocus(Visual visual)
    {
        return visual.Enablement.GetValue().IsFocusable && visual.Visibility.GetValue().IsVisible;
    }
}
