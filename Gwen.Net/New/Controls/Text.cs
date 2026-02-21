using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;

namespace Gwen.Net.New.Controls;

/// <summary>
/// Displays read-only text content.
/// </summary>
/// <seealso cref="Visuals.Text"/>
public class Text : TextBase<Text>
{
    /// <inheritdoc/>
    protected override ControlTemplate<Text> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Text>(control => new Visuals.Text
        {
            FontFamily = {Binding = Binding.Direct(control.FontFamily)},
            FontSize = {Binding = Binding.Direct(control.FontSize)},
            FontStyle = {Binding = Binding.Direct(control.FontStyle)},
            FontWeight = {Binding = Binding.Direct(control.FontWeight)},
            FontStretch = {Binding = Binding.Direct(control.FontStretch)},

            Wrapping = {Binding = Binding.Direct(control.TextWrapping)},
            Alignment = {Binding = Binding.Direct(control.TextAlignment)},
            Trimming = {Binding = Binding.Direct(control.TextTrimming)},
            LineHeight = {Binding = Binding.Direct(control.LineHeight)},

            Content = {Binding = Binding.Direct(control.Content)}
        });
    }
}
