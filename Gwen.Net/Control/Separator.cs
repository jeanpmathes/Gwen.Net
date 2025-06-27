using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control;

/// <summary>
/// A simple horizontal line, optionally containing text.
/// </summary>
public class Separator : ControlBase
{
    private readonly Text text;
    
    public Separator(ControlBase parent) : base(parent)
    {
        text = new Text(this)
        {
            VerticalAlignment = VerticalAlignment.Center
        };
    }
    
    /// <summary>
    ///     The text of the separator.
    /// </summary>
    public virtual String Text
    {
        get => text.String;
        set => text.String = value;
    }
    
    /// <summary>
    ///     The color of the text.
    /// </summary>
    public virtual Color TextColor
    {
        get => text.TextColor;
        set => text.TextColor = value;
    }
    
    /// <summary>
    ///     The font of the text.
    /// </summary>
    public virtual Font TextFont
    {
        get => text.Font;
        set => text.Font = value;
    }
    
    protected override Size Measure(Size availableSize)
    {
        Size titleSize = text.DoMeasure(availableSize);

        return new Size(
            10 + titleSize.Width + 10,
            titleSize.Height + 5);
    }

    protected override Size Arrange(Size finalSize)
    {
        text.DoArrange(new Rectangle(x: 10, y: 0, text.MeasuredSize.Width, text.MeasuredSize.Height));

        return finalSize;
    }
    
    protected override void Render(SkinBase currentSkin)
    {
        Int32 textWidth = text.String.Length > 0 ? text.ActualWidth : 0;
        
        currentSkin.DrawSeparator(this, textStart: 10, textWidth);
    }
}
