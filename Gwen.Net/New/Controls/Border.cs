using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// A <see cref="Border"/> draws a border and background around its child control.
/// </summary>
/// <seealso cref="Visuals.Border"/>
public class Border : BorderBase<Border>
{
    /// <inheritdoc />
    protected override ControlTemplate<Border> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Border>(static _ => new Visuals.Border
        {
            Child = new ChildPresenter()
        });
    }
}
