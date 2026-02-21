using System;
using System.Drawing;
using Gwen.Net.New.Bindings;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Texts;
using Gwen.Net.New.Utilities;

namespace Gwen.Net.New.Visuals;

/// <summary>
/// Displays text.
/// </summary>
/// <seealso cref="Controls.Text"/>
public class Text : Visual
{
    private IFormattedText? formattedText;
    
    /// <summary>
    /// Creates a new instance of the <see cref="Text"/> class.
    /// </summary>
    public Text()
    {
        FontFamily = VisualProperty.Create(this, "", _ => InvalidateText());
        FontSize = VisualProperty.Create(this, defaultValue: 12f, _ => InvalidateText());
        FontStyle = VisualProperty.Create(this, Style.Normal, _ => InvalidateText());
        FontWeight = VisualProperty.Create(this, Weight.Normal, _ => InvalidateText());
        FontStretch = VisualProperty.Create(this, Stretch.Normal, _ => InvalidateText());
        
        Content = VisualProperty.Create(this, "", _ => InvalidateText());
        
        Brush = VisualProperty.Create(this, BindToOwnerForeground(), Invalidation.Render);
    }

    #region PROPERTIES
    
    /// <summary>
    /// The family of the font used to display the text content.
    /// </summary>
    public VisualProperty<String> FontFamily { get; }
    
    /// <summary>
    /// The size of the font used to display the text content.
    /// </summary>
    public VisualProperty<Single> FontSize { get; }
    
    /// <summary>
    /// The style of the font used to display the text content, such as normal, italic, or oblique.
    /// </summary>
    public VisualProperty<Style> FontStyle { get; }
    
    /// <summary>
    /// The weight of the font used to display the text content, such as normal, bold, or light.
    /// </summary>
    public VisualProperty<Weight> FontWeight { get; }
    
    /// <summary>
    /// The stretch of the font used to display the text content, such as normal, condensed, or expanded.
    /// </summary>
    public VisualProperty<Stretch> FontStretch { get; }
    
    /// <summary>
    /// The text content to be displayed by the control.
    /// </summary>
    public VisualProperty<String> Content { get; }
    
    /// <summary>
    /// The brush used to draw the text content.
    /// </summary>
    public VisualProperty<Brush> Brush { get; }
    
    #endregion PROPERTIES
    
    #region TEXT 
    
    /// <inheritdoc/>
    public override void OnAttach()
    {
        if (formattedText == null)
            CreateFormattedText();
    }

    /// <inheritdoc/>
    public override void OnDetach(Boolean isReparenting)
    {
        if (isReparenting) return;
        
        formattedText?.Dispose();
        formattedText = null;
    }
    
    private void InvalidateText()
    {
        if (formattedText == null) return;
        
        formattedText.Dispose();
        formattedText = null;

        if (!IsAttached) return;

        CreateFormattedText();
    }

    private void CreateFormattedText()
    {
        formattedText = Renderer.CreateFormattedText(Content.GetValue(), new Font
        {
            Family = FontFamily.GetValue(),
            Size = FontSize.GetValue(),
            Style = FontStyle.GetValue(),
            Weight = FontWeight.GetValue(),
            Stretch = FontStretch.GetValue()
        });
        
        InvalidateMeasure();
    }
    
    /// <inheritdoc/>
    protected override void OnRender()
    {
        formattedText?.Draw(finalTextRectangle, Brush.GetValue());
    }
    
    #endregion TEXT
    
    #region LAYOUT
    
    private RectangleF finalTextRectangle;

    /// <inheritdoc/>
    public override SizeF OnMeasure(SizeF availableSize)
    {
        if (formattedText == null)
            return SizeF.Empty;

        availableSize -= Padding.GetValue();
        
        SizeF textSize = formattedText.Measure(Sizes.Max(SizeF.Empty, availableSize));
        
        return textSize + Padding.GetValue();
    }

    /// <inheritdoc/>
    public override void OnArrange(RectangleF finalRectangle)
    {
        finalTextRectangle = finalRectangle - Padding.GetValue();
    }

    #endregion LAYOUT
}
