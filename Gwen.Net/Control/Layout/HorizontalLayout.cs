using Gwen.Net.Xml;

namespace Gwen.Net.Control.Layout
{
    /// <summary>
    ///     Arrange child controls into a row.
    /// </summary>
    [XmlControl]
    public class HorizontalLayout : StackLayout
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HorizontalLayout" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalLayout(ControlBase parent)
            : base(parent)
        {
            Horizontal = true;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Stretch;
        }
    }
}