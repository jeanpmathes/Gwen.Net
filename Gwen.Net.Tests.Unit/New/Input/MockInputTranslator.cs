using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Input;

namespace Gwen.Net.Tests.Unit.New.Input;

public sealed class MockInputTranslator(Canvas canvas) : InputTranslator(canvas)
{
    public void KeyDown(Key key, Boolean isRepeat = false, ModifierKeys modifiers = ModifierKeys.None)
    {
        ProcessKeyDownInput(key, isRepeat, modifiers);
    }

    public void KeyUp(Key key, Boolean isRepeat = false, ModifierKeys modifiers = ModifierKeys.None)
    {
        ProcessKeyUpInput(key, isRepeat, modifiers);
    }

    public void Text(String text)
    {
        ProcessTextInput(text);
    }

    public void PointerButtonDown(PointF position, PointerButton button = PointerButton.Left, ModifierKeys modifiers = ModifierKeys.None)
    {
        ProcessPointerButtonDownInput(position, button, modifiers);
    }

    public void PointerButtonUp(PointF position, PointerButton button = PointerButton.Left, ModifierKeys modifiers = ModifierKeys.None)
    {
        ProcessPointerButtonUpInput(position, button, modifiers);
    }

    public void PointerMove(PointF position, Single deltaX = 0f, Single deltaY = 0f)
    {
        ProcessPointerMoveInput(position, deltaX, deltaY);
    }

    public void Scroll(PointF position, Single deltaX = 0f, Single deltaY = 0f)
    {
        ProcessScrollInput(position, deltaX, deltaY);
    }
}
