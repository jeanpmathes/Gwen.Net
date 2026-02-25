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
    public static Binding<T> To<T>(IValueSource<T> source)
    {
        return new Binding<T>(source.GetValue, setter: null, [source]);
    }
    
    /// <summary>
    /// Bind directly to two value sources, creating a tuple of their values.
    /// </summary>
    /// <param name="source1">The first value source.</param>
    /// <param name="source2">The second value source.</param>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<(T1, T2)> To<T1, T2>(IValueSource<T1> source1, IValueSource<T2> source2)
    {
        return new Binding<(T1, T2)>(() => (source1.GetValue(), source2.GetValue()), setter: null, [source1, source2]);
    }
    
    /// <summary>
    /// Bind directly to three value sources, creating a tuple of their values.
    /// </summary>
    /// <param name="source1">The first value source.</param>
    /// <param name="source2">The second value source.</param>
    /// <param name="source3">The third value source.</param>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <returns>The created binding.</returns>
    public static Binding<(T1, T2, T3)> To<T1, T2, T3>(IValueSource<T1> source1, IValueSource<T2> source2, IValueSource<T3> source3)
    {
        return new Binding<(T1, T2, T3)>(() => (source1.GetValue(), source2.GetValue(), source3.GetValue()), setter: null, [source1, source2, source3]);
    }
    
    extension<T1, T2>(Binding<(T1, T2)> binding)
    {
        /// <summary>
        /// Deconstructing overload of <see cref="Binding{T}.Select{TSelected}(Func{T,IValueSource{TSelected}})"/> for two-source tuple bindings.
        /// </summary>
        public Binding<TResult> Select<TResult>(Func<T1, T2, IValueSource<TResult>> selector)
        {
            return binding.Select(t => selector(t.Item1, t.Item2));
        }

        /// <summary>
        /// Deconstructing overload of <see cref="Binding{T}.Select{TSelected}(Func{T,IValueSource{TSelected}?},TSelected)"/> for two-source tuple bindings.
        /// </summary>
        public Binding<TResult> Select<TResult>(Func<T1, T2, IValueSource<TResult>?> selector,
            TResult defaultValue)
        {
            return binding.Select(t => selector(t.Item1, t.Item2), defaultValue);
        }

        /// <summary>
        /// Deconstructing overload of <see cref="Binding{T}.Compute{TSelected}"/> for two-source tuple bindings.
        /// </summary>
        public Binding<TResult> Compute<TResult>(Func<T1, T2, TResult> computer)
        {
            return binding.Compute(t => computer(t.Item1, t.Item2));
        }
    }

    extension<T1, T2, T3>(Binding<(T1, T2, T3)> binding)
    {
        /// <summary>
        /// Deconstructing overload of <see cref="Binding{T}.Select{TSelected}(Func{T,IValueSource{TSelected}})"/> for three-source tuple bindings.
        /// </summary>
        public Binding<TResult> Select<TResult>(Func<T1, T2, T3, IValueSource<TResult>> selector)
        {
            return binding.Select(t => selector(t.Item1, t.Item2, t.Item3));
        }

        /// <summary>
        /// Deconstructing overload of <see cref="Binding{T}.Select{TSelected}(Func{T,IValueSource{TSelected}?},TSelected)"/> for three-source tuple bindings.
        /// </summary>
        public Binding<TResult> Select<TResult>(Func<T1, T2, T3, IValueSource<TResult>?> selector,
            TResult defaultValue)
        {
            return binding.Select(t => selector(t.Item1, t.Item2, t.Item3), defaultValue);
        }

        /// <summary>
        /// Deconstructing overload of <see cref="Binding{T}.Compute{TSelected}"/> for three-source tuple bindings.
        /// </summary>
        public Binding<TResult> Compute<TResult>(Func<T1, T2, T3, TResult> computer)
        {
            return binding.Compute(t => computer(t.Item1, t.Item2, t.Item3));
        }
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
    
    /// <summary>
    /// Create a new binding that selects a value from this binding's value using the provided selector function.
    /// This will correctly handle subscription changes if the selector returns a different inner source when the outer value changes.
    /// </summary>
    /// <param name="selector">
    /// A function that selects an inner value source from the current value of this binding.
    /// Must not contain any calls to <see cref="IValueSource{T}.GetValue"/>.
    /// </param>
    /// <typeparam name="TSelected">The type of the selected value.</typeparam>
    /// <returns>>The created binding.</returns>
    public Binding<TSelected> Select<TSelected>(Func<T, IValueSource<TSelected>> selector)
    {
        BindingRelay<TSelected> relay = new();
        
        ValueChanged += (_, _) => relay.SetInner(selector(GetValue()));
        relay.SetInner(selector(GetValue()));
        
        return new Binding<TSelected>(() => relay.Inner!.GetValue(), setter: null, [relay]);
    }

    /// <summary>
    /// Contrary to the non-nullable overload, this variant allows the selector to return <c>null</c>.
    /// It creates a new binding that selects a value from this binding's value using the provided selector function,
    /// returning <paramref name="defaultValue"/> when the selector returns <c>null</c>.
    /// </summary>
    /// <param name="selector">
    /// A function that selects an inner value source from the current value of this binding.
    /// May return <c>null</c>, in which case <paramref name="defaultValue"/> is used.
    /// Must not contain any calls to <see cref="IValueSource{T}.GetValue"/>.
    /// </param>
    /// <param name="defaultValue">The value to return when the selected inner source is <c>null</c>.</param>
    /// <typeparam name="TSelected">The type of the selected value.</typeparam>
    /// <returns>The created binding.</returns>
    public Binding<TSelected> Select<TSelected>(Func<T, IValueSource<TSelected>?> selector, TSelected defaultValue)
    {
        BindingRelay<TSelected> relay = new();

        ValueChanged += (_, _) => relay.SetInner(selector(GetValue()));
        relay.SetInner(selector(GetValue()));

        return new Binding<TSelected>(() => relay.Inner != null ? relay.Inner.GetValue() : defaultValue, setter: null, [relay]);
    }

    /// <summary>
    /// Create a new binding that computes a value from this binding's value using the provided selector function.
    /// Note that using other value sources inside the selector will not automatically trigger change notifications.
    /// </summary>
    /// <param name="computer">
    /// A function that computes a value from the current value of this binding.
    /// Must not contain any calls to <see cref="IValueSource{T}.GetValue"/>.
    /// </param>
    /// <typeparam name="TSelected">The type of the selected value.</typeparam>
    /// <returns>The created binding.</returns>
    public Binding<TSelected> Compute<TSelected>(Func<T, TSelected> computer)
    {
        return new Binding<TSelected>(() => computer(GetValue()), setter: null, [this]);
    }
    
    /// <summary>
    /// Create a new binding that combines this binding with another value source, creating a tuple of their values.
    /// </summary>
    /// <param name="other">The other value source to combine with this binding.</param>
    /// <typeparam name="TSelected">The type of the value in the other source.</typeparam>
    /// <returns>The created binding.</returns>
    public Binding<(T, TSelected)> Combine<TSelected>(IValueSource<TSelected> other)
    {
        return new Binding<(T, TSelected)>(() => (GetValue(), other.GetValue()), setter: null, [this, other]);
    }
}

/// <summary>
/// Internal relay used to manage a dynamic inner subscription that switches whenever the outer source value changes.
/// </summary>
internal sealed class BindingRelay<T> : IValueSource<T>
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
