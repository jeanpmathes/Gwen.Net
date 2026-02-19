using System;
using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Input;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Gwen.Net.OpenTk.New.Input;

public sealed class InputTranslator : Gwen.Net.New.Input.InputTranslator, IDisposable
{
    private readonly GameWindow window;
    
    private PointF lastMousePosition;
    
    public InputTranslator(GameWindow window, Canvas canvas) : base(canvas)
    {
        this.window = window;
        
        window.KeyUp += OnKeyUp;
        window.KeyDown += OnKeyDown;
        window.TextInput += OnTextInput;
        window.MouseUp += OnMouseUp;
        window.MouseDown += OnMouseDown;
        window.MouseMove += OnMouseMove;
        window.MouseWheel += OnMouseWheel;
    }

    public void Dispose()
    {
        window.KeyUp -= OnKeyUp;
        window.KeyDown -= OnKeyDown;
        window.TextInput -= OnTextInput;
        window.MouseUp -= OnMouseUp;
        window.MouseDown -= OnMouseDown;
        window.MouseMove -= OnMouseMove;
        window.MouseWheel -= OnMouseWheel;
    }
    
    private static Key TranslateKeyCode(Keys key)
    {
        return key switch
        {
            Keys.Backspace => Key.Backspace,
            Keys.Enter or Keys.KeyPadEnter => Key.Return,
            Keys.Escape => Key.Escape,
            Keys.Tab => Key.Tab,
            Keys.Space => Key.Space,
            Keys.Up => Key.Up,
            Keys.Down => Key.Down,
            Keys.Left => Key.Left,
            Keys.Right => Key.Right,
            Keys.PageUp => Key.PageUp,
            Keys.PageDown => Key.PageDown,
            Keys.Home => Key.Home,
            Keys.End => Key.End,
            Keys.Delete => Key.Delete,
            Keys.Insert => Key.Insert,
            Keys.LeftControl or Keys.RightControl => Key.Control,
            Keys.LeftAlt or Keys.RightAlt => Key.Alt,
            Keys.LeftShift or Keys.RightShift => Key.Shift,
            
            Keys.A => Key.A,
            Keys.B => Key.B,
            Keys.C => Key.C,
            Keys.D => Key.D,
            Keys.E => Key.E,
            Keys.F => Key.F,
            Keys.G => Key.G,
            Keys.H => Key.H,
            Keys.I => Key.I,
            Keys.J => Key.J,
            Keys.K => Key.K,
            Keys.L => Key.L,
            Keys.M => Key.M,
            Keys.N => Key.N,
            Keys.O => Key.O,
            Keys.P => Key.P,
            Keys.Q => Key.Q,
            Keys.R => Key.R,
            Keys.S => Key.S,
            Keys.T => Key.T,
            Keys.U => Key.U,
            Keys.V => Key.V,
            Keys.W => Key.W,
            Keys.X => Key.X,
            Keys.Y => Key.Y,
            Keys.Z => Key.Z,
            
            _ => Key.Invalid
        };
    }

    private static ModifierKeys TranslateModifierKeys(KeyModifiers modifiers)
    {
        var modifierKeys = ModifierKeys.None;
        
        if (modifiers.HasFlag(KeyModifiers.Alt))
            modifierKeys |= ModifierKeys.Alt;
        
        if (modifiers.HasFlag(KeyModifiers.Control))
            modifierKeys |= ModifierKeys.Control;
        
        if (modifiers.HasFlag(KeyModifiers.Shift))
            modifierKeys |= ModifierKeys.Shift;
        
        return modifierKeys;
    }

    private static PointerButton TranslateMouseButton(MouseButton button)
    {
        return button switch
        {
            MouseButton.Left => PointerButton.Left,
            MouseButton.Right => PointerButton.Right,
            MouseButton.Middle => PointerButton.Middle,
            _ => PointerButton.Invalid
        };
    }
    
    private void OnKeyUp(KeyboardKeyEventArgs args)
    {
        Key key = TranslateKeyCode(args.Key);
        
        if (key == Key.Invalid) 
            return;
        
        ProcessKeyUpInput(key, args.IsRepeat, TranslateModifierKeys(args.Modifiers));
    }

    private void OnKeyDown(KeyboardKeyEventArgs args)
    {
        Key key = TranslateKeyCode(args.Key);
        
        if (key == Key.Invalid) 
            return;
        
        ProcessKeyDownInput(key, args.IsRepeat, TranslateModifierKeys(args.Modifiers));
    }

    private void OnTextInput(TextInputEventArgs args)
    {
        ProcessTextInput(args.AsString);
    }

    private void OnMouseDown(MouseButtonEventArgs args)
    {
        ProcessPointerButtonDownInput(lastMousePosition, TranslateMouseButton(args.Button), TranslateModifierKeys(args.Modifiers));
    }

    private void OnMouseUp(MouseButtonEventArgs args)
    {
        ProcessPointerButtonUpInput(lastMousePosition, TranslateMouseButton(args.Button), TranslateModifierKeys(args.Modifiers));
    }

    private void OnMouseMove(MouseMoveEventArgs args)
    {
        lastMousePosition = new PointF(args.X, args.Y);
        
        ProcessPointerMoveInput(lastMousePosition, args.DeltaX, args.DeltaY);
    }

    private void OnMouseWheel(MouseWheelEventArgs args)
    {
        ProcessScrollInput(lastMousePosition, args.OffsetX, args.OffsetY);
    }
}
