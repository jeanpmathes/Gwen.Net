using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gwen.Net.Renderer;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace Gwen.Net.OpenTk.Renderers
{
    public abstract class OpenTKRendererBase : RendererBase
    {
        protected static int lastTextureID;
        private readonly Graphics graphics;

        private readonly Dictionary<Tuple<string, Font>, TextRenderer> stringCache;
        private readonly StringFormat stringFormat;
        protected bool clipEnabled;
        protected Color color;

        protected int drawCallCount;
        protected bool textureEnabled;

        public OpenTKRendererBase()
        {
            GLVersion = (GL.GetInteger(GetPName.MajorVersion) * 10) + GL.GetInteger(GetPName.MinorVersion);

            stringCache = new Dictionary<Tuple<string, Font>, TextRenderer>();
            graphics = Graphics.FromImage(new Bitmap(width: 1024, height: 1024));
            stringFormat = new StringFormat(StringFormat.GenericTypographic);
            stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
        }

        public int TextCacheSize => stringCache.Count;

        public int DrawCallCount => drawCallCount;

        public abstract int VertexCount { get; }

        public int GLVersion { get; }

        public override Color DrawColor
        {
            get => color;
            set => color = value;
        }

        public override void Dispose()
        {
            FlushTextCache();
            base.Dispose();
        }

        protected override void OnScaleChanged(float oldScale)
        {
            FlushTextCache();
        }

        protected abstract void Flush();

        public void FlushTextCache()
        {
            // todo: some auto-expiring cache? based on number of elements or age
            foreach (TextRenderer textRenderer in stringCache.Values)
            {
                textRenderer.Dispose();
            }

            stringCache.Clear();
        }

        public override void DrawFilledRect(Rectangle rect)
        {
            if (textureEnabled)
            {
                Flush();
                textureEnabled = false;
            }

            rect = Translate(rect);

            DrawRect(rect);
        }

        public override void StartClip()
        {
            clipEnabled = true;
        }

        public override void EndClip()
        {
            clipEnabled = false;
        }

        public override void DrawTexturedRect(Texture t, Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1,
            float v2 = 1)
        {
            // Missing image, not loaded properly?
            if (null == t.RendererData)
            {
                DrawMissingImage(rect);

                return;
            }

            var tex = (int) t.RendererData;
            rect = Translate(rect);

            bool differentTexture = tex != lastTextureID;

            if (!textureEnabled || differentTexture)
            {
                Flush();
            }

            if (!textureEnabled)
            {
                textureEnabled = true;
            }

            if (differentTexture)
            {
                GL.BindTexture(TextureTarget.Texture2D, tex);
                lastTextureID = tex;
            }

            DrawRect(rect, u1, v1, u2, v2);
        }

        protected abstract void DrawRect(Rectangle rect, float u1 = 0, float v1 = 0, float u2 = 1, float v2 = 1);

        public override bool LoadFont(Font font)
        {
            font.RealSize = (float) Math.Ceiling(font.Size * Scale);
            var sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont != null)
            {
                sysFont.Dispose();
            }

            var fontStyle = System.Drawing.FontStyle.Regular;

            if (font.Bold)
            {
                fontStyle |= System.Drawing.FontStyle.Bold;
            }

            if (font.Italic)
            {
                fontStyle |= System.Drawing.FontStyle.Italic;
            }

            if (font.Underline)
            {
                fontStyle |= System.Drawing.FontStyle.Underline;
            }

            if (font.Strikeout)
            {
                fontStyle |= System.Drawing.FontStyle.Strikeout;
            }

            // apaprently this can't fail @_@
            // "If you attempt to use a font that is not supported, or the font is not installed on the machine that is running the application, the Microsoft Sans Serif font will be substituted."
            sysFont = new System.Drawing.Font(font.FaceName, font.RealSize, fontStyle);
            font.RendererData = sysFont;

            return true;
        }

        public override void FreeFont(Font font)
        {
            if (font.RendererData == null)
            {
                return;
            }

            var sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont == null)
            {
                throw new InvalidOperationException("Freeing empty font");
            }

            sysFont.Dispose();
            font.RendererData = null;
        }

        public override FontMetrics GetFontMetrics(Font font)
        {
            var sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont == null || Math.Abs(font.RealSize - (font.Size * Scale)) > 2)
            {
                FreeFont(font);
                LoadFont(font);
                sysFont = font.RendererData as System.Drawing.Font;
            }

            // from: http://csharphelper.com/blog/2014/08/get-font-metrics-in-c
            float emHeight = sysFont.FontFamily.GetEmHeight(sysFont.Style);
            float emHeightPixels = ConvertToPixels(sysFont.Size, sysFont.Unit);
            float designToPixels = emHeightPixels / emHeight;

            float ascentPixels = designToPixels * sysFont.FontFamily.GetCellAscent(sysFont.Style);
            float descentPixels = designToPixels * sysFont.FontFamily.GetCellDescent(sysFont.Style);
            float cellHeightPixels = ascentPixels + descentPixels;
            float internalLeadingPixels = cellHeightPixels - emHeightPixels;
            float lineSpacingPixels = designToPixels * sysFont.FontFamily.GetLineSpacing(sysFont.Style);
            float externalLeadingPixels = lineSpacingPixels - cellHeightPixels;

            FontMetrics fm = new(
                emHeightPixels,
                ascentPixels,
                descentPixels,
                cellHeightPixels,
                internalLeadingPixels,
                lineSpacingPixels,
                externalLeadingPixels
            );

            return fm;
        }

        private float ConvertToPixels(float value, GraphicsUnit unit)
        {
            switch (unit)
            {
                case GraphicsUnit.Document:
                    value *= graphics.DpiX / 300;

                    break;
                case GraphicsUnit.Inch:
                    value *= graphics.DpiX;

                    break;
                case GraphicsUnit.Millimeter:
                    value *= graphics.DpiX / 25.4F;

                    break;
                case GraphicsUnit.Pixel: break;
                case GraphicsUnit.Point:
                    value *= graphics.DpiX / 72;

                    break;
                default: throw new Exception("Unknown unit " + unit);
            }

            return value;
        }

        public override Size MeasureText(Font font, string text)
        {
            var sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont == null || Math.Abs(font.RealSize - (font.Size * Scale)) > 2)
            {
                FreeFont(font);
                LoadFont(font);
                sysFont = font.RendererData as System.Drawing.Font;
            }

            Tuple<string, Font> key = new(text, font);

            if (stringCache.ContainsKey(key))
            {
                Texture tex = stringCache[key].Texture;

                return new Size(tex.Width, tex.Height);
            }

            SizeF TabSize = graphics.MeasureString(
                "....",
                sysFont); //Spaces are not being picked up, let's just use .'s.

            stringFormat.SetTabStops(
                firstTabOffset: 0f,
                new[]
                {
                    TabSize.Width
                });

            SizeF size = graphics.MeasureString(text, sysFont, System.Drawing.Point.Empty, stringFormat);

            return new Size(Util.Ceil(size.Width), Util.Ceil(size.Height));
        }

        public override void RenderText(Font font, Point position, string text)
        {
            Flush();

            var sysFont = font.RendererData as System.Drawing.Font;

            if (sysFont == null || Math.Abs(font.RealSize - (font.Size * Scale)) > 2)
            {
                FreeFont(font);
                LoadFont(font);
                sysFont = font.RendererData as System.Drawing.Font;
            }

            Tuple<string, Font> key = new(text, font);

            if (!stringCache.ContainsKey(key))
            {
                // not cached - create text renderer
                Size size = MeasureText(font, text);
                TextRenderer tr = new(size.Width, size.Height, this);
                tr.DrawString(text, sysFont, Brushes.White, Point.Zero, stringFormat); // renders string on the texture

                DrawTexturedRect(
                    tr.Texture,
                    new Rectangle(position.X, position.Y, tr.Texture.Width, tr.Texture.Height));

                stringCache[key] = tr;
            }
            else
            {
                TextRenderer tr = stringCache[key];

                DrawTexturedRect(
                    tr.Texture,
                    new Rectangle(position.X, position.Y, tr.Texture.Width, tr.Texture.Height));
            }
        }

        internal static void LoadTextureInternal(Texture t, Bitmap bmp)
        {

            PixelFormat lock_format;

            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    lock_format = PixelFormat.Format32bppArgb;

                    break;
                case PixelFormat.Format24bppRgb:
                    lock_format = PixelFormat.Format32bppArgb;

                    break;
                default:
                    t.Failed = true;

                    return;
            }


            int glTex;

            // Create the opengl texture
            GL.GenTextures(n: 1, out glTex);

            GL.BindTexture(TextureTarget.Texture2D, glTex);
            lastTextureID = glTex;

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);

            // Sort out our GWEN texture
            t.RendererData = glTex;
            t.Width = bmp.Width;
            t.Height = bmp.Height;

            BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(x: 0, y: 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                lock_format);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                level: 0,
                PixelInternalFormat.Rgba,
                t.Width,
                t.Height,
                border: 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0);


            bmp.UnlockBits(data);
            bmp.Dispose();
        }

        public override void LoadTexture(Texture t)
        {
            Bitmap bmp;

            try
            {
                bmp = ImageLoader.Load(t.Name);
            }
            catch (Exception)
            {
                t.Failed = true;

                return;
            }

            LoadTextureInternal(t, bmp);
            bmp.Dispose();
        }

        public override void LoadTextureStream(Texture t, Stream data)
        {
            Bitmap bmp;

            try
            {
                bmp = ImageLoader.Load(t.Name);
            }
            catch (Exception)
            {
                t.Failed = true;

                return;
            }

            LoadTextureInternal(t, bmp);
            bmp.Dispose();
        }

        public override void LoadTextureRaw(Texture t, byte[] pixelData)
        {
            Bitmap bmp;

            try
            {
                unsafe
                {
                    fixed (byte* ptr = &pixelData[0])
                    {
                        bmp = new Bitmap(t.Width, t.Height, 4 * t.Width, PixelFormat.Format32bppArgb, (IntPtr) ptr);
                    }
                }
            }
            catch (Exception)
            {
                t.Failed = true;

                return;
            }

            int glTex;

            // Create the opengl texture
            GL.GenTextures(n: 1, out glTex);

            GL.BindTexture(TextureTarget.Texture2D, glTex);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int) TextureMagFilter.Linear);

            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear);

            // Sort out our GWEN texture
            t.RendererData = glTex;

            BitmapData data = bmp.LockBits(
                new System.Drawing.Rectangle(x: 0, y: 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                level: 0,
                PixelInternalFormat.Rgba,
                t.Width,
                t.Height,
                border: 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                PixelType.UnsignedByte,
                data.Scan0);

            bmp.UnlockBits(data);
            bmp.Dispose();

            //[halfofastaple] Must rebind previous texture, to ensure creating a texture doesn't mess with the render flow.
            // Setting m_LastTextureID isn't working, for some reason (even if you always rebind the texture,
            // even if the previous one was the same), we are probably making draw calls where we shouldn't be?
            // Eventually the bug needs to be fixed (color picker in a window causes graphical errors), but for now,
            // this is fine.
            GL.BindTexture(TextureTarget.Texture2D, lastTextureID);

        }

        public override void FreeTexture(Texture t)
        {
            if (t.RendererData == null)
            {
                return;
            }

            var tex = (int) t.RendererData;

            if (tex == 0)
            {
                return;
            }

            GL.DeleteTextures(n: 1, ref tex);
            t.RendererData = null;
        }

        public override unsafe Color PixelColor(Texture texture, uint x, uint y, Color defaultColor)
        {
            if (texture.RendererData == null)
            {
                return defaultColor;
            }

            var tex = (int) texture.RendererData;

            if (tex == 0)
            {
                return defaultColor;
            }

            Color pixel;

            GL.BindTexture(TextureTarget.Texture2D, tex);
            lastTextureID = tex;

            long offset = 4 * (x + (y * texture.Width));
            var data = new byte[4 * texture.Width * texture.Height];

            fixed (byte* ptr = &data[0])
            {
                GL.GetTexImage(
                    TextureTarget.Texture2D,
                    level: 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Rgba,
                    PixelType.UnsignedByte,
                    (IntPtr) ptr);

                pixel = new Color(data[offset + 3], data[offset + 0], data[offset + 1], data[offset + 2]);
            }

            return pixel;
        }

        public abstract void Resize(int width, int height);
    }
}
