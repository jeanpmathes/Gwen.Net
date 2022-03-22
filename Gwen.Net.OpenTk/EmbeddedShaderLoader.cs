using System;
using System.IO;

namespace Gwen.Net.OpenTk
{
    internal static class EmbeddedShaderLoader
    {
        /// <summary>
        ///     Loads the shader source from an assembly. Where the <typeparamref name="TRoot" /> provides the root namespace to
        ///     resolve the shader resource
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetShader<TRoot>(string name, string type)
        {
            Type programType = typeof(TRoot);
            string shaderName = $"{programType.Namespace}.{name}.{type}";

            Stream stream = programType.Assembly.GetManifestResourceStream(shaderName);

            if (stream == null)
            {
                throw new InvalidOperationException($"Resource '{shaderName}' not found");
            }

            using StreamReader reader = new(stream);

            return reader.ReadToEnd();
        }
    }
}
