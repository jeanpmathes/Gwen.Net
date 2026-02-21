using System;
using System.Drawing;
using Gwen.Net.New.Texts;
using Brush = Gwen.Net.New.Graphics.Brush;
using Font = Gwen.Net.New.Texts.Font;

namespace Gwen.Net.OpenTk.New.Graphics;

public sealed class FormattedText(Renderer renderer, String text, Font font, TextOptions options) : IFormattedText
{
    public String Text { get; } = text;

    public Font Font { get; } = font;

    public StringFormat StringFormat { get; } = CreateStringFormat(options);

    private static StringFormat CreateStringFormat(TextOptions options)
    {
        StringFormat format = new();

        if (options.Wrapping == TextWrapping.NoWrap)
            format.FormatFlags |= StringFormatFlags.NoWrap;

        format.Alignment = options.Alignment switch
        {
            TextAlignment.Leading => StringAlignment.Near,
            TextAlignment.Center => StringAlignment.Center,
            TextAlignment.Trailing => StringAlignment.Far,
            _ => StringAlignment.Near
        };

        format.Trimming = options.Trimming switch
        {
            TextTrimming.None => StringTrimming.None,
            TextTrimming.Character => StringTrimming.Character,
            TextTrimming.Word => StringTrimming.Word,
            TextTrimming.CharacterEllipsis => StringTrimming.EllipsisCharacter,
            TextTrimming.WordEllipsis => StringTrimming.EllipsisWord,
            TextTrimming.PathEllipsis => StringTrimming.EllipsisPath,
            _ => StringTrimming.None
        };
        
        return format;
    }

    public SizeF Measure(SizeF availableSize)
    {
        return renderer.MeasureText(this, availableSize);
    }

    public void Draw(RectangleF rectangle, Brush brush)
    {
        renderer.DrawText(this, rectangle, brush);
    }

    public void Dispose()
    {
        StringFormat.Dispose();
    }
}
