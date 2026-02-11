using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// A <see cref="ContentControl{TContent}"/> displays content using a content template.
/// The content template is either set directly via the <see cref="ContentTemplate"/> property,
/// or retrieved from the context if no local template is set.
/// </summary>
/// <typeparam name="TContent">The type of the content.</typeparam>
public class ContentControl<TContent> : ContentControlBase<TContent, ContentControl<TContent>> where TContent : class
{
    /// <inheritdoc />
    protected override ControlTemplate<ContentControl<TContent>> CreateDefaultTemplate()
    {
        return ControlTemplate.Create<ContentControl<TContent>>(static _ => new ChildPresenter());
    }
}
