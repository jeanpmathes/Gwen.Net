using Gwen.Net.New.Resources;
using OpenTK.Windowing.Desktop;

namespace Gwen.Net.OpenTk.New;

public static class GwenGuiFactory
{
    public static IGwenGui CreateFromGame(GameWindow window, ResourceRegistry registry)
    {
        return new GwenGui(window, registry);
    }
}
