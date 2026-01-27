using Gwen.Net.New.Visuals;
using Gwen.Net.New.Visuals.Layout;

namespace Gwen.Net.New.Controls;

/// <summary>
/// The base class of all GWEN controls, meaning logical elements.
/// Controls can be templated and styled.
/// </summary>
public abstract class Control : Element
{
    #region TEMPLATING
    
    private protected override VisualElement GetOrCreateVisualization()
    {
        VisualElement visualization = ApplyControlTemplate();
        
        visualization.TemplateOwner = this;
        
        return visualization;
    }
    
    private VisualElement ApplyControlTemplate()
    {
        return new StackPanel(); // todo: implement proper control visualization
    }
    
    #endregion TEMPLATING
}
