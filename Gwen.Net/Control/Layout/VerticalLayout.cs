using Gwen.Net.Xml;

namespace Gwen.Net.Control.Layout
{
    /// <summary>
    ///     Arrange child controls into a column.
    /// </summary>
    [XmlControl]
    public class VerticalLayout : StackLayout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VerticalLayout" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public VerticalLayout(ControlBase parent)
            : base(parent)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;
        }
    }
}