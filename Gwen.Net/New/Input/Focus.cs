using System;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Input;

/// <summary>
/// Class for managing focus. Focus is the state of an element being the target of input events, such as keyboard events. Only one element can be focused at a time.
/// </summary>
public sealed class Focus
{
    private Control? focusedControl;
    private Visual? focusedVisual;

    /// <summary>
    /// Set the focused visual.
    /// </summary>
    /// <param name="visual">The visual to set as focused.</param>
    public void Set(Visual visual)
    {
        ClearFocusedControl();
        focusedVisual = visual;
    }

    /// <summary>
    /// Set the focused control. This will focus the template anchor of the control.
    /// </summary>
    /// <param name="control">The control to set as focused.</param>
    public void Set(Control control)
    {
        ClearFocusedControl();
        SetFocusedControl(control);
    }
    
    /// <summary>
    /// Clear the focused visual only if it is the given visual.
    /// </summary>
    /// <param name="visual">The visual to clear focus from if it is currently focused.</param>
    public void Unset(Visual visual)
    {
        if (focusedVisual != visual)
            return;
        
        Clear();
    }
    
    /// <summary>
    /// Clear the focused control only if it is the given control.
    /// </summary>
    /// <param name="control">The control to clear focus from if it is currently focused.</param>
    public void Unset(Control control)
    {
        if (focusedControl != control)
            return;
        
        Clear();
    }
    
    /// <summary>
    /// Clear the focused visual and control. This will unfocus any currently focused element.
    /// </summary>
    public void Clear()
    {
        ClearFocusedControl();
        focusedVisual = null;
    }
    
    /// <summary>
    /// Get the currently focused visual.
    /// </summary>
    /// <returns>The currently focused visual, or <c>null</c> if no visual is focused.</returns>
    public Visual? GetFocused()
    {
        return focusedVisual;
    }
    
    private void SetFocusedControl(Control control)
    {
        focusedControl = control;
        focusedVisual = control.Visualization.GetValue();
        
        focusedControl.Visualization.ValueChanged += OnVisualizationChanged;
    }

    private void ClearFocusedControl()
    {
        if (focusedControl != null)
            focusedControl.Visualization.ValueChanged -= OnVisualizationChanged;
        
        focusedControl = null;
    }

    private void OnVisualizationChanged(Object? sender, EventArgs e)
    {
        focusedVisual = focusedControl?.Visualization.GetValue();
    }
}
