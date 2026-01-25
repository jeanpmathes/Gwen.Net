using System;

namespace Gwen.Net.OpenTk.Legacy
{
    public static class GwenGuiSettingsExtensions
    {
        public static GwenGuiSettings From(this GwenGuiSettings settings, Action<GwenGuiSettings> settingsModifier)
        {
            settingsModifier?.Invoke(settings);

            return settings;
        }
    }
}