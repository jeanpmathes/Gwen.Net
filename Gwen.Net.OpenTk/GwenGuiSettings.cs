using System.IO;

namespace Gwen.Net.OpenTk
{
    public class GwenGuiSettings
    {
        public static readonly GwenGuiSettings Default = new GwenGuiSettings
        {
            DefaultFont = "Calibri",
            Renderer = GwenGuiRenderer.GL40,
            DrawBackground = true
        };

        //Make this a source or stream?
        public FileInfo SkinFile { get; set; }

        public string DefaultFont { get; set; }

        public GwenGuiRenderer Renderer { get; set; }

        public bool DrawBackground { get; set; }

        private GwenGuiSettings() { }
    }
}
