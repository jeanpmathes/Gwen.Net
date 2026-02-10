using System;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls.Templates;

/// <summary>
/// Utility class for control templates.
/// </summary>
public static class ControlTemplate
{
    /// <summary>
    /// Creates a new control template from the given function.
    /// </summary>
    /// <param name="function">The function that creates the visual structure for the control.</param>
    /// <typeparam name="TControl">The type of the templated control.</typeparam>
    /// <returns>The created control template.</returns>
    public static ControlTemplate<TControl> Create<TControl>(Func<TControl, Visual> function) where TControl : Control<TControl>
    {
        return new ControlTemplate<TControl>(function);
    }
}

/// <summary>
/// A template defines how a control is visually structured.
/// </summary>
/// <typeparam name="TControl">The type of the templated control.</typeparam>
public class ControlTemplate<TControl> where TControl : Control<TControl>
{
    private readonly Func<TControl, Visual> function;
    
    /// <summary>
    /// Creates a new control template.
    /// </summary>
    /// <param name="function">The function that creates the visual structure for the control.</param>
    public ControlTemplate(Func<TControl, Visual> function)
    {
        this.function = function;
    }
    
    /// <summary>
    /// Applies the template to the given control, creating its visual structure.
    /// </summary>
    /// <param name="control">The control to apply the template to.</param>
    /// <returns>The created visual structure.</returns>
    public Visual Apply(TControl control)
    {
        return function(control);
    }
}
