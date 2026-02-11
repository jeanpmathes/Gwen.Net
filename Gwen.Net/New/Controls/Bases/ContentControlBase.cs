using System;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Templates;

namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// Abstract base class for a content control, which is a control that displays content using a content template.
/// </summary>
/// <typeparam name="TContent">The type of the content.</typeparam>
/// <typeparam name="TControl">The concrete type of the control.</typeparam>
public abstract class ContentControlBase<TContent, TControl> : Control<TControl> where TContent : class where TControl : ContentControlBase<TContent, TControl> 
{
    /// <summary>
    /// Creates a new instance of the <see cref="ContentControlBase{TContent, TControl}"/> class.
    /// </summary>
    protected ContentControlBase()
    {
        Content = Property.Create(this, default(TContent?));
        Content.ValueChanged += OnContentChanged;
        
        ContentTemplate = Property.Create(this, default(ContentTemplate<TContent>?));
        ContentTemplate.ValueChanged += OnContentTemplateChanged;
    }
    
    #region PROPERTIES
    
    /// <summary>
    /// The content to display.
    /// Content can be any objects, including controls.
    /// </summary>
    public Property<TContent?> Content { get; }
    
    /// <summary>
    /// The template used to display the content.
    /// If not set, the template will be retrieved from the context based on the content type.
    /// </summary>
    public Property<ContentTemplate<TContent>?> ContentTemplate { get; }
    
    #endregion PROPERTIES
    
    private void OnContentChanged(Object? sender, EventArgs e)
    {
        UpdateChild();
    }
    
    private void OnContentTemplateChanged(Object? sender, EventArgs e)
    {
        UpdateChild();
    }
    
    /// <inheritdoc/>
    protected override void OnInvalidateContext()
    {
        UpdateChild();
    }
    
    private void UpdateChild()
    {
        TContent? content = Content.GetValue();
        
        if (content == null)
        {
            SetChild(null);
            return;
        }
        
        Control child = GetContentTemplate().Apply(content);
        
        SetChild(child);
    }
    
    private IContentTemplate<TContent> GetContentTemplate()
    {
        if (ContentTemplate.GetValue() is {} localTemplate)
            return localTemplate;
        
        return Context.GetContentTemplate<TContent>();
    }
}
