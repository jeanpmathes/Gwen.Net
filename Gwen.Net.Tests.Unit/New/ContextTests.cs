using Gwen.Net.New;
using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Styles;
using Gwen.Net.Tests.Unit.New.Controls;

namespace Gwen.Net.Tests.Unit.New;

public class ContextTests
{
    [Fact]
    public void GetStyling_ReturnsBaseStyleBeforeDerivedStyle()
    {
        Style style1 = null!;
        Style style2 = null!;
        
        var context = Context.Create(registry =>
        {
            style1 = registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 10f));
            style2 = registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
        });

        IReadOnlyList<IStyle<MockControl>> styles = context.GetStyling<MockControl>();

        Assert.Equal(expected: 2, styles.Count);
        Assert.Same(style1, styles[0]);
        Assert.Same(style2, styles[1]);
    }

    [Fact]
    public void GetStyling_UsesParentStylesWhenChildContextIsEmpty()
    {
        Style style = null!;
        
        var parentContext = Context.Create(registry =>
        {
            style = registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
        });

        var childContext = Context.Default;
        
        var context = new Context(childContext, parentContext);
        IReadOnlyList<IStyle<MockControl>> styles = context.GetStyling<MockControl>();
        
        Assert.Single(styles);
        Assert.Same(style, styles[0]);
    }

    [Fact]
    public void GetStyling_ChildControlStyleOverridesParentChain()
    {
        Style parentStyle = null!;
        
        var parentContext = Context.Create(registry =>
        {
            registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 10f));
            parentStyle = registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
        });
        
        Style childStyle = null!;
        
        var childContext = Context.Create(registry =>
        {
            childStyle = registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 30f));
        });
        
        var context = new Context(childContext, parentContext);
        IReadOnlyList<IStyle<MockControl>> styles = context.GetStyling<MockControl>();
        
        Assert.Equal(expected: 2, styles.Count);
        Assert.Same(childStyle, styles[0]);
        Assert.Same(parentStyle, styles[1]);
    }

    [Fact]
    public void GetStyling_ChildMockControlStyleOverridesParentChain()
    {
        Style parentStyle1 = null!;

        var parentContext = Context.Create(registry =>
        {
            parentStyle1 = registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 10f));
            registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
        });
        
        Style childStyle = null!;
        
        var childContext = Context.Create(registry =>
        {
            childStyle = registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 30f));
        });
        
        var context = new Context(childContext, parentContext);
        IReadOnlyList<IStyle<MockControl>> styles = context.GetStyling<MockControl>();
        
        Assert.Equal(expected: 2, styles.Count);
        Assert.Same(parentStyle1, styles[0]);
        Assert.Same(childStyle, styles[1]);
    }
    
    [Fact]
    public void GetContentTemplate_ReturnsRegisteredTemplate()
    {
        ContentTemplate<String>? template = null;
        
        var context = Context.Create(registry =>
        {
            template = registry.AddContentTemplate<String>(_ => new Border());
        });

        IContentTemplate<String> result = context.GetContentTemplate<String>();

        Assert.Same(template, result);
    }
    
    [Fact]
    public void GetContentTemplate_DoesNotReturnTemplateOfMoreGeneralType()
    {
        var context = Context.Create(registry =>
        {
            registry.AddContentTemplate<Object>(_ => new Border());
        });

        IContentTemplate<String> result = context.GetContentTemplate<String>();

        Assert.Same(ContentTemplate.Default, result);
    }

    [Fact]
    public void GetContentTemplate_UsesParentContextWhenChildContextIsEmpty()
    {
        ContentTemplate<String>? template = null;
        
        var parentContext = Context.Create(registry =>
        {
            template = registry.AddContentTemplate<String>(_ => new Border());
        });

        var childContext = Context.Default;
        
        var context = new Context(childContext, parentContext);
        IContentTemplate<String> result = context.GetContentTemplate<String>();
        
        Assert.Same(template, result);
    }
}
