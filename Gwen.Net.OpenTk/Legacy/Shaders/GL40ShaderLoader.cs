using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace Gwen.Net.OpenTk.Legacy.Shaders
{
    public class GL40ShaderLoader : IShaderLoader
    {
        public IShader Load(String shaderName)
        {
            return Load(shaderName, shaderName);
        }

        public IShader Load(String vertexShaderName, String fragmentShaderName)
        {
            String vSource = EmbeddedShaderLoader.GetShader<GL40ShaderLoader>(vertexShaderName, "vert");
            String fSource = EmbeddedShaderLoader.GetShader<GL40ShaderLoader>(fragmentShaderName, "frag");

            Int32 vShader = GL.CreateShader(ShaderType.VertexShader);
            Int32 fShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vShader, vSource);
            GL.ShaderSource(fShader, fSource);
            // Compile shaders
            GL.CompileShader(vShader);
            GL.CompileShader(fShader);
            Debug.WriteLine(GL.GetShaderInfoLog(vShader));
            Debug.WriteLine(GL.GetShaderInfoLog(fShader));

            Int32 program = GL.CreateProgram();
            // Link and attach shaders to program
            GL.AttachShader(program, vShader);
            GL.AttachShader(program, fShader);

            GL.LinkProgram(program);
            Debug.WriteLine(GL.GetProgramInfoLog(program));

            return new GLShader(program, vShader, fShader);
        }
    }
}
