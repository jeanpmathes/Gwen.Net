using System;
using System.Diagnostics;

namespace Gwen.Net.Renderer
{
    /// <summary>
    ///     Base renderer.
    /// </summary>
    public class RendererBase : IDisposable
    {
        private Rectangle clipRegion;
        
        private Point renderOffset;
        private Single scale;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RendererBase" /> class.
        /// </summary>
        protected RendererBase()
        {
            renderOffset = Point.Zero;
            scale = 1.0f;
        }

        public Single Scale
        {
            get => scale;
            set
            {
                Single oldScale = scale;
                scale = value;
                OnScaleChanged(oldScale);
            }
        }

        /// <summary>
        ///     Gets or sets the current drawing color.
        /// </summary>
        public virtual Color DrawColor { get; set; }

        /// <summary>
        ///     Rendering offset. No need to touch it usually.
        /// </summary>
        public Point RenderOffset
        {
            get => renderOffset;
            set => renderOffset = value;
        }

        /// <summary>
        ///     Clipping rectangle.
        /// </summary>
        public Rectangle ClipRegion
        {
            get => clipRegion;
            set => clipRegion = value;
        }

        /// <summary>
        ///     Indicates whether the clip region is visible.
        /// </summary>
        public Boolean ClipRegionVisible
        {
            get
            {
                if (clipRegion.Width <= 0 || clipRegion.Height <= 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected virtual void OnScaleChanged(Single oldScale) {}
        
        ~RendererBase()
        {
            Debug.Fail("RendererBase was not disposed!");
        }

        /// <summary>
        ///     Starts rendering.
        /// </summary>
        public virtual void Begin() {}

        /// <summary>
        ///     Stops rendering.
        /// </summary>
        public virtual void End() {}

        /// <summary>
        ///     Draws a line.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public virtual void DrawLine(Int32 x, Int32 y, Int32 a, Int32 b) {}

        /// <summary>
        ///     Draws a solid filled rectangle.
        /// </summary>
        /// <param name="rect"></param>
        public virtual void DrawFilledRect(Rectangle rect) {}

        /// <summary>
        ///     Starts clipping to the current clipping rectangle.
        /// </summary>
        public virtual void StartClip() {}

        /// <summary>
        ///     Stops clipping.
        /// </summary>
        public virtual void EndClip() {}

        /// <summary>
        ///     Loads the specified texture.
        /// </summary>
        public virtual void LoadTexture(Texture texture, Action<Exception> errorCallback) {}

        /// <summary>
        ///     Initializes texture from raw pixel data.
        /// </summary>
        /// <param name="texture">Texture to initialize. Dimensions need to be set.</param>
        /// <param name="pixelData">Pixel data in RGBA format.</param>
        public virtual void LoadTextureRaw(Texture texture, Byte[] pixelData) {}

        /// <summary>
        ///     Frees the specified texture.
        /// </summary>
        /// <param name="texture">Texture to free.</param>
        public virtual void FreeTexture(Texture texture) {}

        /// <summary>
        ///     Draws textured rectangle.
        /// </summary>
        /// <param name="texture">Texture to use.</param>
        /// <param name="targetRect">Rectangle bounds.</param>
        /// <param name="u1">Texture coordinate u1.</param>
        /// <param name="v1">Texture coordinate v1.</param>
        /// <param name="u2">Texture coordinate u2.</param>
        /// <param name="v2">Texture coordinate v2.</param>
        public virtual void DrawTexturedRect(Texture texture, Rectangle targetRect, Single u1 = 0, Single v1 = 0, Single u2 = 1,
            Single v2 = 1) {}

        /// <summary>
        ///     Draws "missing image" default texture.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        public virtual void DrawMissingImage(Rectangle rect)
        {
            DrawColor = Color.Red;
            DrawFilledRect(rect);
        }

        /// <summary>
        ///     Loads the specified font.
        /// </summary>
        /// <param name="font">Font to load.</param>
        /// <returns>True if succeeded.</returns>
        public virtual Boolean LoadFont(Font font)
        {
            return false;
        }

        /// <summary>
        ///     Frees the specified font.
        /// </summary>
        /// <param name="font">Font to free.</param>
        public virtual void FreeFont(Font font) {}

        /// <summary>
        ///     Gets the font metrics.
        /// </summary>
        /// <param name="font">Font.</param>
        /// <returns>The font metrics.</returns>
        public virtual FontMetrics GetFontMetrics(Font font)
        {
            return new(font);
        }

        /// <summary>
        ///     Returns dimensions of the text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="text">Text to measure.</param>
        /// <returns>Width and height of the rendered text.</returns>
        public virtual Size MeasureText(Font font, String text)
        {
            Size p = new((Int32) (font.Size * Scale * text.Length * 0.4f), (Int32) (font.Size * Scale));

            return p;
        }

        /// <summary>
        ///     Renders text using specified font.
        /// </summary>
        /// <param name="font">Font to use.</param>
        /// <param name="position">Top-left corner of the text.</param>
        /// <param name="text">Text to render.</param>
        public virtual void RenderText(Font font, Point position, String text)
        {
            Single size = font.Size * Scale;

            for (var i = 0; i < text.Length; i++)
            {
                Char chr = text[i];

                if (chr == ' ')
                {
                    continue;
                }

                Rectangle r = Util.FloatRect(position.X + (i * size * 0.4f), position.Y, (size * 0.4f) - 1, size);

                /*
                    This isn't important, it's just me messing around changing the
                    shape of the rect based on the letter.. just for fun.
                */
                if (chr == 'l' || chr == 'i' || chr == '!' || chr == 't')
                {
                    r.Width = 1;
                }
                else if (chr >= 'a' && chr <= 'z')
                {
                    r.Y = (Int32) (r.Y + (size * 0.5f));
                    r.Height = (Int32) (r.Height - (size * 0.4f));
                }
                else if (chr == '.' || chr == ',')
                {
                    r.X += 2;
                    r.Y += r.Height - 2;
                    r.Width = 2;
                    r.Height = 2;
                }
                else if (chr == '\'' || chr == '`' || chr == '"')
                {
                    r.X += 3;
                    r.Width = 2;
                    r.Height = 2;
                }

                if (chr == 'o' || chr == 'O' || chr == '0')
                {
                    DrawLinedRect(r);
                }
                else
                {
                    DrawFilledRect(r);
                }
            }
        }

        //
        // No need to implement these functions in your derived class, but if 
        // you can do them faster than the default implementation it's a good idea to.
        //

        /// <summary>
        ///     Draws a lined rectangle. Used for keyboard focus overlay.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        public virtual void DrawLinedRect(Rectangle rect)
        {
            DrawFilledRect(new Rectangle(rect.X, rect.Y, rect.Width, height: 1));
            DrawFilledRect(new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, height: 1));

            DrawFilledRect(new Rectangle(rect.X, rect.Y, width: 1, rect.Height));
            DrawFilledRect(new Rectangle(rect.X + rect.Width - 1, rect.Y, width: 1, rect.Height));
        }

        /// <summary>
        ///     Draws a single pixel. Very slow, do not use. :P
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        public virtual void DrawPixel(Int32 x, Int32 y)
        {
            // [omeg] amazing ;)
            DrawFilledRect(new Rectangle(x, y, width: 1, height: 1));
        }

        /// <summary>
        ///     Gets pixel color of a specified texture. Slow.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <returns>Pixel color.</returns>
        public virtual Color PixelColor(Texture texture, UInt32 x, UInt32 y)
        {
            return PixelColor(texture, x, y, Color.White);
        }

        /// <summary>
        ///     Gets pixel color of a specified texture, returning default if otherwise failed. Slow.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        /// <param name="defaultColor">Color to return on failure.</param>
        /// <returns>Pixel color.</returns>
        public virtual Color PixelColor(Texture texture, UInt32 x, UInt32 y, Color defaultColor)
        {
            return defaultColor;
        }

        /// <summary>
        ///     Draws a round-corner rectangle.
        /// </summary>
        /// <param name="rect">Target rectangle.</param>
        /// <param name="slight"></param>
        public virtual void DrawShavedCornerRect(Rectangle rect, Boolean slight = false)
        {
            // Draw INSIDE the w/h.
            rect.Width -= 1;
            rect.Height -= 1;

            if (slight)
            {
                DrawFilledRect(new Rectangle(rect.X + 1, rect.Y, rect.Width - 1, height: 1));
                DrawFilledRect(new Rectangle(rect.X + 1, rect.Y + rect.Height, rect.Width - 1, height: 1));

                DrawFilledRect(new Rectangle(rect.X, rect.Y + 1, width: 1, rect.Height - 1));
                DrawFilledRect(new Rectangle(rect.X + rect.Width, rect.Y + 1, width: 1, rect.Height - 1));

                return;
            }

            DrawPixel(rect.X + 1, rect.Y + 1);
            DrawPixel(rect.X + rect.Width - 1, rect.Y + 1);

            DrawPixel(rect.X + 1, rect.Y + rect.Height - 1);
            DrawPixel(rect.X + rect.Width - 1, rect.Y + rect.Height - 1);

            DrawFilledRect(new Rectangle(rect.X + 2, rect.Y, rect.Width - 3, height: 1));
            DrawFilledRect(new Rectangle(rect.X + 2, rect.Y + rect.Height, rect.Width - 3, height: 1));

            DrawFilledRect(new Rectangle(rect.X, rect.Y + 2, width: 1, rect.Height - 3));
            DrawFilledRect(new Rectangle(rect.X + rect.Width, rect.Y + 2, width: 1, rect.Height - 3));
        }

        private Int32 TranslateX(Int32 x)
        {
            Int32 x1 = x + renderOffset.X;

            return x1;
        }

        private Int32 TranslateY(Int32 y)
        {
            Int32 y1 = y + renderOffset.Y;

            return y1;
        }

        /// <summary>
        ///     Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Translate(ref Int32 x, ref Int32 y)
        {
            x += renderOffset.X;
            y += renderOffset.Y;
        }

        /// <summary>
        ///     Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        public Point Translate(Point point)
        {
            Int32 x = point.X;
            Int32 y = point.Y;
            Translate(ref x, ref y);

            return new Point(x, y);
        }

        /// <summary>
        ///     Translates a panel's local drawing coordinate into view space, taking offsets into account.
        /// </summary>
        public Rectangle Translate(Rectangle rect)
        {
            return new(TranslateX(rect.X), TranslateY(rect.Y), rect.Width, rect.Height);
        }

        /// <summary>
        ///     Adds a point to the render offset.
        /// </summary>
        /// <param name="offset">Point to add.</param>
        public void AddRenderOffset(Rectangle offset)
        {
            renderOffset = new Point(renderOffset.X + offset.X, renderOffset.Y + offset.Y);
        }

        /// <summary>
        ///     Adds a rectangle to the clipping region.
        /// </summary>
        /// <param name="rect">Rectangle to add.</param>
        public void AddClipRegion(Rectangle rect)
        {
            rect.X = renderOffset.X;
            rect.Y = renderOffset.Y;

            Rectangle r = rect;

            if (rect.X < clipRegion.X)
            {
                r.Width -= clipRegion.X - r.X;
                r.X = clipRegion.X;
            }

            if (rect.Y < clipRegion.Y)
            {
                r.Height -= clipRegion.Y - r.Y;
                r.Y = clipRegion.Y;
            }

            if (rect.Right > clipRegion.Right)
            {
                r.Width = clipRegion.Right - r.X + 1;
            }

            if (rect.Bottom > clipRegion.Bottom)
            {
                r.Height = clipRegion.Bottom - r.Y + 1;
            }

            clipRegion = r;
        }

        /// <summary>
        ///     Sets a rectangle to the clipping region.
        /// </summary>
        /// <param name="rect">Rectangle to set.</param>
        public void SetClipRegion(Rectangle rect)
        {
            rect.X += renderOffset.X;
            rect.Y += renderOffset.Y;

            Rectangle r = rect;

            if (rect.X < clipRegion.X)
            {
                r.Width -= clipRegion.X - r.X;
                r.X = clipRegion.X;
            }

            if (rect.Y < clipRegion.Y)
            {
                r.Height -= clipRegion.Y - r.Y;
                r.Y = clipRegion.Y;
            }

            if (rect.Right > clipRegion.Right)
            {
                r.Width = clipRegion.Right - r.X + 1;
            }

            if (rect.Bottom > clipRegion.Bottom)
            {
                r.Height = clipRegion.Bottom - r.Y + 1;
            }

            clipRegion = r;
        }
    }
}
