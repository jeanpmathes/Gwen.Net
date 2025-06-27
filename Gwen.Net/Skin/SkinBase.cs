using System;
using System.Diagnostics;
using Gwen.Net.Control;
using Gwen.Net.Control.Internal;
using Gwen.Net.Renderer;

namespace Gwen.Net.Skin
{
    /// <summary>
    ///     Base skin.
    /// </summary>
    public class SkinBase : IDisposable
    {
        protected readonly RendererBase renderer;

        /// <summary>
        ///     Colors of various UI elements.
        /// </summary>
        internal SkinColors colors;

        protected Int32 baseUnit;
        protected Font defaultFont;

        public Color ModalBackground => colors.modalBackground;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SkinBase" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        protected SkinBase(RendererBase renderer)
        {
            this.renderer = renderer;

            DefaultFont = new Font(renderer);
        }

        /// <summary>
        ///     Default font to use when rendering text if none specified.
        /// </summary>
        public Font DefaultFont
        {
            get => defaultFont;
            set
            {
                if (defaultFont != null)
                {
                    defaultFont.Dispose();
                }

                defaultFont = value;

                baseUnit = Util.Ceil(defaultFont.FontMetrics.EmHeightPixels) + 1;
            }
        }

        /// <summary>
        ///     Base measurement unit based on default font size used in various controls where absolute scale is necessary.
        /// </summary>
        public Int32 BaseUnit => baseUnit;

        /// <summary>
        ///     Renderer used.
        /// </summary>
        public RendererBase Renderer => renderer;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            defaultFont.Dispose();
            GC.SuppressFinalize(this);
        }
        
        ~SkinBase()
        {
            Debug.Print($"IDisposable object finalized: {GetType()}");
        }

        /// <summary>
        ///     Releases the specified font.
        /// </summary>
        /// <param name="font">Font to release.</param>
        protected virtual void ReleaseFont(Font font)
        {
            if (font == null)
            {
                return;
            }

            if (renderer == null)
            {
                return;
            }

            renderer.FreeFont(font);
        }

        /// <summary>
        ///     Sets the default text font.
        /// </summary>
        /// <param name="faceName">Font name. Meaning can vary depending on the renderer.</param>
        /// <param name="size">Font size.</param>
        public virtual void SetDefaultFont(String faceName, Int32 size = 10)
        {
            defaultFont.FaceName = faceName;
            defaultFont.Size = size;
        }

        #region UI elements

        public virtual void DrawButton(ControlBase control, Boolean depressed, Boolean hovered, Boolean disabled) {}
        public virtual void DrawTabButton(ControlBase control, Boolean active, Dock dir) {}
        public virtual void DrawTabControl(ControlBase control) {}
        public virtual void DrawTabTitleBar(ControlBase control) {}
        public virtual void DrawMenuItem(ControlBase control, Boolean submenuOpen, Boolean isChecked) {}
        public virtual void DrawMenuRightArrow(ControlBase control) {}
        public virtual void DrawMenuStrip(ControlBase control) {}
        public virtual void DrawMenu(ControlBase control, Boolean paddingDisabled) {}
        public virtual void DrawRadioButton(ControlBase control, Boolean selected, Boolean depressed) {}
        public virtual void DrawCheckBox(ControlBase control, Boolean selected, Boolean depressed) {}
        public virtual void DrawGroupBox(ControlBase control, Int32 textStart, Int32 textHeight, Int32 textWidth) {}
        public virtual void DrawTextBox(ControlBase control) {}
        public virtual void DrawWindow(ControlBase control, Int32 topHeight, Boolean inFocus) {}
        public virtual void DrawWindowCloseButton(ControlBase control, Boolean depressed, Boolean hovered, Boolean disabled) {}
        public virtual void DrawToolWindow(ControlBase control, Boolean vertical, Int32 dragSize) {}
        public virtual void DrawHighlight(ControlBase control) {}
        public virtual void DrawStatusBar(ControlBase control) {}
        public virtual void DrawShadow(ControlBase control) {}
        public virtual void DrawScrollBarBar(ControlBase control, Boolean depressed, Boolean hovered, Boolean horizontal) {}
        public virtual void DrawScrollBar(ControlBase control, Boolean horizontal, Boolean depressed) {}

