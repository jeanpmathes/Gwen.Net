using Gwen.Net.Renderer;

namespace Gwen.Net.Skin.Texturing
{
    /// <summary>
    ///     Single textured element.
    /// </summary>
    public readonly struct Single
    {
        private readonly Texture texture;
        private readonly float[] uv;
        private readonly int width;
        private readonly int height;

        public Single(Texture texture, float x, float y, float w, float h)
        {
            this.texture = texture;

            float textureWidth = this.texture.Width;
            float textureHeight = this.texture.Height;

            uv = new float[4];
            uv[0] = x / textureWidth;
            uv[1] = y / textureHeight;
            uv[2] = (x + w) / textureWidth;
            uv[3] = (y + h) / textureHeight;

            width = (int)w;
            height = (int)h;
        }

        // can't have this as default param
        public void Draw(RendererBase render, Rectangle r)
        {
            Draw(render, r, Color.White);
        }

        public void Draw(RendererBase render, Rectangle r, Color col)
        {
            if (texture == null)
            {
                return;
            }

            render.DrawColor = col;
            render.DrawTexturedRect(texture, r, uv[0], uv[1], uv[2], uv[3]);
        }

        public void DrawCenter(RendererBase render, Rectangle r)
        {
            if (texture == null)
            {
                return;
            }

            DrawCenter(render, r, Color.White);
        }

        public void DrawCenter(RendererBase render, Rectangle r, Color col)
        {
            r.X += (int)((r.Width - width) * 0.5);
            r.Y += (int)((r.Height - height) * 0.5);
            r.Width = width;
            r.Height = height;

            Draw(render, r, col);
        }
    }
}
