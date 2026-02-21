using System;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Internals;

namespace Gwen.Net.New.Controls.Bases;

/// <summary>
/// Abstract base class for text block controls, which display read-only text content.
/// </summary>
/// <typeparam name="TControl">The type of the control that inherits from this base class.</typeparam>
public abstract class TextBase<TControl> : SingleChildControl<TControl> where TControl : TextBase<TControl>
{
    // todo: https://learn.microsoft.com/de-de/dotnet/desktop/wpf/controls/textblock#inline-elements-and-formatting
    // todo: maybe introduce an interface for text, e.g. ITextContent instead of using string here, maybe also have inlines for images

    /// <summary>
    /// Creates a new instance of the <see cref="TextBase{TControl}"/> class.
    /// </summary>
    protected TextBase()
    {
        FontFamily = Property.Create(this, "");
        FontSize = Property.Create(this, defaultValue: 12f);
        FontStyle = Property.Create(this, Texts.Style.Normal);
        FontWeight = Property.Create(this, Texts.Weight.Normal);
        FontStretch = Property.Create(this, Texts.Stretch.Normal);

        TextWrapping = Property.Create(this, Texts.TextWrapping.Wrap);
        TextAlignment = Property.Create(this, Texts.TextAlignment.Leading);
        TextTrimming = Property.Create(this, Texts.TextTrimming.None);
        LineHeight = Property.Create(this, defaultValue: 0f);

        Content = Property.Create(this, "");
    }
    
    #region PROPERTIES
    
    /// <summary>
    /// The family of the font used to display the text content.
    /// </summary>
    public Property<String> FontFamily { get; }
    
    /// <summary>
    /// The size of the font used to display the text content.
    /// </summary>
    public Property<Single> FontSize { get; }
    
    /// <summary>
    /// The style of the font used to display the text content, such as normal, italic, or oblique.
    /// </summary>
    public Property<Texts.Style> FontStyle { get; }
    
    /// <summary>
    /// The weight of the font used to display the text content, such as normal, bold, or light.
    /// </summary>
    public Property<Texts.Weight> FontWeight { get; }
    
    /// <summary>
    /// The stretch of the font used to display the text content, such as normal, condensed, or expanded.
    /// </summary>
    public Property<Texts.Stretch> FontStretch { get; }
    
    /// <summary>
    /// How text wraps when it exceeds the layout width.
    /// </summary>
    public Property<Texts.TextWrapping> TextWrapping { get; }

    /// <summary>
    /// The horizontal alignment of text within the layout bounds.
    /// </summary>
    public Property<Texts.TextAlignment> TextAlignment { get; }

    /// <summary>
    /// How text is trimmed when it overflows the layout bounds.
    /// </summary>
    public Property<Texts.TextTrimming> TextTrimming { get; }

    /// <summary>
    /// The line height in the same units as the font size.
    /// A value of <c>0</c> means the renderer uses the font's natural line height.
    /// </summary>
    public Property<Single> LineHeight { get; }

    /// <summary>
    /// The text content to be displayed by the control.
    /// </summary>
    public Property<String> Content { get; }

    #endregion PROPERTIES
}
