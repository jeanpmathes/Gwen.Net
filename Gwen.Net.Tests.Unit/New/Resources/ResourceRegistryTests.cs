using Gwen.Net.New.Resources;
using Gwen.Net.New.Styles;
using Gwen.Net.New.Themes;
using Gwen.Net.Tests.Unit.New.Controls;

namespace Gwen.Net.Tests.Unit.New.Resources;

public class ResourceRegistryTests
{
    [Fact]
    public void ResourceRegistry_AddStyle_AddsStylesToRegistry()
    {
        ResourceRegistry registry = new();

        Style style1 = registry.AddStyle<MockControl>(_ => {});
        Style style2 = registry.AddStyle<MockControl>(_ => {});
        
        Assert.NotNull(style1);
        Assert.NotNull(style2);
        Assert.NotSame(style1, style2);
        
        Assert.Contains(style1, registry.Styles);
        Assert.Contains(style2, registry.Styles);
    }
    
    [Fact]
    public void ResourceRegistry_AddBundle_AddsAllStylesToRegistry()
    {
        ResourceRegistry registry = new();
        Assert.Empty(registry.Styles);

        registry.AddBundle<ClassicLight>();
        Assert.NotEmpty(registry.Styles);
        
        Int32 count = registry.Styles.Count;
        registry.AddBundle<ClassicDark>();
        Assert.True(registry.Styles.Count > count);
    }
}
