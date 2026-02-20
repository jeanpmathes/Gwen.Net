using System;
using System.Collections.Generic;
using Gwen.Net.New.Graphics;
using OpenTK.Graphics.OpenGL;
using Boolean = System.Boolean;

namespace Gwen.Net.OpenTk.New.Graphics;

public sealed class Renderer : Gwen.Net.New.Rendering.Renderer, IDisposable
{
    private readonly Shader shader;
    private readonly Boolean restoreRenderState;

    private readonly Dictionary<Brush, System.Drawing.Brush> brushes = [];

    private Int32 vao;
    private Int32 texture;
    
    private System.Drawing.Bitmap? bitmap;
    private System.Drawing.Graphics? graphics; 

    private RenderState previousRenderState;

    public Renderer(Boolean restoreRenderState = true)
    {
        this.restoreRenderState = restoreRenderState;

        CreateTargets(System.Drawing.Size.Empty);

        shader = ShaderLoader.Load("gui");
    }
    
    #region TARGETS
    
    private void CreateTargets(System.Drawing.Size size)
    {
        GL.CreateVertexArrays(n: 1, out vao);

        ResizeTargets(size);
    }

    private void ResizeTargets(System.Drawing.Size size)
    {
        if (texture != 0)
            GL.DeleteTexture(texture);

        GL.CreateTextures(TextureTarget.Texture2D, n: 1, out texture);
        GL.TextureParameter(texture, TextureParameterName.TextureMinFilter, (Int32) TextureMinFilter.Nearest);
        GL.TextureParameter(texture, TextureParameterName.TextureMagFilter, (Int32) TextureMagFilter.Nearest);
        GL.TextureParameter(texture, TextureParameterName.TextureWrapS, (Int32) TextureWrapMode.ClampToEdge);
        GL.TextureParameter(texture, TextureParameterName.TextureWrapT, (Int32) TextureWrapMode.ClampToEdge);

        if (size is {Width: > 0, Height: > 0})
            GL.TextureStorage2D(texture, levels: 1, SizedInternalFormat.Rgba8, size.Width, size.Height);

        graphics?.Dispose();
        bitmap?.Dispose();

        bitmap = null;
        graphics = null;

        if (size is not {Width: > 0, Height: > 0}) return;

        bitmap = new System.Drawing.Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        graphics = System.Drawing.Graphics.FromImage(bitmap);
        
        if (offsetStack.Count > 0)
        {
            System.Drawing.PointF offset = offsetStack.Peek();
            graphics.TranslateTransform(offset.X, offset.Y);
        }
        
        if (isClipping && clipStack.Count > 0)
            graphics.SetClip(clipStack.Peek(), System.Drawing.Drawing2D.CombineMode.Replace);
    }

    private void Draw()
    {
        if (bitmap == null || graphics == null) return;

        System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TextureSubImage2D(texture, level: 0, xoffset: 0, yoffset: 0, width: data.Width, height: data.Height, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        bitmap.UnlockBits(data);

        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, first: 0, count: 3);
        GL.BindVertexArray(0);
    }

    public override void Begin()
    {
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
        
        GL.UseProgram(shader.Program);
        
        GL.BindTextureUnit(unit: 0, texture);
        
        EndClip();
    }

    public override void End()
    {
        EndClip();
        
        Draw();
        
        GL.BindTextureUnit(unit: 0, texture: 0);
        GL.UseProgram(program: 0);

        if (!restoreRenderState) return;
        
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

    #endregion TARGETS
    
    #region TRANSFORM & CLIP
    
    private readonly Stack<System.Drawing.PointF> offsetStack = new();
    private readonly Stack<System.Drawing.RectangleF> clipStack = new();
    
    private Boolean isClipping;
    
    public override void PushOffset(System.Drawing.PointF offset)
    {
        offset = Scale(offset);
        
        graphics?.TranslateTransform(offset.X, offset.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
        
        if (offsetStack.Count > 0)
        {
            System.Drawing.PointF previousOffset = offsetStack.Peek();
            offset = new System.Drawing.PointF(previousOffset.X + offset.X, previousOffset.Y + offset.Y);
        }
        
        offsetStack.Push(offset);
    }
    
    public override void PopOffset()
    {
        offsetStack.Pop();
        
        graphics?.ResetTransform();
        
        if (offsetStack.Count > 0)
        {
            System.Drawing.PointF offset = offsetStack.Peek();
            graphics?.TranslateTransform(offset.X, offset.Y, System.Drawing.Drawing2D.MatrixOrder.Append);
        }
    }
    
    public override void PushClip(System.Drawing.RectangleF rectangle)
    {
        rectangle = Scale(rectangle);
        
        if (isClipping)
            graphics?.SetClip(rectangle, System.Drawing.Drawing2D.CombineMode.Intersect);
        
        if (clipStack.Count > 0)
        {
            System.Drawing.RectangleF previousClip = clipStack.Peek();
            rectangle = System.Drawing.RectangleF.Intersect(previousClip, rectangle);
        }
        
        clipStack.Push(rectangle);
    }
    
    public override void PopClip()
    {
        clipStack.Pop();
        
        if (isClipping)
            graphics?.ResetClip();
        
        if (clipStack.Count > 0)
        {
            System.Drawing.RectangleF rectangle = clipStack.Peek();
            
            if (isClipping)
                graphics?.SetClip(rectangle, System.Drawing.Drawing2D.CombineMode.Replace);
        }
    }
    
    public override void BeginClip()
    {
        if (isClipping) return;
        
        isClipping = true;
        
        if (clipStack.Count > 0)
        {
            System.Drawing.RectangleF rectangle = clipStack.Peek();
            graphics?.SetClip(rectangle, System.Drawing.Drawing2D.CombineMode.Replace);
        }
    }
    
    public override void EndClip() // todo: check when this is ever called, maybe this can be simplified
    {
        if (!isClipping) return;
        
        isClipping = false;
        
        graphics?.ResetClip();
    }
    
    public override Boolean IsClipEmpty()
    {
        if (graphics == null) return true;
        
        System.Drawing.Region clip = graphics.Clip;
        return clip.IsEmpty(graphics);
    }
    
    #endregion TRANSFORM & CLIP
    
    public override void DrawFilledRectangle(System.Drawing.RectangleF rectangle, Brush brush)
    {
        System.Drawing.Brush? systemBrush = GetBrush(brush);
        
        if (systemBrush == null) return;

        rectangle = Scale(rectangle);

        DrawRectangle(rectangle, systemBrush);
    }

    private void DrawRectangle(System.Drawing.RectangleF rectangle, System.Drawing.Brush systemBrush)
    {
        graphics?.FillRectangle(systemBrush, rectangle);
    }

    private System.Drawing.Brush? GetBrush(Brush brush)
    {
        if (brushes.TryGetValue(brush, out System.Drawing.Brush? systemBrush))
            return systemBrush;

        systemBrush = brush switch
        {
            SolidColorBrush solidColorBrush => new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B)),
            TransparentBrush => null,
            _ => systemBrush
        };
        
        if (systemBrush != null)
            brushes[brush] = systemBrush;
        
        return systemBrush;
    }
    
    public override void Resize(System.Drawing.Size size)
    {
        GL.Viewport(x: 0, y: 0, width: size.Width, height: size.Height);
        
        ResizeTargets(size);
    }

    public void Dispose()
    {
        foreach (System.Drawing.Brush brush in brushes.Values)
            brush.Dispose();
        
        GL.DeleteTexture(texture);
        GL.DeleteVertexArray(vao);

        shader.Dispose();
        
        graphics?.Dispose();
        bitmap?.Dispose();
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
