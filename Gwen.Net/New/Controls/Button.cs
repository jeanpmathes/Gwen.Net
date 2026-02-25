using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

internal static class ButtonDefaults
{
    public static readonly Brush PressedBackground = new SolidColorBrush(Color.LightGray);
}

/// <summary>
/// A content control that can be clicked to perform an action.
/// </summary>
public class Button<TContent> : ButtonBase<TContent, Button<TContent>> where TContent : class
{
    /// <summary>
    /// Creates a new instance of the <see cref="Button{TContent}"/> class.
    /// </summary>
    public Button()
    {
        Background.OverrideDefaultBinding(old => old
            .Combine(IsPressed)
            .Compute((background, isPressed) => isPressed ? ButtonDefaults.PressedBackground : background));

        // todo: use light gray for hover
    }
    
    /// <inheritdoc />
    protected override ControlTemplate<Button<TContent>> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Button<TContent>>(static _ => new Visuals.Border
        {
            Child = new ChildPresenter()
        });
    }
}
