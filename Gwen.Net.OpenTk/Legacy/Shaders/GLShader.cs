using System;
using OpenTK.Graphics.OpenGL;

namespace Gwen.Net.OpenTk.Legacy.Shaders
{
    public class GLShader : IShader
    {
        public GLShader(Int32 program, Int32 vertexShader, Int32 fragmentShader)
        {
            Program = program;
            VertexShader = vertexShader;
            FragmentShader = fragmentShader;
            Uniforms = new UniformDictionary(program, GL.GetUniformLocation);
        }

        public Int32 Program { get; }
        public Int32 VertexShader { get; }
        public Int32 FragmentShader { get; }
        public UniformDictionary Uniforms { get; }

        public void Dispose()
        {
            GL.DeleteProgram(Program);
        }
    }
}
