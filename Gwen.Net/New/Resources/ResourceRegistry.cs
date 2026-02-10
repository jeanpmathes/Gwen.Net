using System;
using System.Collections.Generic;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Styles;

namespace Gwen.Net.New.Resources;

/// <summary>
/// A registry for resources in a resource bundle.
/// </summary>
public sealed class ResourceRegistry : IDisposable
{
    private readonly List<Style> styles = [];
    
    /// <summary>
    /// Get all styles registered in this registry, in order of registration.
    /// </summary>
    public IReadOnlyList<Style> Styles  => styles;
    
    /// <summary>
    /// Create a style for a specific element type and register it in the registry.
    /// </summary>
    /// <param name="builder">The builder for the style.</param>
    /// <typeparam name="TControl">The element type this style is for.</typeparam>
    /// <returns>A new instance of the <see cref="Style{TElement}"/> class.</returns>
    public Style<TControl> AddStyle<TControl>(Action<Styling.IBuilder<TControl>> builder) where TControl : Control
    {
        Style<TControl> style = Styling.Create(builder);
        
        styles.Add(style);
        
        return style;
    }

    /// <summary>
    /// Create and add a resource bundle to the registry.
    /// </summary>
    /// <typeparam name="TResourceBundle">The type of the resource bundle to add.</typeparam>
    /// <returns>The loaded resource bundle.</returns>
    public TResourceBundle AddBundle<TResourceBundle>() where TResourceBundle : IResourceBundle<TResourceBundle>
    {
        TResourceBundle bundle = TResourceBundle.Load(this);
        
        return bundle;
    }
    
    /// <inheritdoc/>
    public void Dispose()
    {
        
    }
}
