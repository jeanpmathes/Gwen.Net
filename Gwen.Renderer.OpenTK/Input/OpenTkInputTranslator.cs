using Gwen.Control;
using Gwen.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

using Key = OpenToolkit.Windowing.Common.Input.Key;

namespace Gwen.Renderer.OpenTK.Input
{
    public class OpenTkInputTranslator
    {
        private Canvas canvas = null;
        private Vector2 lastMousePosition;

        bool m_AltGr = false;

        public OpenTkInputTranslator(GameWindow window)
        {
        }

        public void Initialize(Canvas canvas)
        {
            this.canvas = canvas;
        }

        /// <summary>
        /// Translates control key's OpenTK key code to GWEN's code.
        /// </summary>
        /// <param name="key">OpenTK key code.</param>
        /// <returns>GWEN key code.</returns>
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
                    this.m_AltGr = true;
                    return GwenMappedKey.Control;
                case Key.LAlt: return GwenMappedKey.Alt;
                case Key.LShift: return GwenMappedKey.Shift;
                case Key.RControl: return GwenMappedKey.Control;
                case Key.RAlt:
                    if (this.m_AltGr)
                    {
                        this.canvas.Input_Key(GwenMappedKey.Control, false);
                    }
                    return GwenMappedKey.Alt;
                case Key.RShift: return GwenMappedKey.Shift;

            }
            return GwenMappedKey.Invalid;
        }

        /// <summary>
        /// Translates alphanumeric OpenTK key code to character value.
        /// </summary>
        /// <param name="key">OpenTK key code.</param>
        /// <returns>Translated character.</returns>
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

            int ButtonID = -1; //Do not trigger event.

            if (args.Button == MouseButton.Left)
                ButtonID = 0;
            else if (args.Button == MouseButton.Right)
                ButtonID = 1;

            if (ButtonID != -1) //We only care about left and right click for now
                canvas.Input_MouseButton(ButtonID, args.IsPressed);
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
            if(iKey == GwenMappedKey.Invalid)
            {
                return canvas.Input_Character(ch);
            }

            return canvas.Input_Key(iKey, true);
        }

        public bool ProcessKeyUp(KeyboardKeyEventArgs eventArgs)
        {
            GwenMappedKey iKey = TranslateKeyCode(eventArgs.Key);

            return canvas.Input_Key(iKey, false);
        }
    }
}
