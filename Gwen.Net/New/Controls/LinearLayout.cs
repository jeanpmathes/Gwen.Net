using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;

namespace Gwen.Net.New.Controls;

/// <summary>
/// A linear layout arranges its children in a single line, either horizontally or vertically.
/// </summary>
/// <seealso cref="Visuals.LinearLayout"/>
public class LinearLayout : LinearLayoutBase<LinearLayout>
{
    /// <inheritdoc/>
    protected override ControlTemplate<LinearLayout> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<LinearLayout>(control => new Visuals.LinearLayout
        {
            Orientation = {Binding = Binding.Direct(control.Orientation)}
        });
    }
}
