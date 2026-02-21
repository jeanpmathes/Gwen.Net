using System;
using System.Collections.Generic;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Texts;
using Gwen.Net.New.Utilities;
using OpenTK.Graphics.OpenGL;
using Boolean = System.Boolean;
using Brush = Gwen.Net.New.Graphics.Brush;
using Font = Gwen.Net.New.Texts.Font;

namespace Gwen.Net.OpenTk.New.Graphics;

public sealed class Renderer : Gwen.Net.New.Rendering.Renderer, IDisposable
{
    private readonly Shader shader;
    private readonly Boolean restoreRenderState;

    private readonly Dictionary<Brush, System.Drawing.Brush> systemBrushes = [];
    private readonly Dictionary<Brush, System.Drawing.Pen> systemPens = [];
    private readonly Dictionary<Font, System.Drawing.Font> systemFonts = [];

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
        
        if (isClipping && clipStack.Count > 0)
        {
            ApplyClippingRectangle(clipStack.Peek());
        }
        else if (offsetStack.Count > 0)
        {
            System.Drawing.PointF offset = offsetStack.Peek();
            graphics.TranslateTransform(offset.X, offset.Y);
        }
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
    
    public override void Resize(System.Drawing.Size size)
    {
        GL.Viewport(x: 0, y: 0, width: size.Width, height: size.Height);
        
        ResizeTargets(size);
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
    
    #endregion TARGETS
    
    #region TRANSFORM & CLIP
    
    private readonly Stack<System.Drawing.PointF> offsetStack = new();
    private readonly Stack<System.Drawing.RectangleF> clipStack = new();
    
    private Boolean isClipping;
    
    public override void PushOffset(System.Drawing.PointF offset)
    {
        offset = ApplyScale(offset);
        
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
        rectangle = ApplyScale(rectangle);

        System.Drawing.PointF absoluteOffset = offsetStack.Count > 0
            ? offsetStack.Peek()
            : new System.Drawing.PointF(x: 0f, y: 0f);

        System.Drawing.RectangleF effectiveRectangle = rectangle with
        {
            X = rectangle.X + absoluteOffset.X,
            Y = rectangle.Y + absoluteOffset.Y
        };

        if (clipStack.Count > 0)
            effectiveRectangle = System.Drawing.RectangleF.Intersect(clipStack.Peek(), effectiveRectangle);

        clipStack.Push(effectiveRectangle);

        if (isClipping)
            ApplyClippingRectangle(effectiveRectangle);
    }

    public override void PopClip()
    {
        clipStack.Pop();

        if (!isClipping) return;

        graphics?.ResetClip();

        if (clipStack.Count > 0)
            ApplyClippingRectangle(clipStack.Peek());
    }

    public override void BeginClip()
    {
        if (isClipping) return;

        isClipping = true;

        if (clipStack.Count > 0)
            ApplyClippingRectangle(clipStack.Peek());
    }

    private void ApplyClippingRectangle(System.Drawing.RectangleF effectiveRectangle)
    {
        if (graphics == null) return;
        
        graphics.ResetTransform();
        graphics.SetClip(effectiveRectangle, System.Drawing.Drawing2D.CombineMode.Replace);

        System.Drawing.PointF absoluteOffset = offsetStack.Count > 0
            ? offsetStack.Peek()
            : new System.Drawing.PointF(x: 0f, y: 0f);

        if (absoluteOffset.X != 0f || absoluteOffset.Y != 0f)
            graphics.TranslateTransform(absoluteOffset.X, absoluteOffset.Y);
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
    
    #region TEXT

    public override IFormattedText CreateFormattedText(String text, Font font, TextOptions options)
    {
        return new FormattedText(this, text, font, options);
    }

    public System.Drawing.SizeF MeasureText(FormattedText text, System.Drawing.SizeF availableSize)
    {
        availableSize = ApplyScale(availableSize);
        
        if (graphics == null) return System.Drawing.SizeF.Empty;

        System.Drawing.Font systemFont = GetFont(text.Font);
        
        System.Drawing.SizeF measuredSize = graphics.MeasureString(text.Text, systemFont, availableSize, text.StringFormat);
        
        return ApplyInverseScale(measuredSize);
    }
    
    public void DrawText(FormattedText text, System.Drawing.RectangleF rectangle, Brush brush)
    {
        rectangle = ApplyScale(rectangle);
        
        System.Drawing.Brush? systemBrush = GetBrush(brush);
        
        if (systemBrush == null) return;
        
        System.Drawing.Font systemFont = GetFont(text.Font);
        
        graphics?.DrawString(text.Text, systemFont, systemBrush, rectangle, text.StringFormat);
    }
    
    #endregion TEXT
    
    #region RECTANGLES
    
    public override void DrawFilledRectangle(System.Drawing.RectangleF rectangle, Brush brush)
    {
        System.Drawing.Brush? systemBrush = GetBrush(brush);
        
        if (systemBrush == null) return;

        rectangle = ApplyScale(rectangle);

        graphics?.FillRectangle(systemBrush, rectangle);
    }

    public override void DrawLinedRectangle(System.Drawing.RectangleF rectangle, ThicknessF thickness, Brush brush)
    {
        System.Drawing.Brush? systemBrush = GetBrush(brush);
        
        if (systemBrush == null) return;

        rectangle = ApplyScale(rectangle);
        thickness = ApplyScale(thickness);
        
        if (thickness.IsUniform)
        {
            System.Drawing.Pen? systemPen = GetPen(brush, thickness.Left);
            if (systemPen == null) return;

            Single halfT = thickness.Left / 2f;
            graphics?.DrawRectangle(systemPen, rectangle.X + halfT, rectangle.Y + halfT, rectangle.Width - thickness.Left, rectangle.Height - thickness.Left);
        }
        else
        {
            graphics?.FillRectangle(systemBrush, rectangle.X, rectangle.Y, thickness.Left, rectangle.Height);
            graphics?.FillRectangle(systemBrush, rectangle.X, rectangle.Y, rectangle.Width, thickness.Top);
            graphics?.FillRectangle(systemBrush, rectangle.X + rectangle.Width - thickness.Right, rectangle.Y, thickness.Right, rectangle.Height);
            graphics?.FillRectangle(systemBrush, rectangle.X, rectangle.Y + rectangle.Height - thickness.Bottom, rectangle.Width, thickness.Bottom);
        }
    }

    #endregion RECTANGLES

    #region MAPPINGS
    
    private System.Drawing.Brush? GetBrush(Brush brush)
    {
        if (systemBrushes.TryGetValue(brush, out System.Drawing.Brush? systemBrush))
            return systemBrush;

        systemBrush = brush switch
        {
            SolidColorBrush solidColorBrush => new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(solidColorBrush.Color.A, solidColorBrush.Color.R, solidColorBrush.Color.G, solidColorBrush.Color.B)),
            TransparentBrush => null,
            _ => systemBrush
        };
        
        if (systemBrush != null)
            systemBrushes[brush] = systemBrush;
        
        return systemBrush;
    }
    
    private System.Drawing.Pen? GetPen(Brush brush, Single thickness)
    {
        if (systemPens.TryGetValue(brush, out System.Drawing.Pen? systemPen))
        {
            systemPen.Width = thickness;
            return systemPen;
        }

        System.Drawing.Brush? systemBrush = GetBrush(brush);
        
        if (systemBrush == null) return null;

        systemPen = new System.Drawing.Pen(systemBrush, thickness);
        systemPens[brush] = systemPen;
        
        return systemPen;
    }
    
    private System.Drawing.Font GetFont(Font font)
    {
        if (systemFonts.TryGetValue(font, out System.Drawing.Font? systemFont))
            return systemFont;

        systemFont = new System.Drawing.Font(font.Family, font.Size, GetFontStyleFromTextsStyle(font.Style) | GetFontStyleFromTextsWeight(font.Weight));
        
        systemFonts[font] = systemFont;
        
        return systemFont;
    }
    
    private static System.Drawing.FontStyle GetFontStyleFromTextsStyle(Style style)
    {
        return style switch
        {
            Style.Normal => System.Drawing.FontStyle.Regular,
            Style.Italic or Style.Oblique => System.Drawing.FontStyle.Italic,
            _ => System.Drawing.FontStyle.Regular
        };
    }
    
    private static System.Drawing.FontStyle GetFontStyleFromTextsWeight(Weight weight)
    {
        return weight >= Weight.SemiBold ? System.Drawing.FontStyle.Bold : System.Drawing.FontStyle.Regular;
    }
    
    #endregion MAPPINGS

    public void Dispose()
    {
        foreach (System.Drawing.Brush brush in systemBrushes.Values)
            brush.Dispose();
        
        foreach (System.Drawing.Font font in systemFonts.Values)
            font.Dispose();
        
        GL.DeleteTexture(texture);
        GL.DeleteVertexArray(vao);

        shader.Dispose();
        
        graphics?.Dispose();
        bitmap?.Dispose();
    }
}
