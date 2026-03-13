using System;
using System.Collections.Generic;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls;

namespace Gwen.Net.New.Styles;

/// <summary>
///     Static helpers for styles.
/// </summary>
public static class Styling
{
    /// <summary>
    ///     Create a style for a specific element type.
    /// </summary>
    /// <param name="build">
    ///     The action to build the style. The builder is passed as a parameter to the action and can be used
    ///     to set properties for the style.
    /// </param>
    /// <typeparam name="T">The element type this style is for.</typeparam>
    /// <returns>A new instance of the <see cref="Style{T}" /> class.</returns>
    public static Style<T> Create<T>(Action<IBuilder<T>> build) where T : IControl
    {
        Builder<T> builder = new();

        build(builder);

        return builder;
    }
    // todo: add two kinds of style inheritance - call them BasedOn, and BasedOnContext
    // todo: first kind is using a directly given style which is just applied first
    // todo: second kind is just setting a flag in the style, then on apply it is given the context, takes style for same type from context (or if that is the same style then from parent context) and uses that first, same on clear

    /// <summary>
    ///     The builder interface for styles. This is used to build styles in a fluent way.
    /// </summary>
    /// <typeparam name="TControl">The control type this style is for.</typeparam>
    public interface IBuilder<out TControl> : ITriggerBuilder<TControl> where TControl : IControl
    {
        /// <summary>
        ///     Set a property value for a style.
        /// </summary>
        /// <param name="property">The property the style should set.</param>
        /// <param name="value">The value the style should set for the property.</param>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public IBuilder<TControl> Set<TValue>(Func<TControl, Property<TValue>> property, TValue value);

        /// <summary>
        ///     Set a property binding for a style.
        /// </summary>
        /// <param name="property">The property the style should set.</param>
        /// <param name="binding">The binding the style should set for the property.</param>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <returns>The builder instance for chaining.</returns>
        public IBuilder<TControl> Set<TValue>(Func<TControl, Property<TValue>> property, Binding<TValue> binding);
    }

    /// <summary>
    /// Variant of <see cref="IBuilder{TControl}"/> which can only set triggers.
    /// </summary>
    /// <typeparam name="TControl">The control type this style is for.</typeparam>
    public interface ITriggerBuilder<out TControl> where TControl : IControl
    {
        /// <summary>
        ///     Set a trigger for a style. This creates a property setter which uses the passed value only if a condition is met.
        ///     Otherwise, the previously applied value is used.
        /// </summary>
        /// <remarks>
        ///     Triggers are always applied after all setters, no matter the order of declaration.
        /// </remarks>
        /// <param name="condition">The condition which determines whether the style value or the previously applied value is used.</param>
        /// <param name="property">The property the style should set.</param>
        /// <param name="value">The value the style should set for the property if the condition is met.</param>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <returns>>The builder instance for chaining.</returns>
        public ITriggerBuilder<TControl> Trigger<TValue>(Func<TControl, IValueSource<Boolean>> condition, Func<TControl, Property<TValue>> property, TValue value);
    }

    private class Builder<TControl> : IBuilder<TControl> where TControl : IControl
    {
        private readonly List<(Func<TControl, Property>, Object)> setters = [];
        private readonly List<(Func<TControl, Property>, Func<TControl, Object>)> triggers = [];

        /// <inheritdoc />
        public IBuilder<TControl> Set<TValue>(Func<TControl, Property<TValue>> property, TValue value)
        {
            setters.Add((property, value)!);

            return this;
        }

        /// <inheritdoc/>
        public IBuilder<TControl> Set<TValue>(Func<TControl, Property<TValue>> property, Binding<TValue> binding)
        {
            setters.Add((property, binding));

            return this;
        }

        /// <inheritdoc/>
        public ITriggerBuilder<TControl> Trigger<TValue>(Func<TControl, IValueSource<Boolean>> condition, Func<TControl, Property<TValue>> property, TValue value)
        {
            triggers.Add((property, control =>
            {
                return Binding.To(condition(control)).Parametrize<TValue>((original, isConditionMet) => isConditionMet ? value : original);
            }));

            return this;
        }

        /// <summary>
        ///     Convert a builder to a concrete <see cref="Style{T}" /> instance.
        /// </summary>
        /// <param name="builder">The builder containing style assignments.</param>
        public static implicit operator Style<TControl>(Builder<TControl> builder)
        {
            return new Style<TControl>(builder.setters, builder.triggers);
        }
    }
}

/// <summary>
///     Non-generic abstract base class for styles.
/// </summary>
public abstract class Style
{
    /// <summary>
    ///     Get the type of element this style is for.
    /// </summary>
    public abstract Type StyledType { get; }
}

/// <summary>
///     Interface for styles, allowing to apply and clear styles from elements.
/// </summary>
/// <typeparam name="T">The element type this style is for.</typeparam>
public interface IStyle<in T> where T : IControl
{
    /// <summary>
    ///     Apply the style to an element.
    /// </summary>
    /// <param name="element">The element to apply the style to.</param>
    public void Apply(T element);

    /// <summary>
    ///     Clear the style from an element.
    /// </summary>
    /// <param name="element">The element to clear the style from.</param>
    public void Clear(T element);
}

/// <summary>
///     A style for a specific element type.
///     Styles can be used to set default values for properties of element.
/// </summary>
/// <typeparam name="T">The element type this style is for.</typeparam>
public class Style<T> : Style, IStyle<T> where T : IControl
{
    private readonly (Func<T, Property>, Object)[] setters;
    private readonly (Func<T, Property>, Func<T, Object>)[] triggers;

    /// <summary>
    ///     Creates a new instance of the <see cref="Style{T}" /> class.
    /// </summary>
    /// <param name="setters">The setters of the style.</param>
    /// <param name="triggers">The triggers of the style.</param>
    public Style(List<(Func<T, Property>, Object)> setters, List<(Func<T, Property>, Func<T, Object>)> triggers)
    {
        this.setters = setters.ToArray();
        this.triggers = triggers.ToArray();
    }

    /// <inheritdoc />
    public override Type StyledType => typeof(T);

    /// <summary>
    ///     Apply the style to an element.
    /// </summary>
    /// <param name="element">The element to apply the style to.</param>
    public void Apply(T element)
    {
        foreach ((Func<T, Property> getProperty, Object value) in setters)
        {
            Property property = getProperty(element);
            property.Style(value);
        }

        foreach ((Func<T, Property> getProperty, Func<T, Object> getValue) in triggers)
        {
            Property property = getProperty(element);
            property.Style(getValue(element));
        }
    }

    /// <summary>
    ///     Clear the style from an element.
    /// </summary>
    /// <param name="element">The element to clear the style from.</param>
    public void Clear(T element)
    {
        foreach ((Func<T, Property> getProperty, _) in setters)
        {
            Property property = getProperty(element);
            property.ClearStyle();
        }

        foreach ((Func<T, Property> getProperty, _) in triggers)
        {
            Property property = getProperty(element);
            property.ClearStyle();
        }
    }
}
