using Gwen.Net.New.Controls.Bases;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.Tests.Unit.New.Visuals;

namespace Gwen.Net.Tests.Unit.New.Controls;

public class SingleChildControlTests
{
    [Fact]
    public void Child_IsNull_ByDefault()
    {
        var control = new MockControl();
        
        Assert.Null(control.Child);
        Assert.Empty(control.Children);
    }
    
    [Fact]
    public void Child_ReplacesPreviousChild()
    {
        var control = new MockControl();
        var firstChild = new MockControl();
        var secondChild = new MockControl();
        
        control.Child = firstChild;
        Assert.Equal(firstChild, control.Child);
        Assert.Single(control.Children);
        
        control.Child = secondChild;
        Assert.Equal(secondChild, control.Child);
        Assert.Single(control.Children);
    }
    
    private class MockControl : SingleChildControl<MockControl>
    {
        protected override ControlTemplate<MockControl> CreateDefaultTemplate()
        {
            return ControlTemplate.Create<MockControl>(_ => new MockVisual());
        }
    }
}
