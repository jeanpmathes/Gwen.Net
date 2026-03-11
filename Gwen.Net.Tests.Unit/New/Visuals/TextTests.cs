using System.Drawing;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Texts;
using Gwen.Net.New.Visuals;
using Gwen.Net.Tests.Unit.New.Rendering;
using Canvas = Gwen.Net.New.Controls.Canvas;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class TextTests() : VisualTestBase<Text>(() => new Text())
{
    [Fact]
    public void TextVisual_CreatesFormattedTextOnAttach()
    {
        TrackingRenderer renderer = new();
        Canvas canvas = Canvas.Create(renderer, new ResourceRegistry());
        canvas.Child = new Net.New.Controls.Text();

        Assert.NotNull(renderer.LastCreatedText);
        Assert.False(renderer.LastCreatedText.IsDisposed);
    }

    [Fact]
    public void TextVisual_DisposesFormattedTextOnDetach()
    {
        TrackingRenderer renderer = new();
        Canvas canvas = Canvas.Create(renderer, new ResourceRegistry());
        canvas.Child = new Net.New.Controls.Text();

        TrackableFormattedText formattedText = renderer.LastCreatedText!;

        canvas.Child = null;

        Assert.True(formattedText.IsDisposed);
    }

    [Fact]
    public void TextVisual_DisposesOldFormattedTextOnContentChange()
    {
        TrackingRenderer renderer = new();
        Canvas canvas = Canvas.Create(renderer, new ResourceRegistry());

        Net.New.Controls.Text control = new()
            {Content = {Value = "initial"}};

        canvas.Child = control;

        TrackableFormattedText first = renderer.LastCreatedText!;

        control.Content.Value = "updated";

        Assert.True(first.IsDisposed);
    }

    [Fact]
    public void TextVisual_CreatesNewFormattedTextOnContentChange()
    {
        TrackingRenderer renderer = new();
        Canvas canvas = Canvas.Create(renderer, new ResourceRegistry());

        Net.New.Controls.Text control = new()
            {Content = {Value = "initial"}};

        canvas.Child = control;

        TrackableFormattedText first = renderer.LastCreatedText!;

        control.Content.Value = "updated";

        TrackableFormattedText second = renderer.LastCreatedText!;
        Assert.NotSame(first, second);
        Assert.False(second.IsDisposed);
    }

    private sealed class TrackableFormattedText : IFormattedText
    {
        public Boolean IsDisposed { get; private set; }

        public SizeF Measure(SizeF availableSize)
        {
            return new SizeF(width: 42, height: 24);
        }

        public void Draw(RectangleF rectangle, Brush brush) {}

        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    private sealed class TrackingRenderer : MockRenderer
    {
        public TrackableFormattedText? LastCreatedText { get; private set; }

        public override IFormattedText CreateFormattedText(String text, Font font, TextOptions options)
        {
            LastCreatedText = new TrackableFormattedText();
            return LastCreatedText;
        }
    }
}
