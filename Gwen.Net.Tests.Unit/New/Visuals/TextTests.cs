using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Texts;
using Gwen.Net.Tests.Unit.New.Rendering;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class TextTests() : VisualTestBase<Gwen.Net.New.Visuals.Text>(() => new Gwen.Net.New.Visuals.Text())
{
    private sealed class TrackableFormattedText : IFormattedText
    {
        public Boolean IsDisposed { get; private set; }

        public SizeF Measure(SizeF availableSize) => new(width: 42, height: 24);
        public void Draw(RectangleF rectangle, Brush brush) { }
        public void Dispose() => IsDisposed = true;
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

    [Fact]
    public void TextVisual_CreatesFormattedTextOnAttach()
    {
        var renderer = new TrackingRenderer();
        var canvas = Canvas.Create(renderer, new ResourceRegistry());
        canvas.Child = new Text();

        Assert.NotNull(renderer.LastCreatedText);
        Assert.False(renderer.LastCreatedText.IsDisposed);
    }

    [Fact]
    public void TextVisual_DisposesFormattedTextOnDetach()
    {
        var renderer = new TrackingRenderer();
        var canvas = Canvas.Create(renderer, new ResourceRegistry());
        canvas.Child = new Text();

        TrackableFormattedText formattedText = renderer.LastCreatedText!;

        canvas.Child = null;

        Assert.True(formattedText.IsDisposed);
    }

    [Fact]
    public void TextVisual_DisposesOldFormattedTextOnContentChange()
    {
        var renderer = new TrackingRenderer();
        var canvas = Canvas.Create(renderer, new ResourceRegistry());
        var control = new Text { Content = { Value = "initial" } };
        canvas.Child = control;

        TrackableFormattedText first = renderer.LastCreatedText!;

        control.Content.Value = "updated";

        Assert.True(first.IsDisposed);
    }

    [Fact]
    public void TextVisual_CreatesNewFormattedTextOnContentChange()
    {
        var renderer = new TrackingRenderer();
        var canvas = Canvas.Create(renderer, new ResourceRegistry());
        var control = new Text { Content = { Value = "initial" } };
        canvas.Child = control;

        TrackableFormattedText first = renderer.LastCreatedText!;

        control.Content.Value = "updated";

        TrackableFormattedText second = renderer.LastCreatedText!;
        Assert.NotSame(first, second);
        Assert.False(second.IsDisposed);
    }
}
