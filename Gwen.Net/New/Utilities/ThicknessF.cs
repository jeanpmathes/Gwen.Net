using System;
using System.Drawing;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// Describes the thickness of a frame around a rectangle.
/// Uses include margins, padding, and borders.
/// </summary>
public readonly struct ThicknessF : IEquatable<ThicknessF>
{
    /// <summary>
    /// Get a thickness with all sides set to zero.
    /// </summary>
    public static ThicknessF Zero { get; } = new(0);
    
    /// <summary>
    /// Get a thickness with all sides set to one.
    /// </summary>
    public static ThicknessF One { get; } = new(1);
    
    /// <summary>
    /// Create a thickness with the specified left, top, right, and bottom thicknesses.
    /// </summary>
    public ThicknessF(Single left, Single top, Single right, Single bottom) 
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }
    
    /// <summary>
    /// Create a thickness with the specified uniform thickness for all sides.
    /// </summary>
    public ThicknessF(Single uniform) : this(uniform, uniform, uniform, uniform)
    {
    }
    
    /// <summary>
    /// The left thickness.
    /// </summary>
    public Single Left { get; init; }
    
    /// <summary>
    /// The top thickness.
    /// </summary>
    public Single Top { get; init; }
    
    /// <summary>
    /// The right thickness.
    /// </summary>
    public Single Right { get; init; }
    
    /// <summary>
    /// The bottom thickness.
    /// </summary>
    public Single Bottom { get; init; }

    /// <summary>
    /// The total width of the thickness, which is the sum of the left and right thicknesses.
    /// </summary>
    public Single Width => Left + Right;
    
    /// <summary>
    /// The total height of the thickness, which is the sum of the top and bottom thicknesses.
    /// </summary>
    public Single Height => Top + Bottom;
    
    /// <summary>
    /// Add a thickness to a size, resulting in a new size that is increased by the thickness on all sides.
    /// </summary>
    public static SizeF operator +(SizeF size, ThicknessF thickness)
    {
        return new SizeF(size.Width + thickness.Left + thickness.Right, size.Height + thickness.Top + thickness.Bottom);
    }
    
    /// <summary>
    /// Subtract a thickness from a size, resulting in a new size that is decreased by the thickness on all sides.
    /// </summary>
    public static SizeF operator -(SizeF size, ThicknessF thickness)
    {
        return new SizeF(size.Width - thickness.Left - thickness.Right, size.Height - thickness.Top - thickness.Bottom);
    }
    
    /// <summary>
    /// Add a thickness to a rectangle, resulting in a new rectangle that is increased by the thickness on all sides.
    /// </summary>
    public static RectangleF operator +(RectangleF rectangle, ThicknessF thickness)
    {
        return new RectangleF(rectangle.X - thickness.Left, rectangle.Y - thickness.Top, rectangle.Width + thickness.Left + thickness.Right, rectangle.Height + thickness.Top + thickness.Bottom);
    }
    
    /// <summary>
    /// Subtract a thickness from a rectangle, resulting in a new rectangle that is decreased by the thickness on all sides.
    /// </summary>
    public static RectangleF operator -(RectangleF rectangle, ThicknessF thickness)
    {
        return new RectangleF(rectangle.X + thickness.Left, rectangle.Y + thickness.Top, rectangle.Width - thickness.Left - thickness.Right, rectangle.Height - thickness.Top - thickness.Bottom);
    }

    #region EQUALITY
    
    /// <inheritdoc/>
    public Boolean Equals(ThicknessF other)
    {
        return Left.Equals(other.Left) && Top.Equals(other.Top) && Right.Equals(other.Right) && Bottom.Equals(other.Bottom);
    }

    /// <inheritdoc/>
    public override Boolean Equals(Object? obj)
    {
        return obj is ThicknessF other && Equals(other);
    }

    /// <inheritdoc/>
    public override Int32 GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    /// <summary>
    /// The equality operator.
    /// </summary>
    public static Boolean operator ==(ThicknessF left, ThicknessF right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// The inequality operator.
    /// </summary>
    public static Boolean operator !=(ThicknessF left, ThicknessF right)
    {
        return !left.Equals(right);
    }
    
    #endregion EQUALITY
    
    /// <inheritdoc/>
    public override String ToString()
    {
        return this == Zero 
            ? "ThicknessF.Zero" 
            : $"ThicknessF(Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom})";
    }
}
