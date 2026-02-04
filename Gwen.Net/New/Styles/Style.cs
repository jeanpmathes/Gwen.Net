using System;

namespace Gwen.Net.New.Styles;

/// <summary>
/// Utility class for styles.
/// </summary>
public static class Style 
{
    /// <summary>
    /// Create a style for a specific element type.
    /// </summary>
    /// <param name="apply">The action to apply the style to an element.</param>
    /// <typeparam name="T">The element type this style is for.</typeparam>
    /// <returns>>A new instance of the <see cref="Style{T}"/> class.</returns>
    public static Style<T> Create<T>(Action<T> apply) where T : Element
    {
        return new Style<T>(apply);
    }
}

/// <summary>
/// A style for a specific element type.
/// Styles can be used to set default values for properties of element.
/// </summary>
/// <typeparam name="T">The element type this style is for.</typeparam>
public class Style<T> where T : Element
{
    private readonly Action<T> apply;

    /// <summary>
    /// Creates a new instance of the <see cref="Style{T}"/> class.
    /// </summary>
    /// <param name="apply">The action to apply the style to an element.</param>
    public Style(Action<T> apply)
    {
        this.apply = apply;
    }
}
