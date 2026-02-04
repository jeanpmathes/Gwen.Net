namespace Gwen.Net.New.Bindings;

/// <summary>
/// A slot stores a value that can be set or retrieved, as well as subscribed to for changes.
/// </summary>
/// <typeparam name="T">The type of value stored in the slot.</typeparam>
public class Slot<T> : ReadOnlySlot<T>
{
    /// <inheritdoc/>
    public Slot(T value) : base(value) {}
    
    /// <inheritdoc cref="ReadOnlySlot{T}.SetValue"/>
    public new void SetValue(T newValue)
    {
        base.SetValue(newValue);
    }
}
