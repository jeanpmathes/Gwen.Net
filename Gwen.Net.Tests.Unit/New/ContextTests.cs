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

        Context context = Context.Create(registry =>
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

        Context parentContext = Context.Create(registry =>
        {
            style = registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
        });

        Context childContext = Context.Default;

        Context context = new(childContext, parentContext);
        IReadOnlyList<IStyle<MockControl>> styles = context.GetStyling<MockControl>();

        Assert.Single(styles);
        Assert.Same(style, styles[0]);
    }

    [Fact]
    public void GetStyling_ChildControlStyleOverridesParentChain()
    {
        Style parentStyle = null!;

        Context parentContext = Context.Create(registry =>
        {
            registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 10f));
            parentStyle = registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
        });

        Style childStyle = null!;

        Context childContext = Context.Create(registry =>
        {
            childStyle = registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 30f));
        });

        Context context = new(childContext, parentContext);
        IReadOnlyList<IStyle<MockControl>> styles = context.GetStyling<MockControl>();

        Assert.Equal(expected: 2, styles.Count);
        Assert.Same(childStyle, styles[0]);
        Assert.Same(parentStyle, styles[1]);
    }

    [Fact]
    public void GetStyling_ChildMockControlStyleOverridesParentChain()
    {
        Style parentStyle1 = null!;

        Context parentContext = Context.Create(registry =>
        {
            parentStyle1 = registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 10f));
            registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
        });

        Style childStyle = null!;

        Context childContext = Context.Create(registry =>
        {
            childStyle = registry.AddStyle<MockControl>(builder => builder.Set(c => c.MinimumWidth, value: 30f));
        });

        Context context = new(childContext, parentContext);
        IReadOnlyList<IStyle<MockControl>> styles = context.GetStyling<MockControl>();

        Assert.Equal(expected: 2, styles.Count);
        Assert.Same(parentStyle1, styles[0]);
        Assert.Same(childStyle, styles[1]);
    }

    [Fact]
    public void GetStyling_ReturnsStyleForImplementedInterface()
    {
        Style interfaceStyle = null!;

        Context context = Context.Create(registry =>
        {
            interfaceStyle = registry.AddStyle<IButton>(builder => builder.Set(c => c.MinimumWidth, value: 15f));
        });

        IReadOnlyList<IStyle<Button<String>>> styles = context.GetStyling<Button<String>>();

        Assert.Single(styles);
        Assert.Same(interfaceStyle, styles[0]);
    }

    [Fact]
    public void GetStyling_ReturnsStyleForInheritedInterface()
    {
        Style inheritedInterfaceStyle = null!;

        Context context = Context.Create(registry =>
        {
            inheritedInterfaceStyle = registry.AddStyle<IContentControl>(builder => builder.Set(c => c.MinimumWidth, value: 15f));
        });

        IReadOnlyList<IStyle<Button<String>>> styles = context.GetStyling<Button<String>>();

        Assert.Single(styles);
        Assert.Same(inheritedInterfaceStyle, styles[0]);
    }

    [Fact]
    public void GetStyling_OrdersInterfacesBeforeClassStyles()
    {
        Style iControlStyle = null!;
        Style controlStyle = null!;
        Style iButtonStyle = null!;

        Context context = Context.Create(registry =>
        {
            iControlStyle = registry.AddStyle<IControl>(builder => builder.Set(c => c.MinimumWidth, value: 10f));
            controlStyle = registry.AddStyle<Control>(builder => builder.Set(c => c.MinimumWidth, value: 20f));
            iButtonStyle = registry.AddStyle<IButton>(builder => builder.Set(c => c.MinimumWidth, value: 30f));
        });

        IReadOnlyList<IStyle<Button<String>>> styles = context.GetStyling<Button<String>>();

        Assert.Equal(expected: 3, styles.Count);
        Assert.Same(iControlStyle, styles[0]);
        Assert.Same(controlStyle, styles[1]);
        Assert.Same(iButtonStyle, styles[2]);
    }

    [Fact]
    public void GetContentTemplate_ReturnsRegisteredTemplate()
    {
        ContentTemplate<String>? template = null;

        Context context = Context.Create(registry =>
        {
            template = registry.AddContentTemplate<String>(_ => new MockControl());
        });

        IContentTemplate<String> result = context.GetContentTemplate<String>();

        Assert.Same(template, result);
    }

    [Fact]
    public void GetContentTemplate_DoesNotReturnTemplateOfMoreGeneralType()
    {
        Context context = Context.Create(registry =>
        {
            registry.AddContentTemplate<Object>(_ => new MockControl());
        });

        IContentTemplate<String> result = context.GetContentTemplate<String>();

        Assert.Same(ContentTemplate.Default, result);
    }

    [Fact]
    public void GetContentTemplate_UsesParentContextWhenChildContextIsEmpty()
    {
        ContentTemplate<String>? template = null;

        Context parentContext = Context.Create(registry =>
        {
            template = registry.AddContentTemplate<String>(_ => new MockControl());
        });

        Context childContext = Context.Default;

        Context context = new(childContext, parentContext);
        IContentTemplate<String> result = context.GetContentTemplate<String>();

        Assert.Same(template, result);
    }
}
