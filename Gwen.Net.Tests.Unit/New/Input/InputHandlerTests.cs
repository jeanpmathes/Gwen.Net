using System.Drawing;
using Gwen.Net.New;
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

        MockControl control = new MockControl();
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
    public void PointerButtonDown_WhenVisualIsHidden_DoesNotDeliverEvent()
    {
        InputEvent? received = null;
        childVisual.OnInputHandler = e => received = e;

        childVisual.Visibility.Value = Visibility.Hidden;
        translator.PointerButtonDown(new PointF(x: 100f, y: 100f));

        Assert.Null(received);
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
        List<String> order = new List<String>();

        childVisual.OnInputPreviewHandler = _ => order.Add("tunnel");
        childVisual.OnInputHandler = _ => order.Add("bubble");

        translator.PointerButtonDown(new PointF(x: 100f, y: 100f));

        Assert.Equal(["tunnel", "bubble"], order);
    }

    [Fact]
    public void PointerButtonDown_HandledInPreview_DoesNotBubble()
    {
        Boolean bubbled = false;

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
        using Canvas nestedCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator nestedTranslator = new MockInputTranslator(nestedCanvas);

        NestedMockControl control = new NestedMockControl();
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
        using Canvas nestedCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator nestedTranslator = new MockInputTranslator(nestedCanvas);

        NestedMockControl control = new NestedMockControl();
        nestedCanvas.Child = control;
        nestedCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        nestedCanvas.Render();

        Boolean receivedByOuter = false;
        control.OuterVisual!.OnInputHandler = _ => receivedByOuter = true;

        nestedTranslator.PointerButtonDown(new PointF(x: 50f, y: 50f));

        Assert.True(receivedByOuter);
    }
    
    [Fact]
    public void Tab_OnEmptyCanvas_DoesNotSetFocus()
    {
        using Canvas emptyCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator emptyTranslator = new(emptyCanvas);
        emptyCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        emptyCanvas.Render();

        emptyTranslator.KeyDown(Key.Tab);

        Assert.Null(emptyCanvas.Input!.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void Tab_WithNoNavigableVisuals_DoesNotSetFocus()
    {
        translator.KeyDown(Key.Tab);

        Assert.Null(canvas.Input!.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void ShiftTab_WithNoNavigableVisuals_DoesNotSetFocus()
    {
        translator.KeyDown(Key.Tab, modifiers: ModifierKeys.Shift);

        Assert.Null(canvas.Input!.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void Tab_WithSingleNavigable_FocusesIt()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        SingleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localTranslator.KeyDown(Key.Tab);

        Assert.Same(localControl.Navigable, localCanvas.Input!.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void Tab_Forward_MovesFocusToNext()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        MultipleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localCanvas.Input!.KeyboardFocus.Set(localControl.A!);
        localTranslator.KeyDown(Key.Tab);

        Assert.Same(localControl.B, localCanvas.Input.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void Tab_Backward_MovesFocusToPrevious()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        MultipleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localCanvas.Input!.KeyboardFocus.Set(localControl.C!);
        localTranslator.KeyDown(Key.Tab, modifiers: ModifierKeys.Shift);

        Assert.Same(localControl.B, localCanvas.Input.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void Tab_ForwardFromLast_WrapsToFirst()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        MultipleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localCanvas.Input!.KeyboardFocus.Set(localControl.C!);
        localTranslator.KeyDown(Key.Tab);

        Assert.Same(localControl.A, localCanvas.Input.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void ShiftTab_BackwardFromFirst_WrapsToLast()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        MultipleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localCanvas.Input!.KeyboardFocus.Set(localControl.A!);
        localTranslator.KeyDown(Key.Tab, modifiers: ModifierKeys.Shift);

        Assert.Same(localControl.C, localCanvas.Input.KeyboardFocus.GetFocused());
    }

    [Fact]
    public void Tab_FocusedVisual_IsKeyboardFocused()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        MultipleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localTranslator.KeyDown(Key.Tab);

        Assert.True(localControl.A!.IsKeyboardFocused.GetValue());
    }

    [Fact]
    public void Tab_PreviouslyFocusedVisual_IsNoLongerKeyboardFocused()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        MultipleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localTranslator.KeyDown(Key.Tab);
        localTranslator.KeyDown(Key.Tab);

        Assert.False(localControl.A!.IsKeyboardFocused.GetValue());
    }
    
    [Fact]
    public void Tab_SkipsCollapsedVisuals()
    {
        using Canvas localCanvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        MockInputTranslator localTranslator = new(localCanvas);
        MultipleNavigableControl localControl = new();
        localCanvas.Child = localControl;
        localCanvas.SetRenderingSize(new Size(width: 500, height: 500));
        localCanvas.Render();

        localControl.B!.Visibility.Value = Visibility.Collapsed;

        localTranslator.KeyDown(Key.Tab);
        localTranslator.KeyDown(Key.Tab);

        Assert.Same(localControl.C, localCanvas.Input!.KeyboardFocus.GetFocused());
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

    private sealed class SingleNavigableControl : Control<SingleNavigableControl>
    {
        public MockVisual? Navigable { get; private set; }

        protected override ControlTemplate<SingleNavigableControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<SingleNavigableControl>(_ =>
            {
                MockVisual container = new();
                Navigable = new MockVisual {IsNavigable = {Value = true}};
                container.AddChildVisual(Navigable);
                container.AddChildVisual(new MockVisual());

                return container;
            });
        }
    }

    private sealed class MultipleNavigableControl : Control<MultipleNavigableControl>
    {
        public MockVisual? A { get; private set; }
        public MockVisual? B { get; private set; }
        public MockVisual? C { get; private set; }

        protected override ControlTemplate<MultipleNavigableControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<MultipleNavigableControl>(_ =>
            {
                MockVisual container = new();
                A = new MockVisual {IsNavigable = {Value = true}};
                B = new MockVisual {IsNavigable = {Value = true}};
                C = new MockVisual {IsNavigable = {Value = true}};
                container.AddChildVisual(A);
                container.AddChildVisual(B);
                container.AddChildVisual(C);

                return container;
            });
        }
    }
}
