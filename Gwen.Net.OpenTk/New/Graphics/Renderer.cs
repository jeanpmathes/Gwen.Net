using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Gwen.Net.New.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Boolean = System.Boolean;
using Brush = Gwen.Net.New.Graphics.Brush;

namespace Gwen.Net.OpenTk.New.Graphics;

public sealed class Renderer : Gwen.Net.New.Rendering.Renderer, IDisposable
{
    private const Int32 MaxVerts = 4096;

    private readonly Shader shader;
    private readonly Boolean restoreRenderState;

    private readonly Vertex[] vertices;
    private readonly Int32 vertexSize;

    private Int32 numberOfVertices;

    private Int32 vao;
    private Int32 vbo;

    private Int32 lastTextureID;
    private Boolean textureEnabled;

    private RenderState previousRenderState;

    public Renderer(Boolean restoreRenderState = true)
    {
        this.restoreRenderState = restoreRenderState;

        vertices = new Vertex[MaxVerts];
        vertexSize = Marshal.SizeOf(vertices[0]);

        CreateBuffers();

        shader = ShaderLoader.Load("gui");
    }

    private void CreateBuffers()
    {
        GL.GenVertexArrays(n: 1, out vao);
        GL.BindVertexArray(vao);

        GL.GenBuffers(n: 1, out vbo);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

        GL.BufferData(
            BufferTarget.ArrayBuffer,
            (IntPtr) (vertexSize * MaxVerts),
            IntPtr.Zero,
            BufferUsageHint.StreamDraw);

        // Positions (x, y):
        GL.EnableVertexAttribArray(index: 0);

        GL.VertexAttribPointer(
            index: 0,
            size: 2,
            VertexAttribPointerType.Float,
            normalized: false,
            vertexSize,
            offset: 0);

        // Texture Coordinates (u, v):
        GL.EnableVertexAttribArray(index: 1);

        GL.VertexAttribPointer(
            index: 1,
            size: 2,
            VertexAttribPointerType.Float,
            normalized: false,
            vertexSize,
            2 * sizeof(Single));

        // Colors (r, g, b, a):
        GL.EnableVertexAttribArray(index: 2);

        GL.VertexAttribPointer(
            index: 2,
            size: 4,
            VertexAttribPointerType.Float,
            normalized: false,
            vertexSize,
            2 * (sizeof(Single) + sizeof(Single)));

        GL.BindBuffer(BufferTarget.ArrayBuffer, buffer: 0);
        GL.BindVertexArray(array: 0);
    }

    public override void Begin()
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.UseProgram(shader.Program);

        GL.BindVertexArray(vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

        if (restoreRenderState)
        {
            GL.GetInteger(GetPName.BlendSrc, out previousRenderState.blendSrc);
            GL.GetInteger(GetPName.BlendDst, out previousRenderState.blendDst);
            GL.GetInteger(GetPName.AlphaTestFunc, out previousRenderState.alphaFunc);
            GL.GetFloat(GetPName.AlphaTestRef, out previousRenderState.alphaRef);

            previousRenderState.blendEnabled = GL.IsEnabled(EnableCap.Blend);
            previousRenderState.depthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
        }

        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);
        GL.Disable(EnableCap.DepthTest);

        numberOfVertices = 0;
        textureEnabled = false;
        lastTextureID = -1;
        
        base.Begin();
    }

    public override void End()
    {
        base.End();
        
        Flush();

        GL.BindVertexArray(array: 0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, buffer: 0);

        if (!restoreRenderState) return;

        GL.BindTexture(TextureTarget.Texture2D, texture: 0);
        lastTextureID = 0;

        GL.BlendFunc((BlendingFactor) previousRenderState.blendSrc, (BlendingFactor) previousRenderState.blendDst);
        GL.AlphaFunc((AlphaFunction) previousRenderState.alphaFunc, previousRenderState.alphaRef);

        if (!previousRenderState.blendEnabled)
        {
            GL.Disable(EnableCap.Blend);
        }

        if (previousRenderState.depthTestEnabled)
        {
            GL.Enable(EnableCap.DepthTest);
        }
    }
    
    public override void DrawFilledRectangle(RectangleF rectangle, Brush brush)
    {
        if (!ConvertBrush(brush, out ColorData color)) return;
        
        if (textureEnabled)
        {
            Flush();
            textureEnabled = false;
        }

        rectangle = Transform(rectangle);

        DrawRectangle(rectangle, color);
    }

    private static Boolean ConvertBrush(Brush brush, out ColorData color)
    {
        color = default;

        switch (brush)
        {
            case SolidColorBrush solidColorBrush:
                color = ColorData.FromColor(solidColorBrush.Color);
                return true;
            
            default:
                return false;
        }
    }
    
    private void DrawRectangle(RectangleF rectangle, in ColorData color, Vector2? uvA = null, Vector2? uvB = null)
    {
        (Single x, Single y) uv0 = (uvA?.X ?? 0f, uvA?.Y ?? 0f);
        (Single x, Single y) uv1 = (uvB?.X ?? 1f, uvB?.Y ?? 1f);
        
        if (numberOfVertices + 6 >= MaxVerts)
        {
            Flush();
        }

        ClipRectangle(ref rectangle, ref uv0, ref uv1);

        Int32 vertexIndex = numberOfVertices;
        
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X, rectangle.Y, uv0, in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X + rectangle.Width, rectangle.Y, (uv1.x, uv0.y), in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, uv1, in color);
        
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X, rectangle.Y, uv0, in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, uv1, in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X, rectangle.Y + rectangle.Height, (uv0.x, uv1.y), in color);

        numberOfVertices = vertexIndex;
    }

    private void Flush()
    {
        if (numberOfVertices == 0)
        {
            return;
        }

        GL.InvalidateBufferData(vbo);

        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr) (numberOfVertices * vertexSize), vertices);

        //GL.Uniform1(shader.Uniforms["uUseTexture"], textureEnabled ? 1.0f : 0.0f);
        // todo: bring the above comment back

        GL.DrawArrays(PrimitiveType.Triangles, first: 0, numberOfVertices);

        numberOfVertices = 0;
    }

    public override void Resize(Size size)
    {
        GL.Viewport(x: 0, y: 0, width: size.Width, height: size.Height);
        GL.UseProgram(shader.Program);
        GL.Uniform2(shader.Uniforms["uScreenSize"], new Vector2(size.Width, size.Height));
    }

    public void Dispose()
    {
        GL.DeleteBuffer(vbo);
        GL.DeleteVertexArray(vao);

        shader.Dispose();
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorData
    {
        public Single r, g, b, a;

        public static ColorData FromColor(Color color)
        {
            return new ColorData
            {
                r = color.R / 255f,
                g = color.G / 255f,
                b = color.B / 255f,
                a = color.A / 255f
            };
        }
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vertex
    {
        public Single x, y;
        public Single u, v;
        public ColorData color;
        
        public static void Set(ref Vertex vertex, Single x, Single y, Vector2 uv, in ColorData color)
        {
            vertex.x = x;
            vertex.y = y;
            vertex.u = uv.X;
            vertex.v = uv.Y;
            vertex.color = color;
        }
    }

    private struct RenderState
    {
        public Int32 alphaFunc;
        public Single alphaRef;
        public Int32 blendDst;
        public Int32 blendSrc;

        public Boolean depthTestEnabled;
        public Boolean blendEnabled;
    }
}
