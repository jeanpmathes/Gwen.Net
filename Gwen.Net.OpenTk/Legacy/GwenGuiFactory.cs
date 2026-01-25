using OpenTK.Windowing.Desktop;

namespace Gwen.Net.OpenTk.Legacy
{
    public static class GwenGuiFactory
    {
        public static IGwenGui CreateFromGame(GameWindow window, GwenGuiSettings settings = default)
        {
            settings ??= GwenGuiSettings.Default;

            return new GwenGui(window, settings);
        }
    }
}
