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
    /// <returns>>The created binding.</returns>
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
    /// <returns>>The created binding.</returns>
    public static Binding<T> Computed<T>(Func<T> getter)
    {
        return new Binding<T>(getter, setter: null, []);
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
}

/// <summary>
/// Binds a property to a slot.
/// </summary>
/// <typeparam name="T">The type of value stored in the binding.</typeparam>
public class Binding<T> : IValueSource<T>
{
    private readonly Func<T> getter;
    private readonly Action<T>? setter;
    private readonly IValueSource[] dependencies;
    
    internal Binding(Func<T> getter, Action<T>? setter, IValueSource[] dependencies)
    {
        this.getter = getter;
        this.setter = setter;
        this.dependencies = dependencies;
        
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
