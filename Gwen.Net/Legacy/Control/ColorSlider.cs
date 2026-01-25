using System;
using Gwen.Net.Legacy.Input;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     HSV hue selector.
    /// </summary>
    public class ColorSlider : ControlBase
    {
        private Boolean depressed;
        private Int32 selectedDist;
        private Texture texture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorSlider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorSlider(ControlBase parent)
            : base(parent)
        {
            Width = BaseUnit * 2;

            MouseInputEnabled = true;
            depressed = false;
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => GetColorAtHeight(selectedDist);
            set => SetColor(value);
        }

        protected override void AdaptToScaleChange()
        {
            Width = BaseUnit * 2;
        }

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
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            //Is there any way to move this into skin? Not for now, no idea how we'll "actually" render these

            if (texture == null)
            {
                var pixelData = new Byte[ActualWidth * ActualHeight * 4];

                for (var y = 0; y < ActualHeight; y++)
                {
                    Color c = GetColorAtHeight(y);

                    for (var x = 0; x < ActualWidth; x++)
                    {
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
            currentSkin.Renderer.DrawTexturedRect(texture, new Rectangle(x: 5, y: 0, ActualWidth - 10, ActualHeight));

            Int32 drawHeight = selectedDist - 3;

            //Draw our selectors
            currentSkin.Renderer.DrawColor = Color.Black;
            currentSkin.Renderer.DrawFilledRect(new Rectangle(x: 0, drawHeight + 2, ActualWidth, height: 1));
            currentSkin.Renderer.DrawFilledRect(new Rectangle(x: 0, drawHeight, width: 5, height: 5));
            currentSkin.Renderer.DrawFilledRect(new Rectangle(ActualWidth - 5, drawHeight, width: 5, height: 5));
            currentSkin.Renderer.DrawColor = Color.White;
            currentSkin.Renderer.DrawFilledRect(new Rectangle(x: 1, drawHeight + 1, width: 3, height: 3));
            currentSkin.Renderer.DrawFilledRect(new Rectangle(ActualWidth - 4, drawHeight + 1, width: 3, height: 3));

            base.Render(currentSkin);
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
                Point cursorPos = CanvasPosToLocal(new Point(x, y));

                if (cursorPos.Y < 0)
                {
                    cursorPos.Y = 0;
                }

                if (cursorPos.Y > ActualHeight)
                {
                    cursorPos.Y = ActualHeight;
                }

                selectedDist = cursorPos.Y;

                if (ColorChanged != null)
                {
                    ColorChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private Color GetColorAtHeight(Int32 y)
        {
            Single yPercent = y / (Single) ActualHeight;

            return Util.HSVToColor(yPercent * 360, s: 1, v: 1);
        }

        public void SetColor(Color color, Boolean doEvents = true)
        {
            var hsv = color.ToHSV();

            selectedDist = (Int32) (hsv.H / 360 * ActualHeight);

            if (doEvents && ColorChanged != null)
            {
                ColorChanged.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Size Measure(Size availableSize)
        {
            return new(width: 32, height: 10);
        }

        protected override Size Arrange(Size finalSize)
        {
            return new(MeasuredSize.Width, finalSize.Height);
        }
    }
}
