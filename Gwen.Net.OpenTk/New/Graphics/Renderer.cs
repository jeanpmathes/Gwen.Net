using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Boolean = System.Boolean;
using Brush = Gwen.Net.New.Graphics.Brush;

namespace Gwen.Net.OpenTk.New.Graphics;

public sealed class Renderer : IRenderer, IDisposable
{
    private const Int32 MaxVerts = 4096;

    private readonly Shader shader;
    private readonly Boolean restoreRenderState;

    private readonly Vertex[] vertices;
    private readonly Int32 vertexSize;
    
    private static readonly SizeF initialOffset = SizeF.Empty;
    private static readonly RectangleF initialClip = new(x: 0, y: 0, Int32.MaxValue, Int32.MaxValue);
    
    private readonly Stack<SizeF> offsetStack = new();
    private readonly Stack<RectangleF> clipStack = new();

    private Int32 numberOfVertices;

    private Int32 vao;
    private Int32 vbo;

    private Int32 lastTextureID;
    private Boolean textureEnabled;

    private RenderState previousRenderState;
    
    private Single scale = 1.0f;

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

    public void Begin()
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
        IsClippingEnabled = false;
        textureEnabled = false;
        lastTextureID = -1;
    }

    public void End()
    {
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

    public void PushOffset(PointF offset)
    {
        offsetStack.Push(Offset);
        
        Offset = new SizeF(Offset.Width + offset.X, Offset.Height + offset.Y);
    }
    
    public void PopOffset()
    {
        Offset = offsetStack.Count > 0 ? offsetStack.Pop() : initialOffset;
    }
    
    public void PushClip(RectangleF rectangle)
    {
        clipStack.Push(Clip);

        rectangle = Transform(rectangle);

        RectangleF result = rectangle;

        if (rectangle.X < Clip.X)
        {
            result.Width -= Clip.X - result.X;
            result.X = Clip.X;
        }

        if (rectangle.Y < Clip.Y)
        {
            result.Height -= Clip.Y - result.Y;
            result.Y = Clip.Y;
        }

        if (rectangle.Right > Clip.Right)
        {
            result.Width = Clip.Right - result.X;
        }

        if (rectangle.Bottom > Clip.Bottom)
        {
            result.Height = Clip.Bottom - result.Y;
        }

        Clip = result with
        {
            Width = Math.Max(val1: 0, result.Width),
            Height = Math.Max(val1: 0, result.Height)
        };
    }
    
    public void PopClip()
    {
        Clip = clipStack.Count > 0 ? clipStack.Pop() : initialClip;
    }
    
    public void BeginClip()
    {
        IsClippingEnabled = true;
    }
    
    public void EndClip()
    {
        IsClippingEnabled = false;
    }

    public Boolean IsClipEmpty()
    {
        return Clip.Width <= 0 || Clip.Height <= 0;
    }

    private SizeF Offset { get; set; } = initialOffset;
    
    private RectangleF Clip { get; set; } = initialClip;

    private Boolean IsClippingEnabled { get; set; }
    
    public void DrawFilledRectangle(RectangleF rectangle, Brush brush)
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
        Vector2 uv0 = uvA ?? (0f, 0f);
        Vector2 uv1 = uvB ?? (1f, 1f);
        
        if (numberOfVertices + 6 >= MaxVerts)
        {
            Flush();
        }

        if (IsClippingEnabled)
        {
            if (rectangle.Y < Clip.Y)
            {
                Single oldHeight = rectangle.Height;
                Single delta = Clip.Y - rectangle.Y;
                rectangle.Y = Clip.Y;
                rectangle.Height -= delta;

                if (rectangle.Height <= 0)
                {
                    return;
                }

                Single dv = delta / oldHeight;
                uv0.Y += dv * (uv1.Y - uv0.Y);
            }

            if (rectangle.Y + rectangle.Height > Clip.Y + Clip.Height)
            {
                Single oldHeight = rectangle.Height;
                Single delta = rectangle.Y + rectangle.Height - (Clip.Y + Clip.Height);

                rectangle.Height -= delta;

                if (rectangle.Height <= 0)
                {
                    return;
                }

                Single dv = delta / oldHeight;
                uv1.Y -= dv * (uv1.Y - uv0.Y);
            }

            if (rectangle.X < Clip.X)
            {
                Single oldWidth = rectangle.Width;
                Single delta = Clip.X - rectangle.X;
                rectangle.X = Clip.X;
                rectangle.Width -= delta;

                if (rectangle.Width <= 0)
                {
                    return;
                }

                Single du = delta / oldWidth;
                uv0.X += du * (uv1.X - uv0.X);
            }

            if (rectangle.X + rectangle.Width > Clip.X + Clip.Width)
            {
                Single oldWidth = rectangle.Width;
                Single delta = rectangle.X + rectangle.Width - (Clip.X + Clip.Width);

                rectangle.Width -= delta;

                if (rectangle.Width <= 0)
                {
                    return;
                }

                Single du = delta / oldWidth;
                uv1.X -= du * (uv1.X - uv0.X);
            }
        }

        Int32 vertexIndex = numberOfVertices;
        
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X, rectangle.Y, uv0, in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X + rectangle.Width, rectangle.Y, (uv1.X, uv0.Y), in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, uv1, in color);
        
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X, rectangle.Y, uv0, in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, uv1, in color);
        Vertex.Set(ref vertices[vertexIndex++], rectangle.X, rectangle.Y + rectangle.Height, (uv0.X, uv1.Y), in color);

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

    public void Resize(Size size)
    {
        GL.Viewport(x: 0, y: 0, width: size.Width, height: size.Height);
        GL.UseProgram(shader.Program);
        GL.Uniform2(shader.Uniforms["uScreenSize"], new Vector2(size.Width, size.Height));
    }

    public void Scale(Single newScale)
    {
        scale = newScale;
    }

    public RectangleF Transform(RectangleF rectangle)
    {
        rectangle.Location += Offset;
        
        rectangle.X *= scale;
        rectangle.Y *= scale;
        rectangle.Width *= scale;
        rectangle.Height *= scale;

        return rectangle;
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
    
    public struct RenderState
    {
        public Int32 alphaFunc;
        public Single alphaRef;
        public Int32 blendDst;
        public Int32 blendSrc;

        public Boolean depthTestEnabled;
        public Boolean blendEnabled;
    }
}
