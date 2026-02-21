using System;

namespace Gwen.Net.New.Texts;

/// <summary>
/// The weight of a font.
/// </summary>
/// <param name="Value">The weight value, an integer between 1 and 999.</param>
public record struct Weight(Int16 Value = 400)
{
    /// <summary>
    /// Thin, with a value of <c>100</c>.
    /// </summary>
    public static readonly Weight Thin = new(100);
    
    /// <summary>
    /// ExtraLight, with a value of <c>200</c>.
    /// </summary>
    public static readonly Weight ExtraLight = new(200);
    
    /// <summary>
    /// Light, with a value of <c>300</c>.
    /// </summary>
    public static readonly Weight Light = new(300);
    
    /// <summary>
    /// SemiLight, with a value of <c>350</c>.
    /// </summary>
    public static readonly Weight SemiLight = new(350);
    
    /// <summary>
    /// Normal, with a value of <c>400</c>.
    /// </summary>
    public static readonly Weight Normal = new(400);
    
    /// <summary>
    /// Medium, with a value of <c>500</c>.
    /// </summary>
    public static readonly Weight Medium = new(500);
    
    /// <summary>
    /// SemiBold, with a value of <c>600</c>.
    /// </summary>
    public static readonly Weight SemiBold = new(600);
    
    /// <summary>
    /// Bold, with a value of <c>700</c>.
    /// </summary>
    public static readonly Weight Bold = new(700);
    
    /// <summary>
    /// ExtraBold, with a value of <c>800</c>.
    /// </summary>
    public static readonly Weight ExtraBold = new(800);
    
    /// <summary>
    /// Black, with a value of <c>900</c>.
    /// </summary>
    public static readonly Weight Black = new(900);
    
    /// <summary>
    /// ExtraBlack, with a value of <c>950</c>.
    /// </summary>
    public static readonly Weight ExtraBlack = new(950);
    
    /// <summary>
    /// Greater than operator, compares the weight values of two <see cref="Weight"/> instances.
    /// </summary>
    public static Boolean operator >(Weight left, Weight right) => left.Value > right.Value;
    
    /// <summary>
    /// Less than operator, compares the weight values of two <see cref="Weight"/> instances.
    /// </summary>
    public static Boolean operator <(Weight left, Weight right) => left.Value < right.Value;
    
    /// <summary>
    /// Greater than or equal operator, compares the weight values of two <see cref="Weight"/> instances.
    /// </summary>
    public static Boolean operator >=(Weight left, Weight right) => left.Value >= right.Value;
    
    /// <summary>
    /// Less than or equal operator, compares the weight values of two <see cref="Weight"/> instances.
    /// </summary>
    public static Boolean operator <=(Weight left, Weight right) => left.Value <= right.Value;
}
