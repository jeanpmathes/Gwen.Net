using Gwen.Net.New.Bindings;
using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Graphics;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// The base class of all GWEN controls, meaning logical elements.
/// Controls can be templated and styled.
/// </summary>
public abstract class Control : Element
{
    #region PROPERTIES

    /// <summary>
    /// Creates a new instance of the <see cref="Control"/> class.
    /// </summary>
    protected Control()
    {
        Foreground = Properties.Create(this, Brushes.Black);
        Background = Properties.Create(this, Brushes.Transparent);
    }
    
    /// <summary>
    /// The preferred foreground brush of the control.
    /// </summary>
    public Property<Brush> Foreground { get; }
    
    /// <summary>
    /// The preferred background brush of the control.
    /// </summary>
    public Property<Brush> Background { get; }
    
    #endregion PROPERTIES
}

/// <summary>
/// The generic base class of all GWEN controls, meaning logical elements.
/// The generic variant is needed for templating.
/// </summary>
/// <typeparam name="TSelf">The type of the control itself.</typeparam>
public abstract class Control<TSelf> : Control where TSelf : Control<TSelf>
{
    #region TEMPLATING

    /// <summary>
    /// Creates a new instance of the <see cref="Control{TSelf}"/> class.
    /// </summary>
    protected Control()
    {
        Template = Properties.Create(this, Binding.Computed(CreateDefaultTemplate), Invalidation.Visualization);
    }
    
    /// <summary>
    /// The template used to visualize this control.
    /// </summary>
    public Property<ControlTemplate<TSelf>> Template { get; }
    
    private protected override VisualElement GetOrCreateVisualization()
    {
        VisualElement visualization = ApplyControlTemplate();
        
        SetTemplateOwner(visualization);
        
        return visualization;

        void SetTemplateOwner(VisualElement element)
        {
            if (element.TemplateOwner.GetValue() != null)
                return;
            
            element.TemplateOwnerInternal.SetValue(this);
            
            foreach (VisualElement child in element.VisualChildren)
                SetTemplateOwner(child);
        }
    }
    
    private VisualElement ApplyControlTemplate()
    {
        ControlTemplate<TSelf> template = Template;

        return template.Apply((TSelf)this);
    }
    
    /// <summary>
    /// Create a default template for this control, which is used if no style or local template is set.
    /// </summary>
    /// <returns>The default control template.</returns>
    protected abstract ControlTemplate<TSelf> CreateDefaultTemplate();
    
    #endregion TEMPLATING
}
