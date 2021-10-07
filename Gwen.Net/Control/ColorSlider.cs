using System;
using Gwen.Net.Input;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     HSV hue selector.
    /// </summary>
    public class ColorSlider : ControlBase
    {
        private bool m_Depressed;
        private int m_SelectedDist;
        private Texture m_Texture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ColorSlider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ColorSlider(ControlBase parent)
            : base(parent)
        {
            Width = BaseUnit * 2;

            MouseInputEnabled = true;
            m_Depressed = false;
        }

        /// <summary>
        ///     Selected color.
        /// </summary>
        public Color SelectedColor
        {
            get => GetColorAtHeight(m_SelectedDist);
            set => SetColor(value);
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
            if (m_Texture != null)
            {
                m_Texture.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            //Is there any way to move this into skin? Not for now, no idea how we'll "actually" render these

            if (m_Texture == null)
            {
                byte[] pixelData = new byte[ActualWidth * ActualHeight * 4];

                for (int y = 0; y < ActualHeight; y++)
                {
                    Color c = GetColorAtHeight(y);

                    for (int x = 0; x < ActualWidth; x++)
                    {
                        pixelData[4 * (x + (y * ActualWidth))] = c.R;
                        pixelData[(4 * (x + (y * ActualWidth))) + 1] = c.G;
                        pixelData[(4 * (x + (y * ActualWidth))) + 2] = c.B;
                        pixelData[(4 * (x + (y * ActualWidth))) + 3] = c.A;
                    }
                }

                m_Texture = new Texture(skin.Renderer);
                m_Texture.Width = ActualWidth;
                m_Texture.Height = ActualHeight;
                m_Texture.LoadRaw(ActualWidth, ActualHeight, pixelData);
            }

            skin.Renderer.DrawColor = Color.White;
            skin.Renderer.DrawTexturedRect(m_Texture, new Rectangle(x: 5, y: 0, ActualWidth - 10, ActualHeight));

            int drawHeight = m_SelectedDist - 3;

            //Draw our selectors
            skin.Renderer.DrawColor = Color.Black;
            skin.Renderer.DrawFilledRect(new Rectangle(x: 0, drawHeight + 2, ActualWidth, height: 1));
            skin.Renderer.DrawFilledRect(new Rectangle(x: 0, drawHeight, width: 5, height: 5));
            skin.Renderer.DrawFilledRect(new Rectangle(ActualWidth - 5, drawHeight, width: 5, height: 5));
            skin.Renderer.DrawColor = Color.White;
            skin.Renderer.DrawFilledRect(new Rectangle(x: 1, drawHeight + 1, width: 3, height: 3));
            skin.Renderer.DrawFilledRect(new Rectangle(ActualWidth - 4, drawHeight + 1, width: 3, height: 3));

            base.Render(skin);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down)
        {
            base.OnMouseClickedLeft(x, y, down);
            m_Depressed = down;

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
        protected override void OnMouseMoved(int x, int y, int dx, int dy)
        {
            if (m_Depressed)
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

                m_SelectedDist = cursorPos.Y;

                if (ColorChanged != null)
                {
                    ColorChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private Color GetColorAtHeight(int y)
        {
            float yPercent = y / (float)ActualHeight;

            return Util.HSVToColor(yPercent * 360, s: 1, v: 1);
        }

        public void SetColor(Color color, bool doEvents = true)
        {
            HSV hsv = color.ToHSV();

            m_SelectedDist = (int)(hsv.H / 360 * ActualHeight);

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