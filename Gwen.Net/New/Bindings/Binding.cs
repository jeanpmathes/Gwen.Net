using System;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// Utility class for defining bindings.
/// </summary>
public static class Bindings
{
    /// <summary>
    /// Create a read-only binding that binds to a constant value.
    /// </summary>
    /// <param name="value">The constant value.</param>
    /// <typeparam name="T">The type of value stored in the binding.</typeparam>
    /// <returns>>The created binding.</returns>
    public static Binding<T> Constant<T>(T value)
    {
        return new Binding<T>(() => value, setter: null);
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
    
    internal Binding(Func<T> getter, Action<T>? setter)
    {
        this.getter = getter;
        this.setter = setter;
    }

    /// <inheritdoc/>
    public T GetValue()
    {
        return getter();
    }
    
    /// <inheritdoc/>
    public event EventHandler? ValueChanged;
}
