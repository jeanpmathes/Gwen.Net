using System;
using System.Drawing;
using Gwen.Net.New.Controls;

namespace Gwen.Net.New.Input;

/// <summary>
/// Base class for an input translator, translating platform input events into GWEN input events.
/// This base class handles scaling of input positions based on the UI scaling factor.
/// </summary>
public abstract class InputTranslator
{
    private readonly Canvas canvas;
    
    /// <summary>
    /// Creates a new <seealso cref="InputTranslator"/> for the specified canvas.
    /// </summary>
    /// <param name="canvas">The canvas to which input events will be translated.</param>
    protected InputTranslator(Canvas canvas)
    {
        this.canvas = canvas;
    }
    
    private PointF ScalePosition(PointF position)
    {
        return new PointF(position.X / canvas.Scale, position.Y / canvas.Scale);
    }

    /// <summary>
    /// Processes a key up event and translates it into a GWEN key event.
    /// </summary>
    protected void ProcessKeyUpInput(Key key, Boolean isRepeat, ModifierKeys modifiers)
    {
        canvas.Input?.HandleKeyEvent(key, isDown: false, isRepeat, modifiers);
    }
    /// <summary>
    /// Processes a key down event and translates it into a GWEN key event.
    /// </summary>
    protected void ProcessKeyDownInput(Key key, Boolean isRepeat, ModifierKeys modifiers)
    {
        canvas.Input?.HandleKeyEvent(key, isDown: true, isRepeat, modifiers);
    }
    
    /// <summary>
    /// Processes a text input event and translates it into a GWEN text event.
    /// </summary>
    protected void ProcessTextInput(String text)
    {
        canvas.Input?.HandleTextEvent(text);
    }
    
    /// <summary>
    /// Processes a pointer button up event and translates it into a GWEN pointer button event.
    /// </summary>
    protected void ProcessPointerButtonUpInput(PointF position, PointerButton button, ModifierKeys modifiers)
    {
        canvas.Input?.HandlePointerButtonEvent(ScalePosition(position), button, isDown: false, modifiers);
    }
    
    /// <summary>
    /// Processes a pointer button down event and translates it into a GWEN pointer button event.
    /// </summary>
    protected void ProcessPointerButtonDownInput(PointF position, PointerButton button, ModifierKeys modifiers)
    {
        canvas.Input?.HandlePointerButtonEvent(ScalePosition(position), button, isDown: true, modifiers);
    }
    
    /// <summary>
    /// Processes a pointer move event and translates it into a GWEN pointer move event.
    /// </summary>
    protected void ProcessPointerMoveInput(PointF position, Single deltaX, Single deltaY)
    {
        canvas.Input?.HandlePointerMoveEvent(ScalePosition(position), deltaX / canvas.Scale, deltaY / canvas.Scale);
    }
    
    /// <summary>
    /// Processes a scroll event and translates it into a GWEN scroll event.
    /// </summary>
    protected void ProcessScrollInput(PointF position, Single deltaX, Single deltaY)
    {
        canvas.Input?.HandleScrollEvent(ScalePosition(position), deltaX, deltaY);
    }
}
