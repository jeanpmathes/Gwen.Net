using System;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// Represents a source of values that can notify listeners when the value changes.
/// </summary>
public interface IValueSource
{
    /// <summary>
    /// The event that is raised when the value changes.
    /// </summary>
    public event EventHandler? ValueChanged;
}

/// <summary>
/// Represents a source of values of type T.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IValueSource<out T> : IValueSource
{
    /// <summary>
    /// Gets the current value.
    /// </summary>
    /// <returns>The current value.</returns>
    public T GetValue();
}
