using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    public enum BorderType
    {
        ToolTip,
        StatusBar,
        MenuStrip,
        Selection,
        PanelNormal,
        PanelBright,
        PanelDark,
        PanelHighlight,
        ListBox,
        TreeControl,
        CategoryList
    }

    [XmlControl]
    public class Border : ControlBase
    {
        private BorderType m_BorderType;

        public Border(ControlBase parent)
            : base(parent)
        {
            m_BorderType = BorderType.PanelNormal;
        }

        [XmlProperty] public BorderType BorderType
        {
            get => m_BorderType;
            set
            {
                if (m_BorderType == value)
                {
                    return;
                }

                m_BorderType = value;
            }
        }

        protected override void Render(SkinBase skin)
        {
            skin.DrawBorder(this, m_BorderType);
        }
    }
}