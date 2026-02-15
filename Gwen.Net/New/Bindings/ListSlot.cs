using System;
using System.Collections.Generic;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// A list-valued slot, which can be modified.
/// </summary>
/// <typeparam name="TItem">The item type.</typeparam>
public sealed class ListSlot<TItem> : ReadOnlyListSlot<TItem>, IList<TItem>
{
    /// <inheritdoc/>
    public new void Add(TItem item) => base.Add(item);
    
    /// <inheritdoc/>
    public new void Clear() => base.Clear();
    
    /// <inheritdoc/>
    public new Boolean Remove(TItem item) => base.Remove(item);

    Int32 ICollection<TItem>.Count => Count.GetValue();
    
    /// <inheritdoc/>
    public Boolean IsReadOnly => false;
    
    /// <inheritdoc/>
    public new void Insert(Int32 index, TItem item) => base.Insert(index, item);
    
    /// <inheritdoc/>
    public new void RemoveAt(Int32 index) => base.RemoveAt(index);

    /// <inheritdoc/>
    public new TItem this[Int32 index]
    {
        get => base[index];
        set => base[index] = value;
    }
    
    /// <summary>
    /// Move an item from one index to another, shifting other items as necessary.
    /// </summary>
    /// <param name="oldIndex">The index of the item to move.</param>
    /// <param name="newIndex">The index to move the item to.</param>
    public new void Move(Int32 oldIndex, Int32 newIndex) => base.Move(oldIndex, newIndex);
    
    /// <summary>
    /// Sort the items in the list using the specified comparison function.
    /// </summary>
    /// <param name="comparison">The comparison function to use for sorting.</param>
    public new void Sort(Comparison<TItem> comparison) => base.Sort(comparison);
}