        public virtual void DrawScrollButton(ControlBase control, ScrollBarButtonDirection direction, Boolean depressed,
            Boolean hovered, Boolean disabled) {}

        public virtual void DrawProgressBar(ControlBase control, Boolean horizontal, Single progress) {}
        public virtual void DrawTableLine(ControlBase control, Boolean even) {}
        public virtual void DrawListBox(ControlBase control) {}
        public virtual void DrawListBoxLine(ControlBase control, Boolean selected, Boolean even) {}
        public virtual void DrawSlider(ControlBase control, Boolean horizontal, Int32 numNotches, Int32 barSize) {}
        public virtual void DrawSliderButton(ControlBase control, Boolean depressed, Boolean horizontal) {}
        public virtual void DrawComboBox(ControlBase control, Boolean down, Boolean isMenuOpen) {}

        public virtual void DrawComboBoxArrow(ControlBase control, Boolean hovered, Boolean depressed, Boolean open,
            Boolean disabled) {}

        public virtual void DrawKeyboardHighlight(ControlBase control, Rectangle rect, Int32 offset) {}
        public virtual void DrawToolTip(ControlBase control) {}
        public virtual void DrawNumericUpDownButton(ControlBase control, Boolean depressed, Boolean up) {}
        public virtual void DrawTreeButton(ControlBase control, Boolean open) {}
        public virtual void DrawTreeControl(ControlBase control) {}
        public virtual void DrawBorder(ControlBase control, BorderType borderType) {}

        public virtual void DrawDebugOutlines(ControlBase control)
        {
            renderer.DrawColor = control.PaddingOutlineColor;
            Rectangle inner = control.Bounds;
            inner.Deflate(control.Padding);
            renderer.DrawLinedRect(inner);

            renderer.DrawColor = control.MarginOutlineColor;
            Rectangle outer = control.Bounds;
            outer.Inflate(control.Margin);
            renderer.DrawLinedRect(outer);

            renderer.DrawColor = control.BoundsOutlineColor;
            renderer.DrawLinedRect(control.Bounds);
        }

        public virtual void DrawTreeNode(ControlBase ctrl, Boolean open, Boolean selected, Int32 labelHeight, Int32 labelWidth,
            Int32 halfWay, Int32 lastBranch, Boolean isRoot, Int32 indent)
        {
            Renderer.DrawColor = colors.treeColors.lines;

            if (!isRoot)
            {
                Renderer.DrawFilledRect(new Rectangle(indent / 2, halfWay, indent / 2, height: 1));
            }

            if (!open)
            {
                return;
            }

            Renderer.DrawFilledRect(
                new Rectangle(indent + (indent / 2), labelHeight + 1, width: 1, lastBranch + halfWay - labelHeight));
        }

        public virtual void DrawPropertyRow(ControlBase control, Int32 iWidth, Boolean bBeingEdited, Boolean hovered)
        {
            Rectangle rect = control.RenderBounds;

            if (bBeingEdited)
            {
                renderer.DrawColor = colors.propertiesColors.columnSelected;
            }
            else if (hovered)
            {
                renderer.DrawColor = colors.propertiesColors.columnHover;
            }
            else
            {
                renderer.DrawColor = colors.propertiesColors.columnNormal;
            }

            renderer.DrawFilledRect(new Rectangle(x: 0, rect.Y, iWidth, rect.Height));

            if (bBeingEdited)
            {
                renderer.DrawColor = colors.propertiesColors.lineSelected;
            }
            else if (hovered)
            {
                renderer.DrawColor = colors.propertiesColors.lineHover;
            }
            else
            {
                renderer.DrawColor = colors.propertiesColors.lineNormal;
            }

            renderer.DrawFilledRect(new Rectangle(iWidth, rect.Y, width: 1, rect.Height));

            rect.Y += rect.Height - 1;
            rect.Height = 1;

            renderer.DrawFilledRect(rect);
        }

