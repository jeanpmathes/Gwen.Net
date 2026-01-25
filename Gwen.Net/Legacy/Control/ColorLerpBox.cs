using System;
using Gwen.Net.Legacy.Input;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     Linear-interpolated HSV color box.
    /// </summary>
    public class ColorLerpBox : ControlBase
    {
        private Point cursorPos;
        private Boolean depressed;
        private Single hue;
        private Texture texture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorLerpBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorLerpBox(ControlBase parent) : base(parent)
        {
            SetColor(new Color(a: 255, r: 255, g: 128, b: 0));
            MouseInputEnabled = true;
            depressed = false;

            // texture is initialized in Render() if null
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor => GetColorAt(cursorPos.X, cursorPos.Y);

        /// <summary>
        ///     Invoked when the selected color has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ColorChanged;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (texture != null)
            {
                texture.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        ///     Linear color interpolation.
        /// </summary>
        public static Color Lerp(Color toColor, Color fromColor, Single amount)
        {
            Color delta = toColor.Subtract(fromColor);
            delta = delta.Multiply(amount);

            return fromColor.Add(delta);
        }

        /// <summary>
        ///     Sets the selected color.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="onlyHue">Determines whether to only set H value (not SV).</param>
        /// <param name="doEvents">Determines whether to invoke the ColorChanged event.</param>
        public void SetColor(Color value, Boolean onlyHue = true, Boolean doEvents = true)
        {
            var hsv = value.ToHSV();
            hue = hsv.H;

            if (!onlyHue)
            {
                cursorPos.X = (Int32) (hsv.S * ActualWidth);
                cursorPos.Y = (Int32) ((1 - hsv.V) * ActualHeight);
            }

            InvalidateTexture();

            if (doEvents && ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Handler invoked on mouse moved event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="dx">X change.</param>
        /// <param name="dy">Y change.</param>
        protected override void OnMouseMoved(Int32 x, Int32 y, Int32 dx, Int32 dy)
        {
            if (depressed)
            {
                cursorPos = CanvasPosToLocal(new Point(x, y));

                //Do we have clamp?
                if (cursorPos.X < 0)
                {
                    cursorPos.X = 0;
                }

                if (cursorPos.X > ActualWidth)
                {
                    cursorPos.X = ActualWidth;
                }

                if (cursorPos.Y < 0)
                {
                    cursorPos.Y = 0;
                }

                if (cursorPos.Y > ActualHeight)
                {
                    cursorPos.Y = ActualHeight;
                }

                if (ColorChanged != null)
                {
                    ColorChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            base.OnMouseClickedLeft(x, y, down);
            depressed = down;

            if (down)
            {
                InputHandler.MouseFocus = this;
            }
            else
            {
                InputHandler.MouseFocus = null;
            }

            OnMouseMoved(x, y, dx: 0, dy: 0);
        }

        /// <summary>
        ///     Gets the color from specified coordinates.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <returns>Color value.</returns>
        private Color GetColorAt(Int32 x, Int32 y)
        {
            Single xPercent = x / (Single) ActualWidth;
            Single yPercent = 1 - (y / (Single) ActualHeight);

            Color result = Util.HSVToColor(hue, xPercent, yPercent);

            return result;
        }

        /// <summary>
        ///     Invalidates the control.
        /// </summary>
        private void InvalidateTexture()
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            if (texture == null)
            {
                var pixelData = new Byte[ActualWidth * ActualHeight * 4];

                for (var x = 0; x < ActualWidth; x++)
                {
                    for (var y = 0; y < ActualHeight; y++)
                    {
                        Color c = GetColorAt(x, y);
                        pixelData[4 * (x + (y * ActualWidth))] = c.R;
                        pixelData[(4 * (x + (y * ActualWidth))) + 1] = c.G;
                        pixelData[(4 * (x + (y * ActualWidth))) + 2] = c.B;
                        pixelData[(4 * (x + (y * ActualWidth))) + 3] = c.A;
                    }
                }

                texture = new Texture(currentSkin.Renderer);
                texture.Width = ActualWidth;
                texture.Height = ActualHeight;
                texture.LoadRaw(ActualWidth, ActualHeight, pixelData);
            }

            currentSkin.Renderer.DrawColor = Color.White;
            currentSkin.Renderer.DrawTexturedRect(texture, RenderBounds);

            currentSkin.Renderer.DrawColor = Color.Black;
            currentSkin.Renderer.DrawLinedRect(RenderBounds);

            Color selected = SelectedColor;

            if ((selected.R + selected.G + selected.B) / 3 < 170)
            {
                currentSkin.Renderer.DrawColor = Color.White;
            }
            else
            {
                currentSkin.Renderer.DrawColor = Color.Black;
            }

            Rectangle testRect = new(cursorPos.X - 3, cursorPos.Y - 3, width: 6, height: 6);

            currentSkin.Renderer.DrawShavedCornerRect(testRect);
        }

        protected override void OnBoundsChanged(Rectangle oldBounds)
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }

            base.OnBoundsChanged(oldBounds);
        }

        protected override Size Measure(Size availableSize)
        {
            cursorPos = new Point(x: 0, y: 0);

            return new Size(width: 128, height: 128);
        }

        protected override Size Arrange(Size finalSize)
        {
            return finalSize;
        }
    }
}
