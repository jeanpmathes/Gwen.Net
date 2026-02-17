namespace Gwen.Net.New.Controls.Internals;

/// <summary>
/// A control which has at most one child control. Setting the <see cref="Child"/> property will replace any existing child control.
/// </summary>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class SingleChildControl<TControl> : Control<TControl> where TControl : SingleChildControl<TControl>
{
    /// <summary>
    /// Gets or sets the single child control.
    /// </summary>
    public Control? Child
    {
        get => Children.Count.GetValue() > 0 ? Children[0] : null;
        set => SetChild(value);
    }
}
