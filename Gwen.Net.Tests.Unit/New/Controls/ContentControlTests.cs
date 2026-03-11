using Gwen.Net.New.Controls;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Resources;
using Gwen.Net.Tests.Unit.New.Rendering;

namespace Gwen.Net.Tests.Unit.New.Controls;

public class ContentControlTests() : ControlTestBase<ContentControl<Object>>(() => new ContentControl<Object>())
{
    [Fact]
    public void ContentControl_WithNullContent_HasNoChild()
    {
        Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        ContentControl<Object> control = new ContentControl<Object>();
        canvas.Child = control;
        
        Assert.Empty(control.Children);
    }

    [Fact]
    public void ContentControl_WithLocalTemplate_UsesLocalTemplate()
    {
        Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        
        const String contentString = "test content";
        String? templatedContent = null;
        ContentTemplate<String> template = ContentTemplate.Create<String>(content =>
        {
            templatedContent = content;
            return new MockControl();
        });

        ContentControl<String> control = new ContentControl<String>
        {
            Content = {Value = contentString},
            ContentTemplate = {Value = template}
        };
        canvas.Child = control;
        
        Assert.Equal(contentString, templatedContent);
        
        Assert.Single(control.Children);
        Assert.IsType<MockControl>(control.Children[0]);
    }

    [Fact]
    public void ContentControl_WithContextTemplate_UsesContextTemplate()
    {
        ResourceRegistry registry = new ResourceRegistry();
        
        const String contentString = "test content";
        String? templatedContent = null;
        registry.AddContentTemplate<String>(content =>
        {
            templatedContent = content;
            return new MockControl();
        });
        
        Canvas canvas = Canvas.Create(new MockRenderer(), registry);
        ContentControl<String> control = new ContentControl<String>
        {
            Content = {Value = contentString}
        };
        canvas.Child = control;
        
        Assert.Equal(contentString, templatedContent);
        
        Assert.Single(control.Children);
        Assert.IsType<MockControl>(control.Children[0]);
    }

    [Fact]
    public void ContentControl_LocalTemplateOverridesContextTemplate()
    {
        ResourceRegistry registry = new ResourceRegistry();
        
        registry.AddContentTemplate<String>(_ => new MockControl("no tok"));
        
        Canvas canvas = Canvas.Create(new MockRenderer(), registry);
        
        const String okTag = "ok";
        ContentControl<Object> control = new ContentControl<Object>
        {
            Content = {Value = "test content"},
            ContentTemplate = {Value = ContentTemplate.Create<Object>(_ => new MockControl(okTag))}
        };
        canvas.Child = control;
        
        Assert.Single(control.Children);
        MockControl mockControl = Assert.IsType<MockControl>(control.Children[0]);
        Assert.Equal(okTag, mockControl.Tag);
    }

    [Fact]
    public void ContentControl_ChangingContent_UpdatesChild()
    {
        Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());
        
        const String firstContent = "first";
        const String secondContent = "second";
        
        ContentControl<String> control = new ContentControl<String>
        {
            ContentTemplate = {Value = ContentTemplate.Create<String>(content => new MockControl(content)) }
        };
        canvas.Child = control;
        
        control.Content.Value = firstContent;
        
        Assert.Single(control.Children);
        MockControl mockControl = Assert.IsType<MockControl>(control.Children[0]);
        Assert.Equal(firstContent, mockControl.Tag);
        
        control.Content.Value = secondContent;
        Assert.Single(control.Children);
        mockControl = Assert.IsType<MockControl>(control.Children[0]);
        Assert.Equal(secondContent, mockControl.Tag);

        control.Content.Value = null;
        Assert.Empty(control.Children);
    }

    [Fact]
    public void ContentControl_WithStringContent_HasTextElement()
    {
        ResourceRegistry registry = new ResourceRegistry();
        registry.AddContentTemplate<String>(content => new Text { Content = { Value = content } });

        Canvas canvas = Canvas.Create(new MockRenderer(), registry);
        ContentControl<String> control = new ContentControl<String>
        {
            Content = { Value = "hello" }
        };
        canvas.Child = control;

        Assert.Single(control.Children);
        Assert.IsType<Text>(control.Children[0]);
    }

    [Fact]
    public void ContentControl_ChangingContentTemplate_UpdatesChild()
    {
        Canvas canvas = Canvas.Create(new MockRenderer(), new ResourceRegistry());

        const String firstContent = "first";
        const String secondContent = "second";

        ContentControl<String> control = new ContentControl<String>
        {
            Content = {Value = "test content"},
            ContentTemplate = {Value = ContentTemplate.Create<String>(_ => new MockControl(firstContent)) }
        };
        
        canvas.Child = control;
        
        Assert.Single(control.Children);
        MockControl mockControl = Assert.IsType<MockControl>(control.Children[0]);
        Assert.Equal(firstContent, mockControl.Tag);
        
        control.ContentTemplate.Value = ContentTemplate.Create<String>(_ => new MockControl(secondContent));
        Assert.Single(control.Children);
        mockControl = Assert.IsType<MockControl>(control.Children[0]);
        Assert.Equal(secondContent, mockControl.Tag);
    }
}
