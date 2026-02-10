using System;
using System.Collections.Generic;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.New;

/// <summary>
/// The context in which UI elements exist.
/// It provides access to inherited values, such as styles.
/// Contexts are immutable.
/// </summary>
public class Context
{
    private readonly Context? parent;

    private readonly TypeDictionary<Style>? styles;

    /// <summary>
    /// Create an inheriting context with the given parent.
    /// </summary>
    /// <param name="self">The overriding context, which is the local context of the element.</param>
    /// <param name="parent">The parent context to inherit values from.</param>
    internal Context(Context self, Context parent)
    {
        this.parent = parent;
        
        styles = self.styles;
    }

    /// <summary>
    /// Create a context from a registry.
    /// </summary>
    /// <param name="registry">The registry to create the context from.</param>
    internal Context(ResourceRegistry registry)
    {
        styles = new TypeDictionary<Style>();
        
        foreach (Style style in registry.Styles)
        {
            styles[style.StyledType] = style;
        }
    }
    
    /// <summary>
    /// Create a new context.
    /// While this scope is not parented, that is not an issue as elements will parent their local context when attached.
    /// </summary>
    private Context() {}
    
    private IStyle<T>? GetStyleForType<T>(Type type) where T : Control
    {
        if (styles?[type] is IStyle<T> style)
            return style;
        
        return parent?.GetStyleForType<T>(type);
    }
    
    /// <summary>
    /// Get the styles for the given element type.
    /// This will provide a list of styles, starting with the most general and ending with the most specific, that should be applied to an element of the given type.
    /// </summary>
    /// <typeparam name="T">The element type to get the style for.</typeparam>
    /// <returns>The styles for the given element type, may be empty.</returns>
    public IReadOnlyList<IStyle<T>> GetStyling<T>() where T : Control
    {
        if (styles == null) 
            return parent?.GetStyling<T>() ?? [];
        
        // todo: caching of lists
        List<IStyle<T>> result = [];
        
        Type queryType = typeof(T);
        Type? currentType = queryType;
        
        while (currentType != typeof(Object) && currentType != null)
        {
            if (GetStyleForType<T>(currentType) is {} style)
                result.Add(style);
            
            currentType = currentType.BaseType;
        }
        
        result.Reverse();
        
        return result;
    }
    
    /// <summary>
    /// The default, empty context.
    /// </summary>
    public static Context Default { get; } = new();
    
    /// <summary>
    /// Create a context from a registry.
    /// </summary>
    /// <param name="build">The action to build the registry for the context.</param>
    /// <returns>A new instance of the <see cref="Context"/> class.</returns>
    public static Context Create(Action<ResourceRegistry> build)
    {
        ResourceRegistry registry = new();
        
        build(registry);
        
        return new Context(registry);
    }
}
