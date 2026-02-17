using System.Drawing;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Internals;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Resources;
using Gwen.Net.New.Visuals;
using Gwen.Net.Tests.Unit.New.Controls;
using Gwen.Net.Tests.Unit.New.Rendering;
using Canvas = Gwen.Net.New.Controls.Canvas;

namespace Gwen.Net.Tests.Unit.New.Visuals;

public class ChildrenPresenterTests() : VisualTestBase<ChildrenPresenter>(() => new MockChildrenPresenter())
{
    [Fact]
    public void OnAttach_VisualizesExistingChildren()
    {
        using ResourceRegistry registry = new();
        using var canvas = Canvas.Create(new MockRenderer(), registry);

        MockMultiChildControl control = new();
        Control child1 = new MockControl();
        Control child2 = new MockControl();

        control.Children.Add(child1);
        control.Children.Add(child2);

        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 200, height: 200));
        canvas.Render();

        Assert.Equal(expected: 2, actual: control.GetPresenter().Children.Count);
    }

    [Fact]
    public void ChildAdded_AfterAttach_VisualizesNewChild()
    {
        using ResourceRegistry registry = new();
        using var canvas = Canvas.Create(new MockRenderer(), registry);

        MockMultiChildControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 200, height: 200));
        canvas.Render();

        Control child = new MockControl();
        control.Children.Add(child);

        Assert.Single(control.GetPresenter().Children);
    }

    [Fact]
    public void ChildRemoved_AfterAttach_RemovesVisualization()
    {
        using ResourceRegistry registry = new();
        using var canvas = Canvas.Create(new MockRenderer(), registry);

        MockMultiChildControl control = new();
        Control child = new MockControl();
        control.Children.Add(child);

        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 200, height: 200));
        canvas.Render();

        Assert.Single(control.GetPresenter().Children);

        control.Children.Remove(child);

        Assert.Empty(control.GetPresenter().Children);
    }

    [Fact]
    public void MultipleChildrenAdded_AllVisualized()
    {
        using ResourceRegistry registry = new();
        using var canvas = Canvas.Create(new MockRenderer(), registry);

        MockMultiChildControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 200, height: 200));
        canvas.Render();

        control.Children.Add(new MockControl());
        control.Children.Add(new MockControl());
        control.Children.Add(new MockControl());

        Assert.Equal(expected: 3, actual: control.GetPresenter().Children.Count);
    }

    [Fact]
    public void AddAndRemoveMultipleChildren_PresenterStaysConsistent()
    {
        using ResourceRegistry registry = new();
        using var canvas = Canvas.Create(new MockRenderer(), registry);

        MockMultiChildControl control = new();
        canvas.Child = control;
        canvas.SetRenderingSize(new Size(width: 200, height: 200));
        canvas.Render();

        Control child1 = new MockControl();
        Control child2 = new MockControl();
        Control child3 = new MockControl();

        control.Children.Add(child1);
        control.Children.Add(child2);
        control.Children.Add(child3);

        Assert.Equal(expected: 3, actual: control.GetPresenter().Children.Count);

        control.Children.Remove(child2);

        Assert.Equal(expected: 2, actual: control.GetPresenter().Children.Count);

        control.Children.Remove(child1);
        control.Children.Remove(child3);

        Assert.Empty(control.GetPresenter().Children);
    }

    private class MockChildrenPresenter : ChildrenPresenter;
    
    private class MockMultiChildControl : MultiChildControl<MockMultiChildControl>
    {
        private MockChildrenPresenter? presenter;

        public MockChildrenPresenter GetPresenter() => presenter!;

        protected override ControlTemplate<MockMultiChildControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<MockMultiChildControl>(_ =>
            {
                presenter = new MockChildrenPresenter();

                return presenter;
            });
        }
    }
}
