using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// A <see cref="Border"/> draws a border and background around its child control.
/// </summary>
/// <seealso cref="Visuals.Border"/>
public class Border : Control<Border>
{
    /// <summary>
    /// Gets or sets the single child control.
    /// </summary>
    public Control? Child
    {
        get => Children.Count > 0 ? Children[0] : null;
        set => SetChild(value);
    } 
    
    /// <inheritdoc />
    protected override ControlTemplate<Border> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Border>(static _ => new Visuals.Border
        {
            Child = new ChildPresenter()
        });
    }
}
