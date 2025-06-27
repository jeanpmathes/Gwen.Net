using System;

namespace Gwen.Net.OpenTk.Shaders
{
    public interface IShaderLoader
    {
        IShader Load(String shaderName);

        IShader Load(String vertexShaderName, String fragmentShaderName);
    }
}