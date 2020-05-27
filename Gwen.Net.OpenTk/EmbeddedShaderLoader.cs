using System;
using System.IO;

namespace Gwen.Net.OpenTk
{
    internal static class EmbeddedShaderLoader
    {
        public static string GetShader<T>(string type)
        {
            var programType = typeof(T);
            string shaderName = $"{programType.FullName}.{type}";

            var stream = programType.Assembly.GetManifestResourceStream(shaderName);
            if(stream == null)
            {
                throw new Exception($"Resource '{shaderName}' not found");
            }

            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
