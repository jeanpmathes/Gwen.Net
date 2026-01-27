using System;
using OpenTK.Graphics.OpenGL4;

namespace Gwen.Net.OpenTk.New.Graphics;

public sealed class Shader(Int32 program, Int32 vertexShader, Int32 fragmentShader) : IDisposable
{
    public Int32 Program { get; } = program;
    public Int32 VertexShader { get; } = vertexShader;
    public Int32 FragmentShader { get; } = fragmentShader;
    public UniformDictionary Uniforms { get; } = new(program, GL.GetUniformLocation);

    public void Dispose()
    {
        GL.DeleteProgram(Program);
    }
}
