using System;
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
        protected readonly RendererBase m_Renderer;

        /// <summary>
        ///     Colors of various UI elements.
        /// </summary>
        public SkinColors Colors;

        protected int m_BaseUnit;
        protected Font m_DefaultFont;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SkinBase" /> class.
        /// </summary>
        /// <param name="renderer">Renderer to use.</param>
        protected SkinBase(RendererBase renderer)
        {
            m_Renderer = renderer;

            DefaultFont = new Font(renderer);
        }

        /// <summary>
        ///     Default font to use when rendering text if none specified.
        /// </summary>
        public Font DefaultFont
        {
            get => m_DefaultFont;
            set
            {
                if (m_DefaultFont != null)
                {
                    m_DefaultFont.Dispose();
                }

                m_DefaultFont = value;

                m_BaseUnit = Util.Ceil(m_DefaultFont.FontMetrics.EmHeightPixels) + 1;
            }
        }

        /// <summary>
        ///     Base measurement unit based on default font size used in various controls where absolute scale is necessary.
        /// </summary>
        public int BaseUnit => m_BaseUnit;

        /// <summary>
        ///     Renderer used.
        /// </summary>
        public RendererBase Renderer => m_Renderer;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            m_DefaultFont.Dispose();
            GC.SuppressFinalize(this);
        }

#if DEBUG
        ~SkinBase()
        {
            throw new InvalidOperationException(String.Format("IDisposable object finalized: {0}", GetType()));
            //Debug.Print(String.Format("IDisposable object finalized: {0}", GetType()));
        }
