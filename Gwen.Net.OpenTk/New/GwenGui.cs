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
        Root = new Canvas();
        Renderer = new Renderer();

        Size size = new(Parent.Size.X, Parent.Size.Y);
        Root.SetSize(size);
        Renderer.Resize(size);
    }

    public void Render()
    {
        if (Root == null || Renderer == null)
            return;
        
        Root.Render(Renderer);
    }

    public void Resize(Vector2i size)
    {
        Size newSize = new(size.X, size.Y);
        Renderer?.Resize(newSize);
        Root?.SetSize(newSize);
    }

    public void Dispose()
    {
        Root?.Dispose();
        Renderer?.Dispose();
    }
}
