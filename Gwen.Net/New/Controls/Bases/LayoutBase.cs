using Gwen.Net.New.Controls.Internals;

namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// Abstract base class for all layout controls, which arrange their children in a specific way.
/// </summary>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class LayoutBase<TControl> : MultiChildControl<TControl> where TControl : LayoutBase<TControl>;
