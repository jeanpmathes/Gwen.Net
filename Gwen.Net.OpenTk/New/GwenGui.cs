using System.Drawing;
using Gwen.Net.New;
using Gwen.Net.OpenTk.New.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Gwen.Net.OpenTk.New;

internal class GwenGui : IGwenGui
{
    internal GwenGui(GameWindow parent)
    {
        Parent = parent;
    }

    public GameWindow Parent { get; }

    public Canvas? Root { get; private set; }
    
    private Renderer? Renderer { get; set; }

    public void Load()
    {
        Renderer = new Renderer();
        Root = new Canvas(Renderer);
        
        Root?.SetRenderingSize(new Size(Parent.Size.X, Parent.Size.Y));
    }

    public void Render()
    {
        Root?.Render();
    }

    public void Resize(Vector2i size)
    {
        Root?.SetRenderingSize(new Size(size.X, size.Y));
    }

    public void Dispose()
    {
        Root?.Dispose();
        Renderer?.Dispose();
    }
}
