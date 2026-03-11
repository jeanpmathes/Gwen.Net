using System;

namespace Gwen.Net.New;

/// <summary>
///     The enablement of elements.
/// </summary>
public enum Enablement
{
    /// <summary>
    ///     The element is enabled and can receive input and focus.
    /// </summary>
    Enabled = 0,

    /// <summary>
    ///     The element is read-only. It can still receive focus and might react to input, but should not write to the model or
    ///     execute commands.
    ///     This state is necessary as for example creating a read-only view would not be possible with just disabled, as
    ///     disabling would also disable scroll controls and such.
    /// </summary>
    ReadOnly = 1,

    /// <summary>
    ///     The element is disabled. It cannot receive input or focus.
    /// </summary>
    Disabled = 2
}

/// <summary>
///     Tools for working with enablement values.
/// </summary>
public static class Enablements
{
    /// <summary>
    ///     Get an enablement value from a boolean. Enabled if <c>true</c>, disabled if <c>false</c>.
    /// </summary>
    /// <param name="enabled">Whether the control should be enabled.</param>
    /// <returns>>An enablement value corresponding to the given boolean.</returns>
    public static Enablement FromBoolean(Boolean enabled)
    {
        return enabled ? Enablement.Enabled : Enablement.Disabled;
    }

    /// <summary>
    ///     Get the lower of two enablement values. The lower enablement is the one that is less enabled.
    /// </summary>
    public static Enablement Lower(Enablement a, Enablement b)
    {
        return (Enablement) Math.Max((Int32) a, (Int32) b);
    }

    extension(Enablement enablement)
    {
        /// <summary>
        ///     Whether the element is enabled, which means it can write to the model and execute commands.
        /// </summary>
        public Boolean IsEnabled => enablement == Enablement.Enabled;

        /// <summary>
        ///     Whether the element can receive focus.
        /// </summary>
        public Boolean IsFocusable => enablement != Enablement.Disabled;

        /// <summary>
        ///     Whether the element can receive input events.
        /// </summary>
        public Boolean CanReceiveInput => enablement != Enablement.Disabled;
    }
}
