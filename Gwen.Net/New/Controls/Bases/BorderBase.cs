using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Internals;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// Abstract base class for a border control, which is a control that draws a border and background for its child control.
/// </summary>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class BorderBase<TControl> : SingleChildControl<TControl> where TControl : BorderBase<TControl>
{
    /// <summary>
    /// Creates a new instance of the <see cref="BorderBase{TControl}"/> class.
    /// </summary>
    protected BorderBase()
    {
        BorderThickness = Property.Create(this, ThicknessF.One);
    }

    #region PROPERTIES
    
    /// <summary>
    /// The thickness of the border drawn around the child control.
    /// </summary>
    public Property<ThicknessF> BorderThickness { get; }
    
    #endregion PROPERTIES
}
