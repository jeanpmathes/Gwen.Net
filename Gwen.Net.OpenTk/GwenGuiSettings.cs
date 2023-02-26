using System;
using System.Collections.Generic;
using System.IO;

namespace Gwen.Net.OpenTk
{
    public record TexturePreload(FileInfo File, string Name);
    
    public class GwenGuiSettings
    {
        public static readonly GwenGuiSettings Default = new();

        private GwenGuiSettings() {}
        
        public FileInfo SkinFile { get; set; }

        public Action<Exception> SkinLoadingErrorCallback { get; set; } = _ => { };
        
        public List<TexturePreload> TexturePreloads { get; set; } = new();
        public Action<TexturePreload, Exception> TexturePreloadErrorCallback { get; set; } = (_, _) => { };
    }
}
