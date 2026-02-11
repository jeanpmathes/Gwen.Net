using System;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Bindings;

/// <summary>
/// Abstract base class for visual properties.
/// </summary>
public class VisualProperty : IValueSource
{
    private readonly Visual owner;
    private readonly Invalidation invalidation;
    
    /// <summary>
    /// Initializes a new visual property base with its owner and invalidation behavior.
    /// </summary>
    /// <param name="owner">The visual that owns this property.</param>
    /// <param name="invalidation">The invalidation to trigger when the property changes.</param>
    protected VisualProperty(Visual owner, Invalidation invalidation)
    {
        this.owner = owner;
        this.invalidation = invalidation;
    }
    
    /// <summary>
    /// Create a new property with the given default binding and invalidation behavior.
    /// </summary>
    /// <param name="owner">The visual that owns the property.</param>
    /// <param name="defaultBinding">The default binding for the property.</param>
    /// <param name="invalidation">The invalidation behavior when the property value changes.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static VisualProperty<T> Create<T>(Visual owner, Binding<T> defaultBinding, Invalidation invalidation = Invalidation.None)
    {
        return new VisualProperty<T>(owner, invalidation, defaultBinding);
    }

    /// <summary>
    /// Create a new property with a constant default value and invalidation behavior.
    /// </summary>
    /// <param name="owner">The visual that owns the property.</param>
    /// <param name="defaultValue">The default value for the property.</param>
    /// <param name="invalidation">The invalidation behavior when the property value changes.</param>
    /// <typeparam name="T">The type of value stored in the property.</typeparam>
    /// <returns>The created property.</returns>
    public static VisualProperty<T> Create<T>(Visual owner, T defaultValue, Invalidation invalidation = Invalidation.None)
    {
        return Create(owner, Binding.Constant(defaultValue), invalidation);
    }
    
    /// <summary>
    /// Notifies subscribers that the value of the property has changed.
    /// </summary>
    protected void NotifyValueChanged()
    {
        switch (invalidation)
        {
            case Invalidation.Measure:
                owner.InvalidateMeasure();
                break;
            
            case Invalidation.Arrange:
                owner.InvalidateArrange();
                break;
            
            case Invalidation.Render:
                owner.InvalidateRender();
                break;
        }
        
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public event EventHandler? ValueChanged;
}

/// <summary>
/// A visual property is a value of a <see cref="Visual"/> that can be set, styled and bound to a slot.
/// </summary>
/// <typeparam name="T">The type of value stored in the property.</typeparam>
public sealed class VisualProperty<T> : VisualProperty, IValueSource<T>
{
    private readonly Binding<T> defaultBinding;
    
    private Binding<T>? localBinding;
    
    private Binding<T> targetBinding;
    private Boolean isActive;
    
    private T? cachedValue;
    private Boolean isCacheValid;
    
    /// <summary>
    /// Initializes a new visual property with default binding and invalidation behavior.
    /// </summary>
    /// <param name="owner">The visual that owns this property.</param>
    /// <param name="invalidation">The invalidation to trigger when the property changes.</param>
    /// <param name="defaultBinding">The default value binding.</param>
    internal VisualProperty(Visual owner, Invalidation invalidation, Binding<T> defaultBinding) : base(owner, invalidation)
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
        Binding<T> binding = localBinding ?? defaultBinding;
        
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
}
