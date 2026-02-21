using System;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Texts;

namespace Gwen.Net.OpenTk.New.Graphics;

public sealed class FormattedText(Renderer renderer, String text, Font font) : IFormattedText
{
    public String Text { get; } = text;

    public Font Font { get; } = font;

    public System.Drawing.StringFormat StringFormat { get; } = new();

    public System.Drawing.SizeF Measure(System.Drawing.SizeF availableSize)
    {
        return renderer.MeasureText(this, availableSize);
    }
    
    public void Draw(System.Drawing.RectangleF rectangle, Brush brush)
    {
        renderer.DrawText(this, rectangle, brush);
    }

    public void Dispose()
    {
        StringFormat.Dispose();
    }
}
