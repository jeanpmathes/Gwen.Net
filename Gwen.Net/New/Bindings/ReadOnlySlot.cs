using System;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// A read-only slot stores a value that can be retrieved and subscribed to for changes, but not set externally.
/// </summary>
/// <typeparam name="T">The type of value stored in the slot.</typeparam>
public class ReadOnlySlot<T> : IValueSource<T> 
{
    // todo: maybe remove ReadOnlySlot and just use IValueSource<T> everywhere, move code down to Slot<T>
    // todo: maybe the same can be done for the list slot as well
    
    private T value;
    
    /// <summary>
    /// Creates a new instance of the <see cref="ReadOnlySlot{T}"/> class.
    /// </summary>
    /// <param name="value">The initial value of the slot.</param>
    public ReadOnlySlot(T value)
    {
        this.value = value;
    }
    
    /// <inheritdoc/>
    public T GetValue()
    {
        return value;
    }
    
    /// <summary>
    /// Set the value of the slot.
    /// </summary>
    /// <param name="newValue">The new value to set.</param>
    protected void SetValue(T newValue)
    {
        if (Equals(value, newValue))
            return;
        
        value = newValue;
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }
    
    /// <inheritdoc/>
    public event EventHandler? ValueChanged;

    /// <inheritdoc/>
    public override String ToString()
    {
        return $"{{{GetValue()?.ToString()}}}";
    }
}
