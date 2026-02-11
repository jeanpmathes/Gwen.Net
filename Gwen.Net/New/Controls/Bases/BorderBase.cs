namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// Abstract base class for a border control, which is a control that draws a border and background for its child control.
/// </summary>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class BorderBase<TControl> : SingleChildControl<TControl> where TControl : BorderBase<TControl>;
