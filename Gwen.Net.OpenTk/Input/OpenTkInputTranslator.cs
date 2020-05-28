using System;
using Gwen.Net.Control;
using Gwen.Net.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;

using Key = OpenToolkit.Windowing.Common.Input.Key;

namespace Gwen.Net.OpenTk.Input
{
    public class OpenTkInputTranslator
    {
        private readonly Canvas canvas;
        private Vector2 lastMousePosition;

        bool controlPressed = false;

        public OpenTkInputTranslator(Canvas canvas)
        {
            this.canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
        }

        private GwenMappedKey TranslateKeyCode(Key key)
        {
            switch (key)
            {
                case Key.BackSpace: return GwenMappedKey.Backspace;
                case Key.Enter: return GwenMappedKey.Return;
                case Key.KeypadEnter: return GwenMappedKey.Return;
                case Key.Escape: return GwenMappedKey.Escape;
                case Key.Tab: return GwenMappedKey.Tab;
                case Key.Space: return GwenMappedKey.Space;
                case Key.Up: return GwenMappedKey.Up;
                case Key.Down: return GwenMappedKey.Down;
                case Key.Left: return GwenMappedKey.Left;
                case Key.Right: return GwenMappedKey.Right;
                case Key.Home: return GwenMappedKey.Home;
                case Key.End: return GwenMappedKey.End;
                case Key.Delete: return GwenMappedKey.Delete;
                case Key.LControl:
                    controlPressed = true;
                    return GwenMappedKey.Control;
                case Key.LAlt: return GwenMappedKey.Alt;
                case Key.LShift: return GwenMappedKey.Shift;
                case Key.RControl: return GwenMappedKey.Control;
                case Key.RAlt:
                    if (controlPressed)
                    {
                        canvas.Input_Key(GwenMappedKey.Control, false);
                    }
                    return GwenMappedKey.Alt;
                case Key.RShift: return GwenMappedKey.Shift;

            }
            return GwenMappedKey.Invalid;
        }

        private static char TranslateChar(Key key)
        {
            if (key >= Key.A && key <= Key.Z)
                return (char)('a' + ((int)key - (int)Key.A));
            return ' ';
        }

        public void ProcessMouseDown(MouseButtonEventArgs args)
        {
            if (canvas is null)
                return;

            if (args.Button == MouseButton.Left)
                canvas.Input_MouseButton(0, args.IsPressed);
            else if (args.Button == MouseButton.Right)
                canvas.Input_MouseButton(1, args.IsPressed);
        }

        public void ProcessMouseMove(MouseMoveEventArgs args)
        {
            if (null == canvas)
                return;

            var deltaPosition = args.Position - lastMousePosition;
            lastMousePosition = args.Position;

            canvas.Input_MouseMoved((int)lastMousePosition.X, (int)lastMousePosition.Y, (int)deltaPosition.X, (int)deltaPosition.Y);
        }

        public void ProcessMouseWheel(MouseWheelEventArgs args)
        {
            if (null == canvas)
                return;

            canvas.Input_MouseWheel((int)(args.OffsetY * 60));
        }

        public bool ProcessKeyDown(KeyboardKeyEventArgs eventArgs)
        {
            char ch = TranslateChar(eventArgs.Key);

            if (InputHandler.DoSpecialKeys(canvas, ch))
                return false;

            GwenMappedKey iKey = TranslateKeyCode(eventArgs.Key);
            if (iKey == GwenMappedKey.Invalid)
            {
                return canvas.Input_Character(ch);
            }

            return canvas.Input_Key(iKey, true);
        }

        public bool ProcessKeyUp(KeyboardKeyEventArgs eventArgs)
        {
            GwenMappedKey key = TranslateKeyCode(eventArgs.Key);

            return canvas.Input_Key(key, false);
        }
    }
}