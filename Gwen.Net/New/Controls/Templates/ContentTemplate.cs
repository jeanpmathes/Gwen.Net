using System;
using Gwen.Net.New.Graphics;

namespace Gwen.Net.New.Controls.Templates;

/// <summary>
/// Abstract base class of all content templates.
/// </summary>
/// <seealso cref="ContentTemplate{TContent}"/>
public abstract class ContentTemplate
{
    /// <summary>
    /// Creates a content template for content of type <typeparamref name="TContent"/> using the given function.
    /// </summary>
    /// <param name="function">The function that creates the control structure for the content.</param>
    /// <typeparam name="TContent">The type of the content.</typeparam>
    /// <returns>>The created content template.</returns>
    public static ContentTemplate<TContent> Create<TContent>(Func<TContent, Control> function) where TContent : class
    {
        return new ContentTemplate<TContent>(function);
    }
    
    /// <summary>
    /// Get the trivial content template for content of type <see cref="Control"/>.
    /// This template simply returns the content as the control structure.
    /// </summary>
    public static IContentTemplate<Control> Trivial { get; } = new ContentTemplate<Control>(content => content);

    /// <summary>
    /// Get a sensible content template for content of type <see cref="String"/>.
    /// It simply displays the string in a text element.
    /// </summary>
    public static IContentTemplate<String> String { get; } = new ContentTemplate<String>(CreateStringContent);
    
    /// <summary>
    /// Get the default content template when no specific template is found for the content type.
    /// </summary>
    public static IContentTemplate<Object> Default { get; } = new ContentTemplate<Object>(content =>  CreateStringContent(content.ToString() ?? ""));

    private static Control CreateStringContent(String content)
    {
        return new Border {Foreground = {Value = Brushes.Red}}; // todo: should put a text element here instead of border
    }
    
    /// <summary>
    /// The type of content this template can be applied to.
    /// </summary>
    public abstract Type ContentType { get; }
}

/// <summary>
/// Interface for content templates, defining how to display content of a specific type.
/// </summary>
/// <typeparam name="TContent">The type of the content.</typeparam>
public interface IContentTemplate<in TContent> where TContent : class
{
    /// <summary>
    /// Applies the template to the given content, creating its control structure.
    /// </summary>
    /// <param name="content">The content to apply the template to.</param>
    /// <returns>The created control structure.</returns>
    public Control Apply(TContent content);
}

/// <summary>
/// Defines how to display content of a specific type.
/// </summary>
/// <typeparam name="TContent">The type of the content.</typeparam>
public sealed class ContentTemplate<TContent> : ContentTemplate, IContentTemplate<TContent> where TContent : class
{
    private readonly Func<TContent, Control> function;
    
    /// <summary>
    /// Creates a new content template.
    /// </summary>
    /// <param name="function">The function that creates the control structure for the content.</param>
    public ContentTemplate(Func<TContent, Control> function)
    {
        this.function = function;
    }
    
    /// <inheritdoc/>
    public Control Apply(TContent content)
    {
        return function(content);
    }

    /// <inheritdoc/>
    public override Type ContentType => typeof(TContent);
}
