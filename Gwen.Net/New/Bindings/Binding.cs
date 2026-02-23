using System;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// Utility class for defining bindings.
/// </summary>
public static class Binding
{
    /// <summary>
    /// Create a read-only binding that binds to a constant value.
    /// </summary>
    /// <param name="value">The constant value.</param>
    /// <typeparam name="T">The type of value stored in the binding.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<T> Constant<T>(T value)
    {
        return new Binding<T>(() => value, setter: null, []);
    }
    
    /// <summary>
    /// Create a read-only computed binding. Note that using other value sources inside the getter will not automatically
    /// trigger change notifications.
    /// </summary>
    /// <param name="getter">The getter function.</param>
    /// <typeparam name="T">The type of value stored in the binding.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<T> Computed<T>(Func<T> getter)
    {
        return new Binding<T>(getter, setter: null, []);
    }

    /// <summary>
    /// Bind directly to a value source.
    /// </summary>
    /// <param name="source">The value source.</param>
    /// <typeparam name="T">The type of value stored in the binding.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<T> Direct<T>(IValueSource<T> source)
    {
        return new Binding<T>(source.GetValue, setter: null, [source]);
    }
    
    /// <summary>
    /// Transforms a value source into another value source using the provided transformer function.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <param name="transformer">The transformer function.</param>
    /// <typeparam name="TSource">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<TResult> Transform<TSource, TResult>(IValueSource<TSource> source, Func<TSource, TResult> transformer)
    {
        return new Binding<TResult>(() => transformer(source.GetValue()), setter: null, [source]);
    }
    
    /// <summary>
    /// Transforms two value sources into another value source using the provided transformer function.
    /// </summary>
    /// <param name="source1">The first source value.</param>
    /// <param name="source2">The second source value.</param>
    /// <param name="transformer">The transformer function.</param>
    /// <typeparam name="TSource1">The type of the first source value.</typeparam>
    /// <typeparam name="TSource2">The type of the second source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<TResult> Transform<TSource1, TSource2, TResult>(IValueSource<TSource1> source1, IValueSource<TSource2> source2, Func<TSource1, TSource2, TResult> transformer)
    {
        return new Binding<TResult>(() => transformer(source1.GetValue(), source2.GetValue()), setter: null, [source1, source2]);
    }

    /// <summary>
    /// Transforms a value from one source type to a new type using the specified transformation function.
    /// </summary>
    /// <param name="source1">The first source value.</param>
    /// <param name="source2">The second source value.</param>
    /// <param name="source3">The third source value.</param>
    /// <param name="transformer">The transformer function.</param>
    /// <typeparam name="TSource1">The type of the first source value.</typeparam>
    /// <typeparam name="TSource2">The type of the second source value.</typeparam>
    /// <typeparam name="TSource3">The type of the third source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<TResult> Transform<TSource1, TSource2, TSource3, TResult>(IValueSource<TSource1> source1, IValueSource<TSource2> source2, IValueSource<TSource3> source3, Func<TSource1, TSource2, TSource3, TResult> transformer)
    {
        return new Binding<TResult>(() => transformer(source1.GetValue(), source2.GetValue(), source3.GetValue()), setter: null, [source1, source2, source3]);
    }

    /// <summary>
    /// Like <see cref="Transform{TSource,TResult}"/>, but the selector returns an <see cref="IValueSource{T}"/>
    /// instead of a plain value. The binding dynamically subscribes to whichever inner source the selector
    /// currently points to, switching subscriptions whenever the outer source changes.
    /// This makes it composable: the inner source may itself be a <see cref="Binding{T}"/>, a
    /// <see cref="Property{T}"/>, or another <c>FlatTransform</c>.
    /// </summary>
    /// <param name="source">The outer value source to observe.</param>
    /// <param name="selector">A function that selects an inner value source from the current outer value.</param>
    /// <typeparam name="TSource">The type of the outer source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<TResult> FlatTransform<TSource, TResult>(
        IValueSource<TSource> source,
        Func<TSource, IValueSource<TResult>> selector)
    {
        FlatTransformer<TResult> relay = new();

        source.ValueChanged += (_, _) => relay.SetInner(selector(source.GetValue()));
        relay.SetInner(selector(source.GetValue()));

        return new Binding<TResult>(() => relay.Inner!.GetValue(), setter: null, [relay]);
    }

    /// <summary>
    /// Variant of the non-nullable overload where the selector may return <c>null</c>,
    /// in which case <paramref name="defaultValue"/> is used.
    /// </summary>
    /// <param name="source">The outer value source to observe.</param>
    /// <param name="selector">
    ///     A function that selects an inner value source from the current outer value.
    ///     May return <c>null</c>, in which case <paramref name="defaultValue"/> is used.
    /// </param>
    /// <param name="defaultValue">The value to return when the selected inner source is <c>null</c>.</param>
    /// <typeparam name="TSource">The type of the outer source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<TResult> FlatTransform<TSource, TResult>(
        IValueSource<TSource> source,
        Func<TSource, IValueSource<TResult>?> selector,
        TResult defaultValue)
    {
        FlatTransformer<TResult> relay = new();

        source.ValueChanged += (_, _) => relay.SetInner(selector(source.GetValue()));
        relay.SetInner(selector(source.GetValue()));

        return new Binding<TResult>(() => relay.Inner != null ? relay.Inner.GetValue() : defaultValue, setter: null, [relay]);
    }
}

/// <summary>
/// Internal relay used by <c>Binding.FlatTransform</c> to manage a dynamic inner subscription
/// that switches whenever the outer source value changes.
/// </summary>
internal sealed class FlatTransformer<T> : IValueSource<T>
{
    internal IValueSource<T>? Inner { get; private set; }

    internal void SetInner(IValueSource<T>? newInner)
    {
        if (ReferenceEquals(Inner, newInner)) return;

        if (Inner != null)
            Inner.ValueChanged -= OnInnerValueChanged;

        Inner = newInner;

        if (Inner != null)
            Inner.ValueChanged += OnInnerValueChanged;

        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnInnerValueChanged(Object? sender, EventArgs e)
    {
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    T IValueSource<T>.GetValue() => Inner != null ? Inner.GetValue() : default!;

    public event EventHandler? ValueChanged;
}

/// <summary>
/// Binds a property to a slot.
/// </summary>
/// <typeparam name="T">The type of value stored in the binding.</typeparam>
public class Binding<T> : IValueSource<T>
{
    private readonly Func<T> getter;
    private readonly Action<T>? setter;

    internal Binding(Func<T> getter, Action<T>? setter, IValueSource[] dependencies)
    {
        this.getter = getter;
        this.setter = setter;

        foreach (IValueSource dependency in dependencies)
            dependency.ValueChanged += OnDependencyValueChanged;
    }

    private void OnDependencyValueChanged(Object? sender, EventArgs e)
    {
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public T GetValue()
    {
        return getter();
    }
    
    /// <inheritdoc/>
    public event EventHandler? ValueChanged;
}
