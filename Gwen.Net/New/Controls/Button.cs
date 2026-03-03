using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

internal static class ButtonDefaults
{
    public static readonly Brush FocusedBorderBrush = new SolidColorBrush(Color.Blue);
    
    public static readonly Brush HoveredBackground = new SolidColorBrush(Color.LightGray);
    public static readonly Brush PressedBackground = new SolidColorBrush(Color.Gray);
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
        BorderBrush = new Property<Brush>(this, Binding.To(Foreground).Combine(IsKeyboardFocused).Compute(
            (foreground, isFocused) => isFocused ? ButtonDefaults.FocusedBorderBrush : foreground));
        
        Background.OverrideDefault(old => old
            .Combine(IsPressed, IsHovered)
            .Compute((background, isPressed, isHovered) => isPressed 
                    ? ButtonDefaults.PressedBackground 
                    : isHovered 
                        ? ButtonDefaults.HoveredBackground 
                        : background));
        
        IsNavigable.OverrideDefault(defaultValue: true);
    }
    
    #region PROPERTIES
    
    /// <summary>
    /// The brush used to draw the border of the button.
    /// </summary>
    public Property<Brush> BorderBrush { get; }
    
    #endregion PROPERTIES
    
    /// <inheritdoc />
    protected override ControlTemplate<Button<TContent>> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<Button<TContent>>(static control => new Visuals.Border
        {
            BorderBrush = {Binding = Binding.To(control.BorderBrush)},
            
            Child = new ChildPresenter()
        });
    }
}
