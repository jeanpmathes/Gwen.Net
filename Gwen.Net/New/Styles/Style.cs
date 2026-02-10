using System;
using System.Collections.Generic;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls;

namespace Gwen.Net.New.Styles;

/// <summary>
/// Static helpers for styles.
/// </summary>
public static class Styling
{
    // todo: add two kinds of style inheritance - call them BasedOn, and BasedOnContext
    // todo: first kind is using a directly given style which is just applied first
    // todo: second kind is just setting a flag in the style, then on apply it is given the context, takes style for same type from context (or if that is the same style then from parent context) and uses that first, same on clear
    
    /// <summary>
    /// The builder interface for styles. This is used to build styles in a fluent way.
    /// </summary>
    /// <typeparam name="TElement">The element type this style is for.</typeparam>
    public interface IBuilder<out TElement> where TElement : Control
    {
        /// <summary>
        /// Set a property value for a style.
        /// </summary>
        /// <param name="property">The property the style should set.</param>
        /// <param name="value">The value the style should set for the property.</param>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public IBuilder<TElement> Set<TValue>(Func<TElement, Property<TValue>> property, TValue value);
    }
    
    private class Builder<TControl> : IBuilder<TControl> where TControl : Control
    {
        private readonly List<(Func<TControl, Property>, Object)> properties = [];

        /// <summary>
        /// Add a property assignment to the style being built.
        /// </summary>
        /// <param name="property">Selector for the control property to set.</param>
        /// <param name="value">Value to assign when the style is applied.</param>
        /// <typeparam name="TValue">The property value type.</typeparam>
        /// <returns>The same builder instance for fluent chaining.</returns>
        public IBuilder<TControl> Set<TValue>(Func<TControl, Property<TValue>> property, TValue value)
        {
            properties.Add((property, value)!);
            
            return this;
        }

        /// <summary>
        /// Convert a builder to a concrete <see cref="Style{T}"/> instance.
        /// </summary>
        /// <param name="builder">The builder containing style assignments.</param>
        public static implicit operator Style<TControl>(Builder<TControl> builder)
        {
            return new Style<TControl>(builder.properties);
        }
    }
    
    /// <summary>
    /// Create a style for a specific element type.
    /// </summary>
    /// <param name="build">The action to build the style. The builder is passed as a parameter to the action and can be used to set properties for the style.</param>
    /// <typeparam name="T">The element type this style is for.</typeparam>
    /// <returns>A new instance of the <see cref="Style{T}"/> class.</returns>
    public static Style<T> Create<T>(Action<IBuilder<T>> build) where T : Control
    {
        Builder<T> builder = new();
        
        build(builder);
        
        return builder;
    }
}

/// <summary>
/// Non-generic abstract base class for styles.
/// </summary>
public abstract class Style
{
    /// <summary>
    /// Get the type of element this style is for.
    /// </summary>
    public abstract Type StyledType { get; }
}

/// <summary>
/// Interface for styles, allowing to apply and clear styles from elements.
/// </summary>
/// <typeparam name="T">The element type this style is for.</typeparam>
public interface IStyle<in T> where T : Control
{
    /// <summary>
    /// Apply the style to an element.
    /// </summary>
    /// <param name="element">The element to apply the style to.</param>
    public void Apply(T element);
    
    /// <summary>
    /// Clear the style from an element.
    /// </summary>
    /// <param name="element">The element to clear the style from.</param>
    public void Clear(T element);
}

/// <summary>
/// A style for a specific element type.
/// Styles can be used to set default values for properties of element.
/// </summary>
/// <typeparam name="T">The element type this style is for.</typeparam>
public class Style<T> : Style, IStyle<T> where T : Control
{
    private readonly (Func<T, Property>, Object)[] properties;
    
    /// <inheritdoc/>
    public override Type StyledType => typeof(T);
    
    /// <summary>
    /// Creates a new instance of the <see cref="Style{T}"/> class.
    /// </summary>
    /// <param name="properties">The properties to set for the style.</param>
    public Style(List<(Func<T, Property>, Object)> properties)
    {
        this.properties = properties.ToArray();
    }
    
    /// <summary>
    /// Apply the style to an element.
    /// </summary>
    /// <param name="element">The element to apply the style to.</param>
    public void Apply(T element)
    {
        foreach ((Func<T, Property> getProperty, Object value) in properties)
        {
            Property property = getProperty(element);
            property.Style(value);
        }
    }
    
    /// <summary>
    /// Clear the style from an element.
    /// </summary>
    /// <param name="element">The element to clear the style from.</param>
    public void Clear(T element)
    {
        foreach ((Func<T, Property> getProperty, _) in properties)
        {
            Property property = getProperty(element);
            property.ClearStyle();
        }
    }
}
