using System;
using System.Collections.Generic;
using System.Drawing;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.New.Rendering;

/// <summary>
/// Abstract base class that helps implement renderers.
/// </summary>
public abstract class Renderer : IRenderer
{
    private static readonly SizeF initialOffset = SizeF.Empty;
    private static readonly RectangleF initialClip = new(x: 0, y: 0, Int32.MaxValue, Int32.MaxValue);
    
    private readonly Stack<SizeF> offsetStack = new();
    private readonly Stack<RectangleF> clipStack = new();
    
    private Single scale = 1.0f;
    
    /// <summary>
    /// Get the current offset.
    /// </summary>
    protected SizeF Offset { get; set; } = initialOffset;
    
    /// <summary>
    /// Get the current clip rectangle.
    /// </summary>
    protected RectangleF Clip { get; set; } = initialClip;

    /// <summary>
    /// Whether clipping is currently enabled.
    /// </summary>
    protected Boolean IsClippingEnabled { get; private set; }

    /// <inheritdoc/>
    public virtual void Begin()
    {
        IsClippingEnabled = false;
    }

    /// <inheritdoc/>
    public virtual void End()
    {
        
    }

    /// <inheritdoc/>
    public virtual void PushOffset(PointF offset)
    {
        offsetStack.Push(Offset);
        
        Offset = new SizeF(Offset.Width + offset.X, Offset.Height + offset.Y);
    }
    
    /// <inheritdoc/>
    public virtual void PopOffset()
    {
        Offset = offsetStack.Count > 0 ? offsetStack.Pop() : initialOffset;
    }

    /// <inheritdoc/>
    public virtual void PushClip(RectangleF rectangle)
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
    
    /// <inheritdoc/>
    public virtual void PopClip() 
    {
        Clip = clipStack.Count > 0 ? clipStack.Pop() : initialClip;
    }
    
    /// <inheritdoc/>
    public virtual void BeginClip()
    {
        IsClippingEnabled = true;
    }

    /// <inheritdoc/>
    public virtual void EndClip()
    {
        IsClippingEnabled = false;
    }

    /// <inheritdoc/>
    public virtual Boolean IsClipEmpty()
    {
        return Clip.IsEmpty;
    }
    
    /// <inheritdoc/>
    public abstract void DrawFilledRectangle(RectangleF rectangle, Brush brush);

    /// <inheritdoc/>
    public virtual void Resize(Size size)
    {
        
    }

    /// <inheritdoc/>
    public virtual void Scale(Single newScale)
    {
        scale = newScale;
    }
    
    /// <summary>
    /// Transform a rectangle by applying offset and scale.
    /// This transforms it from local element space into screen space.
    /// </summary>
    /// <param name="rectangle">The rectangle to transform.</param>
    /// <returns>The transformed rectangle.</returns>
    protected RectangleF Transform(RectangleF rectangle)
    {
        rectangle.Location += Offset;
        
        rectangle.X *= scale;
        rectangle.Y *= scale;
        rectangle.Width *= scale;
        rectangle.Height *= scale;

        return rectangle;
    }
    
    /// <summary>
    /// Clip a (transformed) rectangle against the current clip rectangle.
    /// Adjusts the rectangle and UV coordinates accordingly.
    /// </summary>
    /// <param name="rectangle">The rectangle to clip. Must be transformed already.</param>
    /// <param name="uv0">The UV coordinate at the lower corner of the rectangle.</param>
    /// <param name="uv1">The UV coordinate at the higher corner of the rectangle.</param>
    protected void ClipRectangle(ref RectangleF rectangle, ref (Single x, Single y) uv0, ref (Single x, Single y) uv1)
    {
        if (!IsClippingEnabled) return;

        if (rectangle.Y < Clip.Y)
        {
            Single oldHeight = rectangle.Height;
            Single delta = Clip.Y - rectangle.Y;
            rectangle.Y = Clip.Y;
            rectangle.Height -= delta;

            if (rectangle.Height <= 0)
            {
                rectangle = RectangleF.Empty;
                
                return;
            }

            Single dv = delta / oldHeight;
            uv0.y += dv * (uv1.y - uv0.y);
        }

        if (rectangle.Y + rectangle.Height > Clip.Y + Clip.Height)
        {
            Single oldHeight = rectangle.Height;
            Single delta = rectangle.Y + rectangle.Height - (Clip.Y + Clip.Height);

            rectangle.Height -= delta;

            if (rectangle.Height <= 0)
            {
                rectangle = RectangleF.Empty;
                
                return;
            }

            Single dv = delta / oldHeight;
            uv1.y -= dv * (uv1.y - uv0.y);
        }

        if (rectangle.X < Clip.X)
        {
            Single oldWidth = rectangle.Width;
            Single delta = Clip.X - rectangle.X;
            rectangle.X = Clip.X;
            rectangle.Width -= delta;

            if (rectangle.Width <= 0)
            {
                rectangle = RectangleF.Empty;
                
                return;
            }

            Single du = delta / oldWidth;
            uv0.x += du * (uv1.x - uv0.x);
        }

        if (rectangle.X + rectangle.Width > Clip.X + Clip.Width)
        {
            Single oldWidth = rectangle.Width;
            Single delta = rectangle.X + rectangle.Width - (Clip.X + Clip.Width);

            rectangle.Width -= delta;

            if (rectangle.Width <= 0)
            {
                rectangle = RectangleF.Empty;
                
                return;
            }

            Single du = delta / oldWidth;
            uv1.x -= du * (uv1.x - uv0.x);
        }
    }
}
