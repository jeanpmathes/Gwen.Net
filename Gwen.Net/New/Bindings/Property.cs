using System;
using Gwen.Net.New.Controls;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// Non-generic abstract base class for properties.
/// </summary>
public abstract class Property : IValueSource
{
    /// <summary>
    /// Create a new property with the given default binding.
    /// </summary>
    /// <param name="owner">The owner element of the property.</param>
    /// <param name="defaultBinding">The default binding for the property.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static Property<T> Create<T>(Control owner, Binding<T> defaultBinding)
    {
        return new Property<T>(owner, defaultBinding);
    }
    
    /// <summary>
    /// Create a new property with a constant default value.
    /// </summary>
    /// <param name="owner">The owner element of the property.</param>
    /// <param name="defaultValue">The default value for the property.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static Property<T> Create<T>(Control owner, T defaultValue)
    {
        return Create(owner, Binding.Constant(defaultValue));
    }

    /// <summary>
    /// Style the property with a value or binding.
    /// </summary>
    /// <param name="value">The value or binding, must fit the type of the property.</param>
    internal abstract void Style(Object value);
    
    /// <summary>
    /// Clear the styling of the property.
    /// </summary>
    internal abstract void ClearStyle();
    
    /// <summary>
    /// Notifies subscribers that the value of the property has changed.
    /// </summary>
    protected void NotifyValueChanged()
    {
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public event EventHandler? ValueChanged;
}

/// <summary>
/// A property is a value of a <see cref="Control"/> that can be set, styled and bound to a slot.
/// </summary>
/// <typeparam name="T">The type of value stored in the property.</typeparam>
public sealed class Property<T> : Property, IValueSource<T>
{
    private readonly Binding<T> defaultBinding;

    private Binding<T>? styleBinding;
    private Binding<T>? localBinding;

    private Binding<T> targetBinding;
    private Boolean isActive;
    
    private T? cachedValue;
    private Boolean isCacheValid;
    
    internal Property(Control owner, Binding<T> defaultBinding)
    {
        this.defaultBinding = defaultBinding;
        
        targetBinding = defaultBinding;
        
        owner.AttachedToRoot += (_, _) => Activate();
        owner.DetachedFromRoot += (_, _) => Deactivate();
    }
    
    /// <summary>
    /// Activates the property, causing it to subscribe to changes in its active binding.
    /// </summary>
    public void Activate()
    {
        if (isActive) return;
        isActive = true;
        
        AttachTargetBinding();
        UpdateCachedValue(notify: true);
    }
    
    /// <summary>
    /// Deactivates the property, causing it to unsubscribe from changes in its active binding.
    /// </summary>
    public void Deactivate()
    {
        if (!isActive) return;
        isActive = false;
        
        DetachTargetBinding();
    }

    private void RecomputeTargetBinding()
    {
        Binding<T> binding = localBinding ?? styleBinding ?? defaultBinding;
        
        if (ReferenceEquals(binding, targetBinding))
            return;

        if (isActive)
        {
            DetachTargetBinding();
        }
        
        targetBinding = binding;
        isCacheValid = false;

        if (isActive)
        {
            AttachTargetBinding();
            UpdateCachedValue(notify: true);
        }
    }
    
    private void AttachTargetBinding()
    {
        targetBinding.ValueChanged += OnTargetBindingValueChanged;
    }
    
    private void DetachTargetBinding()
    {
        targetBinding.ValueChanged -= OnTargetBindingValueChanged;
    }
    
    private void OnTargetBindingValueChanged(Object? sender, EventArgs e)
    {
        UpdateCachedValue(notify: true);
    }
    
    private void UpdateCachedValue(Boolean notify)
    {
        T value = targetBinding.GetValue();
        
        if (isCacheValid && Equals(cachedValue, value))
            return;
        
        cachedValue = value;
        isCacheValid = true;
        
        if (notify)
            NotifyValueChanged();
    }
    
    /// <inheritdoc/>
    public T GetValue()
    {
        if (!isActive)
            return targetBinding.GetValue();
        
        if (!isCacheValid)
            UpdateCachedValue(notify: false);
        
        return cachedValue!;
    }
    
    #region LOCAL
    
    private void SetLocal(Binding<T> newLocalBinding)
    {
        localBinding = newLocalBinding;
        RecomputeTargetBinding();
    }
    
    /// <summary>
    /// Binds the property to a constant value locally.
    /// </summary>
    public T Value
    {
        set => SetLocal(Bindings.Binding.Constant(value));
    }

    /// <summary>
    /// Binds the property locally.
    /// </summary>
    public Binding<T> Binding
    {
        set => SetLocal(value);
    }
    
    #endregion LOCAL

    #region STYLE

    private void SetStyle(Binding<T>? newStyleBinding)
    {
        styleBinding = newStyleBinding;
        RecomputeTargetBinding();
    }
    
    /// <summary>
    /// Style the property with a constant value.
    /// </summary>
    /// <param name="value">The constant value.</param>
    public void Style(T value)
    {
        SetStyle(Bindings.Binding.Constant(value));
    }
    
    /// <summary>
    /// Style the property with a binding.
    /// </summary>
    /// <param name="binding">The binding.</param>
    public void Style(Binding<T> binding)
    {
        SetStyle(binding);
    }

    internal override void Style(Object value)
    {
        switch (value)
        {
            case T typedValue:
                Style(typedValue);
                break;
            case Binding<T> binding:
                Style(binding);
                break;
            default:
                throw new ArgumentException($"Invalid style value for property of type {typeof(T)}: {value}", nameof(value));
        }
    }

    /// <summary>
    /// Clear the styling of the property.
    /// </summary>
    internal override void ClearStyle()
    {
        SetStyle(null);
    }

    #endregion STYLE
}
