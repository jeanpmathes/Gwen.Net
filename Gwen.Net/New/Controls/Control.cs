using Gwen.Net.New.Controls.Templates;
using Gwen.Net.New.Visuals;

namespace Gwen.Net.New.Controls;

/// <summary>
/// The base class of all GWEN controls, meaning logical elements.
/// Controls can be templated and styled.
/// </summary>
public abstract class Control : Element;

/// <summary>
/// The generic base class of all GWEN controls, meaning logical elements.
/// The generic variant is needed for templating.
/// </summary>
/// <typeparam name="TSelf">The type of the control itself.</typeparam>
public abstract class Control<TSelf> : Control where TSelf : Control<TSelf>
{
    #region TEMPLATING
    
    /// <summary>
    /// The template used to visualize this control.
    /// </summary>
    public ControlTemplate<TSelf> Template
    {
        get
        {
            field ??= CreateDefaultTemplate();
            return field;
        }

        set
        {
            field = value;
            InvalidateVisualization();
        }
    }
    
    private protected override VisualElement GetOrCreateVisualization()
    {
        VisualElement visualization = ApplyControlTemplate();
        
        visualization.TemplateOwner = this;
        
        return visualization;
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
