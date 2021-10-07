using System.IO;

namespace Gwen.Net.OpenTk
{
    public class GwenGuiSettings
    {
        public static readonly GwenGuiSettings Default = new() {DefaultFont = "Calibri", DrawBackground = true};

        private GwenGuiSettings() {}

        //Make this a source or stream?
        public FileInfo SkinFile { get; set; }

        public string DefaultFont { get; set; }

        public bool DrawBackground { get; set; }
    }
}