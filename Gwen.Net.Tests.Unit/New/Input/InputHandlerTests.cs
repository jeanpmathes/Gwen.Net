using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Input;
using Gwen.Net.New.Resources;
using Gwen.Net.Tests.Unit.New.Rendering;
using Gwen.Net.Tests.Unit.New.Visuals;
using Canvas = Gwen.Net.New.Controls.Canvas;

namespace Gwen.Net.Tests.Unit.New.Input;

public sealed class InputHandlerTests : IDisposable
{
    private readonly Canvas canvas;
    private readonly MockInputTranslator translator;
    private readonly MockVisual childVisual;

    public InputHandlerTests()
    {
        canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        translator = new MockInputTranslator(canvas);

        var control = new MockControl();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 500, height: 500));
        canvas.Render();

        childVisual = control.Visual!;
    }

    public void Dispose()
    {
        canvas.Dispose();
    }

    [Fact]
    public void PointerButtonDown_InsideBounds_DeliversEventToVisual()
    {
        InputEvent? received = null;
        childVisual.OnInputHandler = e => received = e;

        translator.PointerButtonDown(new PointF(x: 100f, y: 100f));

        Assert.IsType<PointerButtonEvent>(received);
    }

    [Fact]
    public void PointerButtonDown_OutsideBounds_DoesNotDeliverEvent()
    {
        InputEvent? received = null;
        childVisual.OnInputHandler = e => received = e;

        translator.PointerButtonDown(new PointF(x: -10f, y: -10f));

        Assert.Null(received);
    }

    [Fact]
    public void PointerButtonDown_EventTunnelsThenBubbles()
    {
        var order = new List<String>();

        childVisual.OnInputPreviewHandler = _ => order.Add("tunnel");
        childVisual.OnInputHandler = _ => order.Add("bubble");

        translator.PointerButtonDown(new PointF(x: 100f, y: 100f));

        Assert.Equal(["tunnel", "bubble"], order);
    }

    [Fact]
    public void PointerButtonDown_HandledInPreview_DoesNotBubble()
    {
        var bubbled = false;

        childVisual.OnInputPreviewHandler = e => e.Handled = true;
        childVisual.OnInputHandler = _ => bubbled = true;

        translator.PointerButtonDown(new PointF(x: 100f, y: 100f));

        Assert.False(bubbled);
    }

    [Fact]
    public void PointerMove_InsideBounds_DeliversPointerMoveEvent()
    {
        InputEvent? received = null;
        childVisual.OnInputHandler = e => received = e;

        translator.PointerMove(new PointF(x: 100f, y: 100f), deltaX: 5f, deltaY: 3f);

        Assert.IsType<PointerMoveEvent>(received);
    }

    [Fact]
    public void Scroll_InsideBounds_DeliversScrollEvent()
    {
        InputEvent? received = null;
        childVisual.OnInputHandler = e => received = e;

        translator.Scroll(new PointF(x: 100f, y: 100f), deltaX: 0f, deltaY: 120f);

        Assert.IsType<ScrollEvent>(received);
    }

    [Fact]
    public void KeyDown_WithoutKeyboardFocus_DoesNotDeliverEvent()
    {
        InputEvent? received = null;
        childVisual.OnInputHandler = e => received = e;

        translator.KeyDown(Key.A);

        Assert.Null(received);
    }

    [Fact]
    public void Text_WithoutKeyboardFocus_DoesNotDeliverEvent()
    {
        InputEvent? received = null;
        childVisual.OnInputHandler = e => received = e;

        translator.Text("hello");

        Assert.Null(received);
    }

    [Fact]
    public void PointerButtonDown_NestedHierarchy_HitsDeepestChild()
    {
        using var nestedCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        var nestedTranslator = new MockInputTranslator(nestedCanvas);

        var control = new NestedMockControl();
        nestedCanvas.Child = control;
        nestedCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        nestedCanvas.Render();

        InputEvent? innerReceived = null;
        control.InnerVisual!.OnInputHandler = e => innerReceived = e;

        nestedTranslator.PointerButtonDown(new PointF(x: 50f, y: 50f));

        Assert.IsType<PointerButtonEvent>(innerReceived);
        Assert.Same(control.InnerVisual, innerReceived!.Source);
    }

    [Fact]
    public void PointerButtonDown_NestedHierarchy_BubblesToParent()
    {
        using var nestedCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        var nestedTranslator = new MockInputTranslator(nestedCanvas);

        var control = new NestedMockControl();
        nestedCanvas.Child = control;
        nestedCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        nestedCanvas.Render();

        var receivedByOuter = false;
        control.OuterVisual!.OnInputHandler = _ => receivedByOuter = true;

        nestedTranslator.PointerButtonDown(new PointF(x: 50f, y: 50f));

        Assert.True(receivedByOuter);
    }

    private sealed class MockControl : Control<MockControl>
    {
        public MockVisual? Visual { get; private set; }

        protected override ControlTemplate<MockControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<MockControl>(_ =>
            {
                Visual = new MockVisual();

                return Visual;
            });
        }
    }

    private sealed class NestedMockControl : Control<NestedMockControl>
    {
        public MockVisual? OuterVisual { get; private set; }
        public MockVisual? InnerVisual { get; private set; }

        protected override ControlTemplate<NestedMockControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<NestedMockControl>(_ =>
            {
                InnerVisual = new MockVisual();
                OuterVisual = new MockVisual();
                OuterVisual.SetChildVisual(InnerVisual);

                return OuterVisual;
            });
        }
    }
}
