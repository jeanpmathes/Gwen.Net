using System;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Commands;
using Gwen.Net.New.Input;

namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// Abstract base class for a button control, which is a control that can be clicked to perform an action.
/// </summary>
/// <typeparam name="TContent">The type of the content displayed on the button.</typeparam>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class ButtonBase<TContent, TControl> : ContentControlBase<TContent, TControl> where TContent : class where TControl : ButtonBase<TContent, TControl>
{
    /// <summary>
    /// Creates a new instance of the <see cref="ButtonBase{TContent, TControl}"/> class.
    /// </summary>
    protected ButtonBase()
    {
        Command = Property.Create(this, default(ICommand<TContent>?));
    }
    
    #region PROPERTIES
    
    /// <summary>
    /// The command to execute when the button is clicked.
    /// </summary>
    public Property<ICommand<TContent>?> Command { get; }
    
    #endregion PROPERTIES
    
    #region SLOTS
    
    private readonly Slot<Boolean> isPressed = new(false);
    
    /// <summary>
    /// Whether the button is currently pressed.
    /// </summary>
    public ReadOnlySlot<Boolean> IsPressed => isPressed;
    
    #endregion SLOTS

    /// <inheritdoc />
    public override void OnInput(InputEvent inputEvent)
    {
        switch (inputEvent)
        {
            case PointerButtonEvent {Button: PointerButton.Left, IsDown: true}:
            {
                isPressed.SetValue(true);
                
                Context.PointerFocus.Set(this);
                
                break;
            }
            
            case PointerButtonEvent { Button: PointerButton.Left, IsDown: false } pointerButtonEvent:
            {
                if (isPressed.GetValue())
                {
                    isPressed.SetValue(false);
                    
                    if (pointerButtonEvent.Hits(this))
                       ExecuteCommand();
                }
                
                Context.PointerFocus.Unset(this);
                
                break;
            }
            
            case KeyEvent { Key: Key.Return, IsDown: true }:
            {
                isPressed.SetValue(true);
                ExecuteCommand();
                isPressed.SetValue(false);
                
                break;
            }
            
            default:
                return;
        }
        
        inputEvent.Handled = true;
    }
    
    private void ExecuteCommand()
    {
        // todo: button should be shown as disabled if no content, if command not executable and if no command
        
        if (Content.GetValue() is {} content)
            Command.GetValue()?.Execute(content);
    }
}
