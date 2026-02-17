using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Internals;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Resources;
using Gwen.Net.Tests.Unit.New.Rendering;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Controls;

public class MultiChildControlTests
{
    [Fact]
    public void Children_IsEmpty_ByDefault()
    {
        MockControl control = new();

        Assert.Empty(control.Children);
    }

    [Fact]
    public void Children_CanContainMultipleControls()
    {
        MockControl control = new();
        MockControl firstChild = new();
        MockControl secondChild = new();

        control.Children.Add(firstChild);
        control.Children.Add(secondChild);

        Assert.Equal(expected: 2, control.Children.Count);
        Assert.Contains(firstChild, control.Children);
        Assert.Contains(secondChild, control.Children);
    }

    [Fact]
    public void Children_Add_SetsParent()
    {
        MockControl control = new();
        MockControl child = new();

        control.Children.Add(child);

        Assert.Equal(control, child.Parent);
    }

    [Fact]
    public void Children_Remove_ClearsParent()
    {
        MockControl control = new();
        MockControl child = new();

        control.Children.Add(child);
        control.Children.Remove(child);

        Assert.Null(child.Parent);
        Assert.Empty(control.Children);
    }

    [Fact]
    public void Children_Add_RaisesChildAddedEvent()
    {
        MockControl control = new();
        MockControl child = new();
        Control? addedChild = null;

        control.ChildAdded += (_, e) => addedChild = e.Child;
        control.Children.Add(child);

        Assert.Equal(child, addedChild);
    }

    [Fact]
    public void Children_Remove_RaisesChildRemovedEvent()
    {
        MockControl control = new();
        MockControl child = new();
        Control? removedChild = null;

        control.Children.Add(child);
        control.ChildRemoved += (_, e) => removedChild = e.Child;
        control.Children.Remove(child);

        Assert.Equal(child, removedChild);
    }

    [Fact]
    public void Children_Set_ReplacesAllExistingChildren()
    {
        MockControl control = new();
        MockControl oldChild = new();
        MockControl newChild1 = new();
        MockControl newChild2 = new();

        control.Children.Add(oldChild);

        control.Children = [newChild1, newChild2];

        Assert.Equal(expected: 2, control.Children.Count);
        Assert.Contains(newChild1, control.Children);
        Assert.Contains(newChild2, control.Children);
        Assert.Null(oldChild.Parent);
    }

    [Fact]
    public void Children_Set_WithListSlot_BindsToSlot()
    {
        MockControl control = new();
        ListSlot<Control> externalChildren = [];
        MockControl child = new();

        control.Children = externalChildren;
        externalChildren.Add(child);

        Assert.Single(control.Children);
        Assert.Equal(child, control.Children[0]);
        Assert.Equal(control, child.Parent);
    }

    [Fact]
    public void Children_Set_WithListSlot_RemoveFromSlot_RemovesChild()
    {
        MockControl control = new();
        ListSlot<Control> externalChildren = [];
        MockControl child = new();

        control.Children = externalChildren;
        externalChildren.Add(child);
        externalChildren.Remove(child);

        Assert.Empty(control.Children);
        Assert.Null(child.Parent);
    }

    [Fact]
    public void Children_Set_WithListSlot_ExistingItemsAreAdded()
    {
        MockControl control = new();
        MockControl child = new();
        ListSlot<Control> externalChildren = [child];

        control.Children = externalChildren;

        Assert.Single(control.Children);
        Assert.Equal(child, control.Children[0]);
        Assert.Equal(control, child.Parent);
    }

    [Fact]
    public void Children_Set_WithSameReference_DoesNothing()
    {
        MockControl control = new();
        MockControl child = new();

        control.Children.Add(child);
        IList<Control> sameRef = control.Children;
        control.Children = sameRef;

        Assert.Single(control.Children);
        Assert.Equal(child, control.Children[0]);
    }

    [Fact]
    public void Children_Set_WithPlainList_CopiesItems()
    {
        MockControl control = new();
        MockControl child1 = new();
        MockControl child2 = new();
        List<Control> plainList = [child1, child2];

        control.Children = plainList;

        Assert.Equal(expected: 2, control.Children.Count);
        Assert.Equal(control, child1.Parent);
        Assert.Equal(control, child2.Parent);
    }

    [Fact]
    public void Children_AddingChildWithExistingParent_ReparentsChild()
    {
        MockControl parent1 = new();
        MockControl parent2 = new();
        MockControl child = new();

        parent1.Children.Add(child);
        parent2.Children.Add(child);

        Assert.Empty(parent1.Children);
        Assert.Single(parent2.Children);
        Assert.Equal(parent2, child.Parent);
    }

    [Fact]
    public void Children_AddingSameChildTwice_ChildKeepsSameParent()
    {
        MockControl control = new();
        MockControl child = new();

        control.Children.Add(child);
        control.Children.Add(child);

        Assert.Equal(control, child.Parent);
    }

    [Fact]
    public void Children_AddingSameChildTwice_LocalListStaysConsistent()
    {
        MockControl control = new();
        MockControl child = new();

        control.Children.Add(child);
        control.Children.Add(child);

        Assert.Single(control.Children);
        Assert.Equal(control, child.Parent);
    }

    [Fact]
    public void Children_AttachedToCanvas_ChildrenGetAttached()
    {
        using ResourceRegistry registry = new();
        using Canvas canvas = Canvas.Create(new MockRenderer(), registry);

        MockControl control = new();

        canvas.Child = control;
        
        MockControl lateChild = new();
        var lateChildAttached = false;
        lateChild.AttachedToRoot += (_, _) => lateChildAttached = true;

        control.Children.Add(lateChild);

        Assert.True(lateChildAttached);
    }

    [Fact]
    public void Children_Set_WithListSlot_UnsupportedOperations_Throw()
    {
        MockControl control = new();
        ListSlot<Control> externalChildren = [];
        MockControl child1 = new();
        MockControl child2 = new();

        control.Children = externalChildren;
        externalChildren.Add(child1);
        externalChildren.Add(child2);

        Assert.Throws<NotSupportedException>(() => externalChildren.Sort((a, b) => 0));
        Assert.Throws<NotSupportedException>(() => externalChildren.Move(oldIndex: 0, newIndex: 1));
        Assert.Throws<NotSupportedException>(() => externalChildren[0] = new MockControl());
    }

    private class MockControl : MultiChildControl<MockControl>
    {
        protected override ControlTemplate<MockControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<MockControl>(_ => new MockVisual());
        }
    }
}
