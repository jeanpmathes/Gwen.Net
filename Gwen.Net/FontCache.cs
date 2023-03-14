using System;
using System.Collections.Generic;
using Gwen.Net.Renderer;

namespace Gwen.Net
{
    public class FontCache : IDisposable
    {
        private static FontCache instance;

        private readonly Dictionary<string, Font> fontCache = new();

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
            instance = null;
        }

        public static Font GetFont(RendererBase renderer, string faceName, int size = 10, FontStyle style = 0)
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

        private Font InternalGetFont(RendererBase renderer, string faceName, int size, FontStyle style)
        {
            string id = $"{faceName};{size};{(int) style}";
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (KeyValuePair<string, Font> font in fontCache)
                {
                    font.Value.Dispose();
                }

                fontCache.Clear();
            }
        }
    }
}
