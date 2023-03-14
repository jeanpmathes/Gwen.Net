using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Image container.
    /// </summary>
    public class ImagePanel : ControlBase
    {
        private readonly Texture texture;
        private readonly float[] uv;
        private Size imageSize;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImagePanel" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ImagePanel(ControlBase parent)
            : base(parent)
        {
            uv = new float[4];
            texture = new Texture(Skin.Renderer);
            imageSize = Size.Zero;
            SetUV(u1: 0, v1: 0, u2: 1, v2: 1);
            MouseInputEnabled = true;
            ImageColor = Color.White;
        }

        /// <summary>
        ///     Texture name.
        /// </summary>
        public string ImageName
        {
            get => texture.Name;
            set => texture.Load(value, _ => {});
        }

        /// <summary>
        ///     Gets or sets the size of the image.
        /// </summary>
        public Size ImageSize
        {
            get => imageSize;
            set
            {
                if (value == imageSize)
                {
                    return;
                }

                imageSize = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the texture coordinates of the image in pixels.
        /// </summary>
        public Rectangle TextureRect
        {
            get
            {
                if (texture == null)
                {
                    return Rectangle.Empty;
                }

                var x1 = (int) (uv[0] * texture.Width);
                var y1 = (int) (uv[1] * texture.Height);
                int x2 = Util.Ceil(uv[2] * texture.Width);
                int y2 = Util.Ceil(uv[3] * texture.Height);

                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            set
            {
                if (texture == null)
                {
                    return;
                }

                uv[0] = value.X / (float) texture.Width;
                uv[1] = value.Y / (float) texture.Height;
                uv[2] = uv[0] + (value.Width / (float) texture.Width);
                uv[3] = uv[1] + (value.Height / (float) texture.Height);
            }
        }

        /// <summary>
        ///     Gets or sets the color of the image.
        /// </summary>
        public Color ImageColor { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            texture.Dispose();
            base.Dispose();
        }

        /// <summary>
        ///     Sets the texture coordinates of the image in uv-coordinates.
        /// </summary>
        public virtual void SetUV(float u1, float v1, float u2, float v2)
        {
            uv[0] = u1;
            uv[1] = v1;
            uv[2] = u2;
            uv[3] = v2;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            base.Render(currentSkin);
            currentSkin.Renderer.DrawColor = ImageColor;
            currentSkin.Renderer.DrawTexturedRect(texture, RenderBounds, uv[0], uv[1], uv[2], uv[3]);
        }

        protected override Size Measure(Size availableSize)
        {
            if (texture == null)
            {
                return Size.Zero;
            }

            float scale = Scale;

            Size size = imageSize;

            if (size.Width == 0)
            {
                size.Width = texture.Width;
            }

            if (size.Height == 0)
            {
                size.Height = texture.Height;
            }

            return new Size(Util.Ceil(size.Width * scale), Util.Ceil(size.Height * scale));
        }

        protected override Size Arrange(Size finalSize)
        {
            return finalSize;
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>
        ///     True if handled.
        /// </returns>
        protected override bool OnKeySpace(bool down)
        {
            if (down)
            {
                base.OnMouseClickedLeft(x: 0, y: 0, down: true);
            }

            return true;
        }
    }
}
