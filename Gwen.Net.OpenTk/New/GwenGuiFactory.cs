using OpenTK.Windowing.Desktop;

namespace Gwen.Net.OpenTk.New;

public static class GwenGuiFactory
{
    public static IGwenGui CreateFromGame(GameWindow window)
    {
        return new GwenGui(window);
    }
}