#endif

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

            if (m_Renderer == null)
            {
                return;
            }

            m_Renderer.FreeFont(font);
        }

        /// <summary>
        ///     Sets the default text font.
        /// </summary>
        /// <param name="faceName">Font name. Meaning can vary depending on the renderer.</param>
        /// <param name="size">Font size.</param>
        public virtual void SetDefaultFont(string faceName, int size = 10)
        {
            m_DefaultFont.FaceName = faceName;
            m_DefaultFont.Size = size;
        }

        #region UI elements

        public virtual void DrawButton(ControlBase control, bool depressed, bool hovered, bool disabled) {}
        public virtual void DrawTabButton(ControlBase control, bool active, Dock dir) {}
        public virtual void DrawTabControl(ControlBase control) {}
        public virtual void DrawTabTitleBar(ControlBase control) {}
        public virtual void DrawMenuItem(ControlBase control, bool submenuOpen, bool isChecked) {}
        public virtual void DrawMenuRightArrow(ControlBase control) {}
        public virtual void DrawMenuStrip(ControlBase control) {}
        public virtual void DrawMenu(ControlBase control, bool paddingDisabled) {}
        public virtual void DrawRadioButton(ControlBase control, bool selected, bool depressed) {}
        public virtual void DrawCheckBox(ControlBase control, bool selected, bool depressed) {}
        public virtual void DrawGroupBox(ControlBase control, int textStart, int textHeight, int textWidth) {}
        public virtual void DrawTextBox(ControlBase control) {}
        public virtual void DrawWindow(ControlBase control, int topHeight, bool inFocus) {}
        public virtual void DrawWindowCloseButton(ControlBase control, bool depressed, bool hovered, bool disabled) {}
        public virtual void DrawToolWindow(ControlBase control, bool vertical, int dragSize) {}
        public virtual void DrawHighlight(ControlBase control) {}
        public virtual void DrawStatusBar(ControlBase control) {}
        public virtual void DrawShadow(ControlBase control) {}
        public virtual void DrawScrollBarBar(ControlBase control, bool depressed, bool hovered, bool horizontal) {}
        public virtual void DrawScrollBar(ControlBase control, bool horizontal, bool depressed) {}

        public virtual void DrawScrollButton(ControlBase control, ScrollBarButtonDirection direction, bool depressed,
            bool hovered, bool disabled) {}

        public virtual void DrawProgressBar(ControlBase control, bool horizontal, float progress) {}
        public virtual void DrawListBox(ControlBase control) {}
        public virtual void DrawListBoxLine(ControlBase control, bool selected, bool even) {}
        public virtual void DrawSlider(ControlBase control, bool horizontal, int numNotches, int barSize) {}
        public virtual void DrawSliderButton(ControlBase control, bool depressed, bool horizontal) {}
        public virtual void DrawComboBox(ControlBase control, bool down, bool isMenuOpen) {}

        public virtual void DrawComboBoxArrow(ControlBase control, bool hovered, bool depressed, bool open,
            bool disabled) {}

        public virtual void DrawKeyboardHighlight(ControlBase control, Rectangle rect, int offset) {}
        public virtual void DrawToolTip(ControlBase control) {}
        public virtual void DrawNumericUpDownButton(ControlBase control, bool depressed, bool up) {}
        public virtual void DrawTreeButton(ControlBase control, bool open) {}
        public virtual void DrawTreeControl(ControlBase control) {}
        public virtual void DrawBorder(ControlBase control, BorderType borderType) {}

        public virtual void DrawDebugOutlines(ControlBase control)
        {
            m_Renderer.DrawColor = control.PaddingOutlineColor;
            Rectangle inner = control.Bounds;
            inner.Deflate(control.Padding);
            m_Renderer.DrawLinedRect(inner);

            m_Renderer.DrawColor = control.MarginOutlineColor;
            Rectangle outer = control.Bounds;
            outer.Inflate(control.Margin);
            m_Renderer.DrawLinedRect(outer);

            m_Renderer.DrawColor = control.BoundsOutlineColor;
            m_Renderer.DrawLinedRect(control.Bounds);
        }

        public virtual void DrawTreeNode(ControlBase ctrl, bool open, bool selected, int labelHeight, int labelWidth,
            int halfWay, int lastBranch, bool isRoot, int indent)
        {
            Renderer.DrawColor = Colors.Tree.Lines;

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

        public virtual void DrawPropertyRow(ControlBase control, int iWidth, bool bBeingEdited, bool hovered)
        {
            Rectangle rect = control.RenderBounds;

            if (bBeingEdited)
            {
                m_Renderer.DrawColor = Colors.Properties.Column_Selected;
            }
            else if (hovered)
            {
                m_Renderer.DrawColor = Colors.Properties.Column_Hover;
            }
            else
            {
                m_Renderer.DrawColor = Colors.Properties.Column_Normal;
            }

            m_Renderer.DrawFilledRect(new Rectangle(x: 0, rect.Y, iWidth, rect.Height));

            if (bBeingEdited)
            {
                m_Renderer.DrawColor = Colors.Properties.Line_Selected;
            }
            else if (hovered)
            {
                m_Renderer.DrawColor = Colors.Properties.Line_Hover;
            }
            else
            {
                m_Renderer.DrawColor = Colors.Properties.Line_Normal;
            }

            m_Renderer.DrawFilledRect(new Rectangle(iWidth, rect.Y, width: 1, rect.Height));

            rect.Y += rect.Height - 1;
            rect.Height = 1;

            m_Renderer.DrawFilledRect(rect);
        }

        public virtual void DrawColorDisplay(ControlBase control, Color color) {}
        public virtual void DrawModalControl(ControlBase control, Color? backgroundColor) {}
        public virtual void DrawMenuDivider(ControlBase control) {}
        public virtual void DrawCategoryHolder(ControlBase control) {}
        public virtual void DrawCategoryInner(ControlBase control, int headerHeight, bool collapsed) {}

        public virtual void DrawPropertyTreeNode(ControlBase control, int BorderLeft, int BorderTop)
        {
            Rectangle rect = control.RenderBounds;

            m_Renderer.DrawColor = Colors.Properties.Border;

            m_Renderer.DrawFilledRect(new Rectangle(rect.X, rect.Y, BorderLeft, rect.Height));
            m_Renderer.DrawFilledRect(new Rectangle(rect.X + BorderLeft, rect.Y, rect.Width - BorderLeft, BorderTop));
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
            float x = rect.Width / 5.0f;
            float y = rect.Height / 5.0f;

            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 0.0f), rect.Y + (y * 1.0f), x, y * 1.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 1.0f), x, y * 2.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 1.0f), x, y * 3.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 1.0f), x, y * 2.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 4.0f), rect.Y + (y * 1.0f), x, y * 1.0f));
        }

        public virtual void DrawArrowUp(Rectangle rect)
        {
            float x = rect.Width / 5.0f;
            float y = rect.Height / 5.0f;

            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 0.0f), rect.Y + (y * 3.0f), x, y * 1.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 2.0f), x, y * 2.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 1.0f), x, y * 3.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 2.0f), x, y * 2.0f));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 4.0f), rect.Y + (y * 3.0f), x, y * 1.0f));
        }

        public virtual void DrawArrowLeft(Rectangle rect)
        {
            float x = rect.Width / 5.0f;
            float y = rect.Height / 5.0f;

            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 0.0f), x * 1.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 1.0f), x * 2.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 2.0f), x * 3.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 3.0f), x * 2.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 4.0f), x * 1.0f, y));
        }

        public virtual void DrawArrowRight(Rectangle rect)
        {
            float x = rect.Width / 5.0f;
            float y = rect.Height / 5.0f;

            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 0.0f), x * 1.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 1.0f), x * 2.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 2.0f), x * 3.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 3.0f), x * 2.0f, y));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 4.0f), x * 1.0f, y));
        }

        public virtual void DrawCheck(Rectangle rect)
        {
            float x = rect.Width / 5.0f;
            float y = rect.Height / 5.0f;

            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 0.0f), rect.Y + (y * 3.0f), x * 2, y * 2));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 1.0f), rect.Y + (y * 4.0f), x * 2, y * 2));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 2.0f), rect.Y + (y * 3.0f), x * 2, y * 2));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 3.0f), rect.Y + (y * 1.0f), x * 2, y * 2));
            m_Renderer.DrawFilledRect(Util.FloatRect(rect.X + (x * 4.0f), rect.Y + (y * 0.0f), x * 2, y * 2));
        }

        #endregion
    }
}