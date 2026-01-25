using System;
using Gwen.Net.Legacy.Renderer;

namespace Gwen.Net.Legacy.Skin.Texturing
{
    /// <summary>
    ///     Single textured element.
    /// </summary>
    public readonly struct Single
    {
        private readonly Texture texture;
        private readonly System.Single[] uv;
        private readonly Int32 width;
        private readonly Int32 height;

        public Single(Texture texture, System.Single x, System.Single y, System.Single w, System.Single h)
        {
            this.texture = texture;

            System.Single textureWidth = this.texture.Width;
            System.Single textureHeight = this.texture.Height;

            uv = new System.Single[4];
            uv[0] = x / textureWidth;
            uv[1] = y / textureHeight;
            uv[2] = (x + w) / textureWidth;
            uv[3] = (y + h) / textureHeight;

            width = (Int32)w;
            height = (Int32)h;
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
            r.X += (Int32)((r.Width - width) * 0.5);
            r.Y += (Int32)((r.Height - height) * 0.5);
            r.Width = width;
            r.Height = height;

            Draw(render, r, col);
        }
    }
}
