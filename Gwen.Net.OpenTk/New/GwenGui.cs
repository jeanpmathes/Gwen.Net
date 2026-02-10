using System.Drawing;
using Gwen.Net.New;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Resources;
using Gwen.Net.OpenTk.New.Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Gwen.Net.OpenTk.New;

internal class GwenGui : IGwenGui
{
    private readonly ResourceRegistry registry;
    
    internal GwenGui(GameWindow parent, ResourceRegistry registry)
    {
        Parent = parent;
        
        this.registry = registry;
    }

    public GameWindow Parent { get; }

    public Canvas? Root { get; private set; }
    
    private Renderer? Renderer { get; set; }

    public void Load()
    {
        Renderer = new Renderer();
        Root = Canvas.Create(Renderer, registry);
        
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
