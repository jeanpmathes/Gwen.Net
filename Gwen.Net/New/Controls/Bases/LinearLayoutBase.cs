using Gwen.Net.New.Bindings;

namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// Abstract base class for linear layout controls, which arrange their children in a single line, either horizontally or vertically.
/// </summary>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class LinearLayoutBase<TControl> : LayoutBase<TControl> where TControl : LinearLayoutBase<TControl>
{
    /// <summary>
    /// Creates a new instance of the <see cref="LinearLayoutBase{TControl}"/> class.
    /// </summary>
    protected LinearLayoutBase()
    {
        Orientation = Property.Create(this, New.Orientation.Horizontal);
    }
    
    #region PROPERTIES

    /// <summary>
    /// The orientation of the layout, which determines whether the children are arranged horizontally or vertically.
    /// </summary>
    public Property<Orientation> Orientation { get; }

    #endregion PROPERTIES
}
