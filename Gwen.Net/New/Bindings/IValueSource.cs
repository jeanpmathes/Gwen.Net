using System;

namespace Gwen.Net.New.Bindings;

/// <summary>
///     Represents a source of values that can notify listeners when the value changes.
/// </summary>
public interface IValueSource
{
    /// <summary>
    ///     The event that is raised when the value changes.
    /// </summary>
    public event EventHandler? ValueChanged;
}

/// <summary>
///     Represents a source of values of type T.
/// </summary>
/// <typeparam name="T">The type of the value provided by this source.</typeparam>
public interface IValueSource<out T> : IValueSource
{
    /// <summary>
    ///     Gets the current value.
    /// </summary>
    /// <returns>The current value.</returns>
    public T GetValue();
}

/// <summary>
///     Represents a parametrized source of values, where the value can depend on an input of type TIn. The output value is
///     of type TOut.
/// </summary>
/// <typeparam name="TIn">The type of the input parameter that the value depends on.</typeparam>
/// <typeparam name="TOut">The type of the value provided by this source.</typeparam>
public interface IValueSource<in TIn, out TOut> : IValueSource
{
    /// <summary>
    ///     Gets the current value for the given input.
    /// </summary>
    /// <param name="input">The input parameter that the value depends on.</param>
    /// <returns>The current value for the given input.</returns>
    public TOut GetValue(TIn input);
}
