using System;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace Gwen.Net.OpenTk.New.Graphics;

public static class ShaderLoader
{
    public static Shader Load(String shaderName)
    {
        return Load(shaderName, shaderName);
    }

    public static Shader Load(String vertexShaderName, String fragmentShaderName)
    {
        String vSource = GetShader(vertexShaderName, "vert");
        String fSource = GetShader(fragmentShaderName, "frag");

        Int32 vShader = GL.CreateShader(ShaderType.VertexShader);
        Int32 fShader = GL.CreateShader(ShaderType.FragmentShader);

        GL.ShaderSource(vShader, vSource);
        GL.ShaderSource(fShader, fSource);

        GL.CompileShader(vShader);
        GL.CompileShader(fShader);
        Debug.WriteLine(GL.GetShaderInfoLog(vShader));
        Debug.WriteLine(GL.GetShaderInfoLog(fShader));

        Int32 program = GL.CreateProgram();
        GL.AttachShader(program, vShader);
        GL.AttachShader(program, fShader);
        GL.LinkProgram(program);
        Debug.WriteLine(GL.GetProgramInfoLog(program));

        return new Shader(program, vShader, fShader);
    }
    
    public static String GetShader(String name, String type)
    {
        Type rootType = typeof(ShaderLoader);
        var shaderName = $"{rootType.Namespace}.{name}.{type}";
        Stream? stream = rootType.Assembly.GetManifestResourceStream(shaderName);

        if (stream == null)
            throw new InvalidOperationException($"Resource '{shaderName}' not found");

        using StreamReader reader = new(stream);

        return reader.ReadToEnd();
    }
}
