using Gwen.Net.Skin;

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
    
    public class Border : ControlBase
    {
        public Border(ControlBase parent)
            : base(parent)
        {
            BorderType = BorderType.PanelNormal;
        }

        public BorderType BorderType { get; set; }

        protected override void Render(SkinBase skin)
        {
            skin.DrawBorder(this, BorderType);
        }
    }
}
