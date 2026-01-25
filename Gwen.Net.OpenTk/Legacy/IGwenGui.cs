using System;
using Gwen.Net.Legacy.Control;
using OpenTK.Mathematics;

namespace Gwen.Net.OpenTk.Legacy
{
    public interface IGwenGui : IDisposable
    {
        ControlBase Root { get; }

        void Load();

        void Resize(Vector2i newSize);

        void Render();
    }
}