        public virtual void DrawColorDisplay(ControlBase control, Color color) {}
        public virtual void DrawModalControl(ControlBase control, Color? backgroundColor) {}
        public virtual void DrawMenuDivider(ControlBase control) {}
        public virtual void DrawCategoryHolder(ControlBase control) {}
        public virtual void DrawCategoryInner(ControlBase control, Int32 headerHeight, Boolean collapsed) {}

        public virtual void DrawPropertyTreeNode(ControlBase control, Int32 borderLeft, Int32 borderTop)
        {
            Rectangle rect = control.RenderBounds;

            renderer.DrawColor = colors.propertiesColors.border;

            renderer.DrawFilledRect(new Rectangle(rect.X, rect.Y, borderLeft, rect.Height));
            renderer.DrawFilledRect(new Rectangle(rect.X + borderLeft, rect.Y, rect.Width - borderLeft, borderTop));
        }

        public virtual void DrawSeparator(ControlBase control, Int32 textStart, Int32 textWidth)
        {
            Rectangle rect = control.RenderBounds;
            
            rect.Y += rect.Height / 2;

            // The same color as for the group box.
            renderer.DrawColor = new Color(a: 171, r: 205, g: 214, b: 216);
            
            if (textWidth == 0)
            {
                renderer.DrawFilledRect(new Rectangle(rect.X, rect.Y, rect.Width, height: 1));
            }
            else
            {
                renderer.DrawFilledRect(new Rectangle(rect.X, rect.Y, textStart - 2, height: 1));
                renderer.DrawFilledRect(new Rectangle(textStart + textWidth + 2, rect.Y, rect.Width - textStart - textWidth - 2, height: 1));
            }
        }

        #endregion

        #region Symbols for Simple skin

        /*
		Here we're drawing a few symbols such as the directional arrows and the checkbox check

		Texture'd skins don't generally use these - but the Simple skin does. We did originally
		use the marlett font to draw these.. but since that's a Windows font it wasn't a very
		good cross platform solution.
		*/

        public virtual void DrawArrowDown(Rectangle rect)
        {
            Single x = rect.Width / 5.0f;
            Single y = rect.Height / 5.0f;

            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 0.0f), rect.Y + (y * 1.0f), x, y * 1.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 1.0f), x, y * 2.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 1.0f), x, y * 3.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 1.0f), x, y * 2.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 4.0f), rect.Y + (y * 1.0f), x, y * 1.0f));
        }

        public virtual void DrawArrowUp(Rectangle rect)
        {
            Single x = rect.Width / 5.0f;
            Single y = rect.Height / 5.0f;

            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 0.0f), rect.Y + (y * 3.0f), x, y * 1.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 2.0f), x, y * 2.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 1.0f), x, y * 3.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 2.0f), x, y * 2.0f));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 4.0f), rect.Y + (y * 3.0f), x, y * 1.0f));
        }

        public virtual void DrawArrowLeft(Rectangle rect)
        {
            Single x = rect.Width / 5.0f;
            Single y = rect.Height / 5.0f;

            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 0.0f), x * 1.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 1.0f), x * 2.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 2.0f), x * 3.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 3.0f), x * 2.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 4.0f), x * 1.0f, y));
        }

        public virtual void DrawArrowRight(Rectangle rect)
        {
            Single x = rect.Width / 5.0f;
            Single y = rect.Height / 5.0f;

            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 0.0f), x * 1.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 1.0f), x * 2.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 2.0f), x * 3.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 3.0f), x * 2.0f, y));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 4.0f), x * 1.0f, y));
        }

        public virtual void DrawCheck(Rectangle rect)
        {
            Single x = rect.Width / 5.0f;
            Single y = rect.Height / 5.0f;

            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 0.0f), rect.Y + (y * 3.0f), x * 2, y * 2));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 4.0f), x * 2, y * 2));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 3.0f), x * 2, y * 2));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 1.0f), x * 2, y * 2));
            renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 4.0f), rect.Y + (y * 0.0f), x * 2, y * 2));
        }

        #endregion
    }
}
