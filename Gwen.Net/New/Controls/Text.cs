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
            FontFamily = {Binding = Binding.To(control.FontFamily)},
            FontSize = {Binding = Binding.To(control.FontSize)},
            FontStyle = {Binding = Binding.To(control.FontStyle)},
            FontWeight = {Binding = Binding.To(control.FontWeight)},
            FontStretch = {Binding = Binding.To(control.FontStretch)},

            Wrapping = {Binding = Binding.To(control.TextWrapping)},
            Alignment = {Binding = Binding.To(control.TextAlignment)},
            Trimming = {Binding = Binding.To(control.TextTrimming)},
            LineHeight = {Binding = Binding.To(control.LineHeight)},

            Content = {Binding = Binding.To(control.Content)}
        });
    }
}
