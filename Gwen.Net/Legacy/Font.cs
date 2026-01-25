using System;
using System.Diagnostics;
using Gwen.Net.Legacy.Renderer;

namespace Gwen.Net.Legacy
{
    /// <summary>
    ///     Font style.
    /// </summary>
    [Flags]
    public enum FontStyle
    {
        Normal = 0,
        Bold = 1 << 0,
        Italic = 1 << 1,
        Underline = 1 << 2,
        Strikeout = 1 << 3
    }

    /// <summary>
    ///     Represents font resource.
    /// </summary>
    public class Font : IDisposable
    {
        private readonly RendererBase renderer;
        private FontMetrics? fontMetrics;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Font" /> class.
        /// </summary>
        public Font(RendererBase renderer)
            : this(renderer, "Arial") {}

        /// <summary>
        ///     Initializes a new instance of the <see cref="Font" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        /// <param name="faceName">Face name.</param>
        /// <param name="size">Font size.</param>
        public Font(RendererBase renderer, String faceName, Int32 size = 10)
        {
            this.renderer = renderer;
            fontMetrics = null;
            FaceName = faceName;
            Size = size;
            Smooth = false;
            Bold = false;
            Italic = false;
            Underline = false;
            Strikeout = false;
            //DropShadow = false;
        }

        /// <summary>
        ///     Font face name. Exact meaning depends on renderer.
        /// </summary>
        public String FaceName { get; set; }

        /// <summary>
        ///     Font size.
        /// </summary>
        public Int32 Size { get; set; }

        /// <summary>
        ///     Enables or disables font smoothing (default: disabled).
        /// </summary>
        public Boolean Smooth { get; set; }

        public Boolean Bold { get; set; }
        public Boolean Italic { get; set; }
        public Boolean Underline { get; set; }
        public Boolean Strikeout { get; set; }

        //public bool DropShadow { get; set; }

        /// <summary>
        ///     This should be set by the renderer if it tries to use a font where it's null.
        /// </summary>
        public Object? RendererData { get; set; }

        /// <summary>
        ///     This is the real font size, after it's been scaled by Renderer.Scale()
        /// </summary>
        public Single RealSize { get; set; }

        /// <summary>
        ///     Gets the font metrics.
        /// </summary>
        public FontMetrics FontMetrics
        {
            get
            {
                if (fontMetrics == null)
                {
                    fontMetrics = renderer.GetFontMetrics(this);
                }

                return (FontMetrics)fontMetrics;
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            renderer.FreeFont(this);
            GC.SuppressFinalize(this);
        }
        
        ~Font()
        {
            
            Debug.Fail($"IDisposable object finalized: {GetType()}");
        }

        /// <summary>
        ///     Duplicates font data (except renderer data which must be reinitialized).
        /// </summary>
        /// <returns></returns>
        public Font Copy()
        {
            Font f = new(renderer, FaceName, Size);
            f.RealSize = RealSize;
            f.RendererData = null; // must be reinitialized
            f.Bold = Bold;
            f.Italic = Italic;
            f.Underline = Underline;
            f.Strikeout = Strikeout;
            //f.DropShadow = DropShadow;

            return f;
        }

        /// <summary>
        ///     Create a new font instance. This function uses a font cache to load the font.
        ///     This is preferable method to create a font. User don't need to worry about
        ///     disposing the font.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        /// <param name="faceName">Face name.</param>
        /// <param name="size">Font size.</param>
        /// <param name="style">Font style.</param>
        /// <returns>Font.</returns>
        public static Font Create(RendererBase renderer, String faceName, Int32 size = 10, FontStyle style = 0)
        {
            return FontCache.GetFont(renderer, faceName, size, style);
        }
    }
}
