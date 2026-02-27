using Gwen.Net.New.Input;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Input;

public class RouteTests
{
    [Fact]
    public void Route_Empty_HasZeroCount()
    {
        using var route = Route.Empty;

        Assert.Equal(expected: 0, route.Count);
    }

    [Fact]
    public void Route_Create_WithNull_ReturnsEmptyRoute()
    {
        using var route = Route.Create(null);

        Assert.Equal(expected: 0, route.Count);
    }

    [Fact]
    public void Route_Create_WithSingleVisual_ReturnsRouteOfLength1()
    {
        MockVisual visual = new();

        using var route = Route.Create(visual);

        Assert.Equal(expected: 1, route.Count);
    }

    [Fact]
    public void Route_Create_WithParentAndChild_ReturnsRouteOfLength2()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route = Route.Create(child);

        Assert.Equal(expected: 2, route.Count);
    }

    [Fact]
    public void Route_Target_IsDeepestVisual()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route = Route.Create(child);

        Assert.Same(child, route.Target);
    }

    [Fact]
    public void Route_Root_IsAncestorVisual()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route = Route.Create(child);

        Assert.Same(parent, route.Root);
    }

    [Fact]
    public void Route_GetFromTop_IndexZero_ReturnsRoot()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route = Route.Create(child);

        Assert.Same(parent, route.GetFromTop(0));
    }

    [Fact]
    public void Route_GetFromTop_LastIndex_ReturnsTarget()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route = Route.Create(child);

        Assert.Same(child, route.GetFromTop(route.Count - 1));
    }

    [Fact]
    public void Route_GetFromBottom_IndexZero_ReturnsTarget()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route = Route.Create(child);

        Assert.Same(child, route.GetFromBottom(0));
    }

    [Fact]
    public void Route_GetFromBottom_LastIndex_ReturnsRoot()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route = Route.Create(child);

        Assert.Same(parent, route.GetFromBottom(route.Count - 1));
    }

    [Fact]
    public void Route_FindFirstDifferenceFromTop_IdenticalRoutes_ReturnsLength()
    {
        MockVisual parent = new();
        MockVisual child = new();
        parent.SetChildVisual(child);

        using var route1 = Route.Create(child);
        using var route2 = Route.Create(child);

        Int32 diff = Route.FindFirstDifferenceFromTop(route1, route2);

        Assert.Equal(expected: route1.Count, diff);
    }

    [Fact]
    public void Route_FindFirstDifferenceFromTop_DifferentRoots_ReturnsZero()
    {
        MockVisual root1 = new();
        MockVisual root2 = new();
        MockVisual child1 = new();
        MockVisual child2 = new();
        root1.SetChildVisual(child1);
        root2.SetChildVisual(child2);

        using var route1 = Route.Create(child1);
        using var route2 = Route.Create(child2);

        Int32 diff = Route.FindFirstDifferenceFromTop(route1, route2);

        Assert.Equal(expected: 0, diff);
    }

    [Fact]
    public void Route_FindFirstDifferenceFromTop_SharedRoot_DifferentChildren_ReturnsOne()
    {
        MockVisual root = new();
        MockVisual child1 = new();
        MockVisual child2 = new();
        root.AddChildVisual(child1);
        root.AddChildVisual(child2);

        using var route1 = Route.Create(child1);
        using var route2 = Route.Create(child2);

        Int32 diff = Route.FindFirstDifferenceFromTop(route1, route2);

        Assert.Equal(expected: 1, diff);
    }

    [Fact]
    public void Route_FindFirstDifferenceFromTop_OneRouteEmpty_ReturnsZero()
    {
        MockVisual visual = new();

        using var nonEmpty = Route.Create(visual);
        using var empty = Route.Empty;

        Int32 diff = Route.FindFirstDifferenceFromTop(nonEmpty, empty);

        Assert.Equal(expected: 0, diff);
    }
}
