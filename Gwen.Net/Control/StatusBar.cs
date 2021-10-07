using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Status bar.
    /// </summary>
    [XmlControl]
    public class StatusBar : ControlBase
    {
        private readonly Label m_Label;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatusBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public StatusBar(ControlBase parent) : base(parent)
        {
            Height = BaseUnit + 11;
            Dock = Dock.Bottom;
            Padding = new Padding(left: 6, top: 2, right: 6, bottom: 1);

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Bottom;

            m_Label = new Label(this);
            m_Label.AutoSizeToContents = false;
            m_Label.Alignment = Alignment.Left | Alignment.CenterV;
            m_Label.Dock = Dock.Fill;
        }

        [XmlProperty] public string Text
        {
            get => m_Label.Text;
            set => m_Label.Text = value;
        }

        [XmlProperty] public Color TextColor
        {
            get => m_Label.TextColor;
            set => m_Label.TextColor = value;
        }

        /// <summary>
        ///     Adds a control to the bar.
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="right">Determines whether the control should be added to the right side of the bar.</param>
        public void AddControl(ControlBase control, bool right)
        {
            control.Parent = this;
            control.Dock = right ? Dock.Right : Dock.Left;
            control.VerticalAlignment = VerticalAlignment.Center;
        }

        protected override void OnChildAdded(ControlBase child)
        {
            child.VerticalAlignment = VerticalAlignment.Center;

            if (child.Dock != Dock.Left)
            {
                child.Dock = Dock.Right;
            }

            base.OnChildAdded(child);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawStatusBar(this);
        }
    }
}