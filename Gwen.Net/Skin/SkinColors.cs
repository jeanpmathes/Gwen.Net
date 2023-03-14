namespace Gwen.Net.Skin
{
    /// <summary>
    ///     UI colors used by skins.
    /// </summary>
    internal struct SkinColors
    {
        public struct WindowColors
        {
            public Color titleActive;
            public Color titleInactive;
        }

        public struct ButtonColors
        {
            public Color normal;
            public Color hover;
            public Color down;
            public Color disabled;
        }

        public struct TabColors
        {
            public struct InactiveColors
            {
                public Color normal;
                public Color hover;
                public Color down;
                public Color disabled;
            }

            public struct ActiveColors
            {
                public Color normal;
                public Color hover;
                public Color down;
                public Color disabled;
            }

            public InactiveColors inactiveColors;
            public ActiveColors activeColors;
        }

        public struct LabelColors
        {
            public Color @default;
            public Color bright;
            public Color dark;
            public Color highlight;
        }

        public struct TextBoxColors
        {
            public Color text;
            public Color backgroundSelected;
            public Color caret;
        }

        public struct ListBoxColors
        {
            public Color textNormal;
            public Color textSelected;
        }

        public struct TreeColors
        {
            public Color lines;
            public Color normal;
            public Color hover;
            public Color selected;
        }

        public struct PropertiesColors
        {
            public Color lineNormal;
            public Color lineSelected;
            public Color lineHover;
            public Color columnNormal;
            public Color columnSelected;
            public Color columnHover;
            public Color labelNormal;
            public Color labelSelected;
            public Color labelHover;
            public Color border;
            public Color title;
        }

        public struct CategoryColors
        {
            public Color header;
            public Color headerClosed;

            public struct LineColors
            {
                public Color text;
                public Color textHover;
                public Color textSelected;
                public Color button;
                public Color buttonHover;
                public Color buttonSelected;
            }

            public struct LineAltColors
            {
                public Color text;
                public Color textHover;
                public Color textSelected;
                public Color button;
                public Color buttonHover;
                public Color buttonSelected;
            }

            public LineColors lineColors;
            public LineAltColors lineAltColors;
        }

        public Color modalBackground;
        public Color tooltipText;

        public WindowColors windowColors;
        public ButtonColors buttonColors;
        public TabColors tabColors;
        public LabelColors labelColors;
        public TextBoxColors textBoxColors;
        public ListBoxColors listBoxColors;
        public TreeColors treeColors;
        public PropertiesColors propertiesColors;
        public CategoryColors categoryColors;
    }
}
