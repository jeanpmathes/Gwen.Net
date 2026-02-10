using System;
using Gwen.Net.New;
using Gwen.Net.New.Controls;
using OpenTK.Mathematics;

namespace Gwen.Net.OpenTk.New;

public interface IGwenGui : IDisposable
{
    Canvas? Root { get; } // todo: maybe make this a Element, not Canvas, or maybe remove completely and add a method to just set the content

    void Load();

    void Resize(Vector2i newSize);

    void Render();
}
