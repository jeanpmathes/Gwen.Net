using System;
using System.Collections.Generic;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Input;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;

namespace Gwen.Net.New;

/// <summary>
/// The context in which UI elements exist.
/// It provides access to inherited values, such as styles.
/// Contexts are immutable.
/// </summary>
public class Context
{
    private readonly Context? parent;
    private readonly Canvas? canvas;

    private readonly Dictionary<Type, Style>? styles;
    private readonly Dictionary<Type, ContentTemplate>? contentTemplates;

    /// <summary>
    /// Create an inheriting context with the given parent.
    /// When creating a context for a control, using this is not necessary as the control will automatically parent its local context to the context of the parent element when attached.
    /// </summary>
    /// <param name="self">The overriding context, which is the local context of the element.</param>
    /// <param name="parent">The parent context to inherit values from.</param>
    public Context(Context self, Context parent)
    {
        this.parent = parent;
        
        canvas = self.canvas ?? parent.canvas;
        
        styles = self.styles;
        contentTemplates = self.contentTemplates;
    }

    /// <summary>
    /// Create a context from a registry.
    /// </summary>
    /// <param name="registry">The registry to create the context from.</param>
    /// <param name="canvas">The root canvas.</param>
    internal Context(ResourceRegistry registry, Canvas? canvas)
    {
        this.canvas = canvas;
        
        styles = new Dictionary<Type, Style>();
        
        foreach (Style style in registry.Styles)
        {
            styles[style.StyledType] = style;
        }

        contentTemplates = new Dictionary<Type, ContentTemplate>();
        
        foreach (ContentTemplate content in registry.ContentTemplates)
        {
            contentTemplates[content.ContentType] = content;
        }
    }
    
    /// <summary>
    /// Create a new context.
    /// While this scope is not parented, that is not an issue as elements will parent their local context when attached.
    /// </summary>
    private Context() {}

    /// <summary>
    /// Get the keyboard focus for this context.
    /// </summary>
    /// <value>The keyboard focus for this context.</value>
    /// <exception cref="InvalidOperationException">Thrown if the context is not attached to a root canvas.</exception>
    public Focus KeyboardFocus => canvas?.Input?.KeyboardFocus ?? throw new InvalidOperationException("Cannot access keyboard focus on an unattached context.");

    /// <summary>
    /// Get the pointer (mouse) focus for this context.
    /// </summary>
    /// <value>The pointer focus for this context.</value>
    /// <exception cref="InvalidOperationException">Thrown if the context is not attached to a root canvas.</exception>
    public Focus PointerFocus => canvas?.Input?.PointerFocus ?? throw new InvalidOperationException("Cannot access pointer focus on an unattached context.");

    private IStyle<T>? GetStyleForType<T>(Type type) where T : Control
    {
        if (styles != null && styles.TryGetValue(type, out Style? potentialStyle) && potentialStyle is IStyle<T> style)
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
    /// Get a content template for the given content type.
    /// </summary>
    /// <typeparam name="TContent">The content type to get the template for.</typeparam>
    /// <returns>The content template for the given type, or null if none is registered.</returns>
    public IContentTemplate<TContent> GetContentTemplate<TContent>() where TContent : class
    {
        if (contentTemplates != null && contentTemplates.TryGetValue(typeof(TContent), out ContentTemplate? potentialTemplate) && potentialTemplate is IContentTemplate<TContent> template)
            return template;
        
        return parent?.GetContentTemplate<TContent>() ?? ContentTemplate.Default;
    }
    
    /// <summary>
    /// The default, empty context.
    /// </summary>
    public static Context Default { get; } = new();
    
    /// <summary>
    /// Create a context from a registry. Use this to create contexts for visuals in a single expression.
    /// </summary>
    /// <param name="build">The action to build the registry for the context.</param>
    /// <returns>A new instance of the <see cref="Context"/> class.</returns>
    public static Context Create(Action<ResourceRegistry> build)
    {
        ResourceRegistry registry = new();
        
        build(registry);
        
        return new Context(registry, canvas: null);
    }
}
