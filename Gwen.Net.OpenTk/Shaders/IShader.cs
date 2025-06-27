using System;

namespace Gwen.Net.OpenTk.Shaders
{
    public interface IShader : IDisposable
    {
        Int32 Program { get; }
        Int32 VertexShader { get; }
        Int32 FragmentShader { get; }

        UniformDictionary Uniforms { get; }
    }
}