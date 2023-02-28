using System;
using System.IO;
using Gwen.Net.Renderer;

namespace Gwen.Net
{
    /// <summary>
    ///     Represents a texture.
    /// </summary>
    public class Texture : IDisposable
    {
        private readonly RendererBase m_Renderer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Texture" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        public Texture(RendererBase renderer)
        {
            m_Renderer = renderer;
            Width = 4;
            Height = 4;
            Failed = false;
        }

        /// <summary>
        ///     Texture name. Usually file name, but exact meaning depends on renderer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Renderer data.
        /// </summary>
        public object RendererData { get; set; }

        /// <summary>
        ///     Indicates that the texture failed to load.
        /// </summary>
        public bool Failed { get; set; }

        /// <summary>
        ///     Texture width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        ///     Texture height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_Renderer.FreeTexture(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Loads the specified texture.
        /// </summary>
        /// <param name="name">Texture name.</param>
        /// <param name="errorCallback">Callback to invoke when texture fails to load.</param>
        public void Load(string name, Action<Exception> errorCallback)
        {
            Name = name;
            m_Renderer.LoadTexture(this, errorCallback);
        }

        /// <summary>
        ///     Initializes the texture from raw pixel data.
        /// </summary>
        /// <param name="width">Texture width.</param>
        /// <param name="height">Texture height.</param>
        /// <param name="pixelData">Color array in RGBA format.</param>
        public void LoadRaw(int width, int height, byte[] pixelData)
        {
            Width = width;
            Height = height;
            m_Renderer.LoadTextureRaw(this, pixelData);
        }

#if DEBUG
        ~Texture()
        {
            throw new InvalidOperationException(String.Format("IDisposable object finalized: {0}", GetType()));
            //Debug.Print(String.Format("IDisposable object finalized: {0}", GetType()));
        }
#endif
    }
}
