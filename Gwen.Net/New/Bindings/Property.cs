using System;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// Utility class for defining properties.
/// </summary>
public static class Properties 
{
    /// <summary>
    /// Create a new property with the given default binding.
    /// </summary>
    /// <param name="owner">The owner element of the property.</param>
    /// <param name="defaultBinding">The default binding for the property.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static Property<T> Create<T>(Element owner, Binding<T> defaultBinding)
    {
        Property<T> property = new(defaultBinding);
        
        owner.AttachedToRoot += (_, _) => property.Activate();
        owner.DetachedFromRoot += (_, _) => property.Deactivate();
        
        return property;
    }
    
    /// <summary>
    /// Create a new property with a constant default value.
    /// </summary>
    /// <param name="owner">The owner element of the property.</param>
    /// <param name="defaultValue">The default value for the property.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static Property<T> Create<T>(Element owner, T defaultValue)
    {
        return Create(owner, Bindings.Constant(defaultValue));
    }
    
    /// <summary>
    /// Create a new property with the given default binding and invalidation behavior.
    /// </summary>
    /// <param name="owner">The owner element of the property.</param>
    /// <param name="defaultBinding">>The default binding for the property.</param>
    /// <param name="invalidation">The invalidation behavior when the property value changes.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static Property<T> Create<T>(VisualElement owner, Binding<T> defaultBinding, Invalidation invalidation)
    {
        Property<T> property = new(defaultBinding);
        
        owner.AttachedToRoot += (_, _) => property.Activate();
        owner.DetachedFromRoot += (_, _) => property.Deactivate();
        
        EventHandler invalidator = invalidation switch
        {
            Invalidation.InvalidateVisualization => (_, _) => owner.InvalidateVisualization(),
            Invalidation.InvalidateMeasure => (_, _) => owner.InvalidateMeasure(),
            Invalidation.InvalidateArrange => (_, _) => owner.InvalidateArrange(),
            Invalidation.InvalidateRender => (_, _) => owner.InvalidateRender(),
            _ => throw new ArgumentOutOfRangeException(nameof(invalidation), invalidation, message: null)
        };

        property.ValueChanged += invalidator;
        
        return property;
    }

    /// <summary>
    /// Create a new property with a constant default value and invalidation behavior.
    /// </summary>
    /// <param name="owner">The owner element of the property.</param>
    /// <param name="defaultValue">The default value for the property.</param>
    /// <param name="invalidation">The invalidation behavior when the property value changes.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static Property<T> Create<T>(VisualElement owner, T defaultValue, Invalidation invalidation)
    {
        return Create(owner, Bindings.Constant(defaultValue), invalidation);
    }
}

/// <summary>
/// A property is a value of a <see cref="Element"/> that can be set, styled and bound to a slot.
/// </summary>
/// <typeparam name="T">The type of value stored in the property.</typeparam>
public sealed class Property<T> : IValueSource<T>
{
    private readonly Binding<T> defaultBinding;

    private Binding<T>? styleBinding;
    private Binding<T>? localBinding;

    private Binding<T> targetBinding;
    private Boolean isActive;
    
    private T? cachedValue;
    private Boolean isCacheValid;
    
    internal Property(Binding<T> defaultBinding)
    {
        this.defaultBinding = defaultBinding;
        
        targetBinding = defaultBinding;
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
            ValueChanged?.Invoke(this, EventArgs.Empty);
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

    /// <inheritdoc/>
    public event EventHandler? ValueChanged;
    
    /// <summary>
    /// Implicitly converts the property to its value by calling <see cref="GetValue"/>.
    /// </summary>
    /// <param name="property">The property to convert.</param>
    /// <returns>The value of the property.</returns>
    public static implicit operator T(Property<T> property) => property.GetValue();
    
    #region LOCAL
    
    private void SetLocal(Binding<T> newLocalBinding)
    {
        localBinding = newLocalBinding;
        RecomputeTargetBinding();
    }
    
    private void SetStyle(Binding<T> newStyleBinding)
    {
        styleBinding = newStyleBinding;
        RecomputeTargetBinding();
    }
    
    /// <summary>
    /// Binds the property to a constant value locally.
    /// </summary>
    public T Value
    {
        set => SetLocal(Bindings.Constant(value));
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

    /// <summary>
    /// Style the property with a constant value.
    /// </summary>
    /// <param name="value">The constant value.</param>
    public void Set(T value)
    {
        SetStyle(Bindings.Constant(value));
    }
    
    /// <summary>
    /// Style the property with a binding.
    /// </summary>
    /// <param name="binding">The binding.</param>
    public void Set(Binding<T> binding)
    {
        SetStyle(binding);
    }

    #endregion STYLE
}
