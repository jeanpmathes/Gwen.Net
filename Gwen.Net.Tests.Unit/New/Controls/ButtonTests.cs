using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Visuals;
using Canvas = Gwen.Net.New.Controls.Canvas;
using Gwen.Net.Tests.Unit.New.Commands;
using Gwen.Net.Tests.Unit.New.Input;
using Gwen.Net.Tests.Unit.New.Rendering;

namespace Gwen.Net.Tests.Unit.New.Controls;

public sealed class ButtonTests : ControlTestBase<Button<String>>, IDisposable
{
    private readonly Canvas canvas;
    private readonly MockInputTranslator translator;
    private readonly Button<String> button;
    private readonly MockCommand command;

    public ButtonTests() : base(() => new Button<String>())
    {
        canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        translator = new MockInputTranslator(canvas);

        button = new Button<String>();
        command = new MockCommand();
        button.Command.Value = command;

        canvas.Child = button;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();
    }

    public void Dispose()
    {
        canvas.Dispose();
    }

    [Fact]
    public void PointerClick_ExecutesCommand()
    {
        translator.Click(button);

        Assert.Equal(expected: 1, command.ExecuteCount);
    }

    [Fact]
    public void PointerClick_MultipleClicks_ExecutesCommandEachTime()
    {
        translator.Click(button);
        translator.Click(button);
        translator.Click(button);

        Assert.Equal(expected: 3, command.ExecuteCount);
    }

    [Fact]
    public void PointerPressAndReleaseOutside_DoesNotExecuteCommand()
    {
        translator.PointerButtonDown(button);
        translator.PointerButtonUp(new PointF(x: -10f, y: -10f));

        Assert.Equal(expected: 0, command.ExecuteCount);
    }

    [Fact]
    public void PointerClick_NoCommand_DoesNotThrow()
    {
        button.Command.Value = null;

        Exception? ex = Record.Exception(() => translator.Click(button));

        Assert.Null(ex);
    }

    [Fact]
    public void PointerDown_SetsIsPressed()
    {
        translator.PointerButtonDown(button);

        Assert.True(button.IsPressed.GetValue());
    }

    [Fact]
    public void PointerUpAfterDown_ClearsIsPressed()
    {
        translator.PointerButtonDown(button);
        translator.PointerButtonUp(button);

        Assert.False(button.IsPressed.GetValue());
    }

    [Fact]
    public void PointerDown_SetsPointerFocus()
    {
        translator.PointerButtonDown(button);

        Visual? focused = canvas.Input!.PointerFocus.GetFocused();

        Assert.Equal(button.Visualization.GetValue(), focused);
    }

    [Fact]
    public void PointerUpAfterDown_ReleasesPointerFocus()
    {
        translator.PointerButtonDown(button);
        translator.PointerButtonUp(button);

        Visual? focused = canvas.Input!.PointerFocus.GetFocused();

        Assert.Null(focused);
    }
}
