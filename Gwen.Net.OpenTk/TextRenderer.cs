using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Gwen.Net.OpenTk.Renderers;
using Gwen.Net.Renderer;

namespace Gwen.Net.OpenTk
{
    public sealed class TextRenderer : IDisposable
    {
        private readonly Bitmap bitmap;
        private readonly Graphics graphics;
        private Boolean disposed;

        public TextRenderer(Int32 width, Int32 height, RendererBase renderer)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException("width");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException("height");
            }

            bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(bitmap);
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.Clear(System.Drawing.Color.Transparent);
            Texture = new Texture(renderer) {Width = width, Height = height};
        }

        public Texture Texture { get; }

        public void Dispose()
        {
            Dispose(manual: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Draws the specified string to the backing store.
        /// </summary>
        /// <param name="text">The <see cref="System.String" /> to draw.</param>
        /// <param name="font">The <see cref="System.Drawing.Font" /> that will be used.</param>
        /// <param name="brush">The <see cref="System.Drawing.Brush" /> that will be used.</param>
        /// <param name="point">
        ///     The location of the text on the backing store, in 2d pixel coordinates.
        ///     The origin (0, 0) lies at the top-left corner of the backing store.
        /// </param>
        /// <param name="format">The <see cref="StringFormat" /> that will be used.</param>
        public void DrawString(String text, System.Drawing.Font font, Brush brush, Point point, StringFormat format)
        {
            graphics.DrawString(
                text,
                font,
                brush,
                new System.Drawing.Point(point.X, point.Y),
                format); // render text on the bitmap

            OpenTkRendererBase.LoadTextureInternal(Texture, bitmap); // copy bitmap to gl texture
        }

        private void Dispose(Boolean manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    bitmap.Dispose();
                    graphics.Dispose();
                    Texture.Dispose();
                }

                disposed = true;
            }
        }
        
        ~TextRenderer()
        {
            Debug.Fail("TextRenderer was not disposed!");
        }
    }
}
