using System;
using System.Collections.Generic;
using Gwen.Net.Legacy.Renderer;

namespace Gwen.Net.Legacy
{
    public class FontCache : IDisposable
    {
        private static FontCache instance;

        private readonly Dictionary<String, Font> fontCache = new();

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
            instance = null;
        }

        public static Font GetFont(RendererBase renderer, String faceName, Int32 size = 10, FontStyle style = 0)
        {
            if (instance == null)
            {
                instance = new FontCache();
            }

            return instance.InternalGetFont(renderer, faceName, size, style);
        }

        public static void FreeCache()
        {
            if (instance != null)
            {
                instance.Dispose();
            }
        }

        private Font InternalGetFont(RendererBase renderer, String faceName, Int32 size, FontStyle style)
        {
            String id = $"{faceName};{size};{(Int32) style}";
            Font font;

            if (!fontCache.TryGetValue(id, out font))
            {
                font = new Font(renderer, faceName, size);

                if ((style & FontStyle.Bold) != 0)
                {
                    font.Bold = true;
                }

                if ((style & FontStyle.Italic) != 0)
                {
                    font.Italic = true;
                }

                if ((style & FontStyle.Underline) != 0)
                {
                    font.Underline = true;
                }

                if ((style & FontStyle.Strikeout) != 0)
                {
                    font.Strikeout = true;
                }

                fontCache[id] = font;
            }

            return font;
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                foreach (KeyValuePair<String, Font> font in fontCache)
                {
                    font.Value.Dispose();
                }

                fontCache.Clear();
            }
        }
    }
}
