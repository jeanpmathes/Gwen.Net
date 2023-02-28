using System;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     List box row (selectable).
    /// </summary>
    [XmlControl(CustomHandler = "XmlElementHandler")]
    public class ListBoxRow : TableRow
    {
        private bool selected;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ListBoxRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public ListBoxRow(ControlBase parent)
            : base(parent)
        {
            ListBox = parent as ListBox;

            MouseInputEnabled = true;
            IsSelected = false;
        }

        public ListBox ListBox { get; }
        
        private Color? selectedColorOverride;
        private Color? normalColorOverride;

        public Color? SelectedTextOverride
        {
            get => selectedColorOverride;
            set
            {
                selectedColorOverride = value;
                UpdateTextColor();
            }
        }

        public Color? NormalTextOverride
        {
            get => normalColorOverride;
            set
            {
                normalColorOverride = value;
                UpdateTextColor();
            }
        }
        
        /// <summary>
        ///     Indicates whether the control is selected.
        /// </summary>
        public bool IsSelected
        {
            get => selected;
            set
            {
                selected = value;
                UpdateTextColor();
            }
        }

        private void UpdateTextColor()
        {
            SetTextColor(IsSelected 
                ? SelectedTextOverride ?? Skin.Colors.ListBox.Text_Selected 
                : NormalTextOverride ?? Skin.Colors.ListBox.Text_Normal);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawListBoxLine(this, IsSelected, EvenRow);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(int x, int y, bool down)
        {
            base.OnMouseClickedLeft(x, y, down);

            if (down)
            {
                //IsSelected = true; // [omeg] ListBox manages that
                OnRowSelected();
            }
        }

        internal static ControlBase XmlElementHandler(Parser parser, Type type, ControlBase parent)
        {
            ListBoxRow element = new(parent);
            parser.ParseAttributes(element);

            if (parser.MoveToContent())
            {
                var colIndex = 1;

                foreach (string elementName in parser.NextElement())
                {
                    if (elementName == "Column")
                    {
                        if (parser.MoveToContent())
                        {
                            ControlBase column = parser.ParseElement(element);
                            element.SetCellContents(colIndex++, column, enableMouseInput: true);
                        }
                        else
                        {
                            string colText = parser.GetAttribute("Text");
                            element.SetCellText(colIndex++, colText ?? string.Empty);
                        }
                    }
                }
            }

            return element;
        }
    }
}
