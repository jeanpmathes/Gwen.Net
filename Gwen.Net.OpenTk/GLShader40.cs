using System;
using OpenToolkit.Graphics.OpenGL;
using System.Diagnostics;
using System.Collections.Generic;

namespace Gwen.Net.OpenTk
{
    public class GLShader40 : IDisposable
    {
        public int Program { get; set; }
        public int VertexShader { get; set; }
        public int FragmentShader { get; set; }

        private UniformDictionary _uniforms;
        public UniformDictionary Uniforms { get { return _uniforms; } set { return; } }

        public GLShader40()
        {
            this.Program = 0;
            this.VertexShader = 0;
            this.FragmentShader = 0;
        }

        public void Load(string shaderName)
        {
            Load(shaderName, shaderName);
        }

        public void Apply()
        {
            GL.UseProgram(this.Program);
        }

        public void Load(string vertexShaderName, string fragmentShaderName)
        {
            string vSource = EmbeddedShaderLoader.GetShader<GLShader40>("vert");
            string fSource = EmbeddedShaderLoader.GetShader<GLShader40>("frag");

            int vShader = GL.CreateShader(ShaderType.VertexShader);
            int fShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vShader, vSource);
            GL.ShaderSource(fShader, fSource);
            // Compile shaders
            GL.CompileShader(vShader);
            GL.CompileShader(fShader);
            Debug.WriteLine(GL.GetShaderInfoLog(vShader));
            Debug.WriteLine(GL.GetShaderInfoLog(fShader));

            int program = GL.CreateProgram();
            // Link and attach shaders to program
            GL.AttachShader(program, vShader);
            GL.AttachShader(program, fShader);

            GL.LinkProgram(program);
            Debug.WriteLine(GL.GetProgramInfoLog(program));

            this.Program = program;
            this.VertexShader = vShader;
            this.FragmentShader = fShader;
            this._uniforms = new UniformDictionary(Program);
        }

        public class UniformDictionary
        {
            private Dictionary<string, int> _data;
            private int _program;

            public UniformDictionary(int program)
            {
                _data = new Dictionary<string, int>();
                _program = program;
            }

            public int this[string key]
            {
                get
                {
                    if (!this._data.ContainsKey(key))
                    {
                        int uniformLocation = GL.GetUniformLocation(_program, key);
                        this._data.Add(key, uniformLocation);
                    }

                    int loc = -1;
                    this._data.TryGetValue(key, out loc);

                    return loc;
                }
            }
        }

        public void Dispose()
        {
            GL.DeleteProgram(this.Program);
        }
    }
}
