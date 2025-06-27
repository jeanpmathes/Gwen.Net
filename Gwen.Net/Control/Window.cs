using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Movable window with title bar.
    /// </summary>
    public class Window : WindowBase
    {
        private readonly WindowTitleBar titleBar;
        private Modal modal;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Window" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Window(ControlBase parent)
            : base(parent)
        {
            titleBar = new WindowTitleBar(this);
            titleBar.Height = BaseUnit + 9;
            titleBar.Title.TextColor = Skin.colors.windowColors.titleInactive;
            titleBar.CloseButton.Clicked += CloseButtonPressed;
            titleBar.SendToBack();
            titleBar.Dragged += OnDragged;

            dragBar = titleBar;

            innerPanel = new InnerContentControl(this);
            innerPanel.SendToBack();
        }

        /// <summary>
        ///     Window caption.
        /// </summary>
        public String Title
        {
            get => titleBar.Title.Text;
            set => titleBar.Title.Text = value;
        }

        /// <summary>
        ///     Determines whether the window has close button.
        /// </summary>
        public Boolean IsClosable
        {
            get => !titleBar.CloseButton.IsCollapsed;
            set => titleBar.CloseButton.IsCollapsed = !value;
        }

        /// <summary>
        ///     Make window modal and set background color. If alpha value is zero, make background dimmed.
        /// </summary>
        public Color ModalBackground
        {
            set
            {
                if (value.A == 0)
                {
                    MakeModal(dim: true);
                }
                else
                {
                    MakeModal(dim: true, value);
                }
            }
        }

        /// <summary>
        ///     Set true to make window modal.
        /// </summary>
        public Boolean Modal
        {
            get => modal != null;
            set
            {
                if (value) MakeModal();
            }
        }

        protected override void AdaptToScaleChange()
        {
            titleBar.Height = BaseUnit + 9;
        }

        public override void Close()
        {
            if (modal != null)
            {
                modal.DelayedDelete();
                modal = null;
            }

            base.Close();
        }

        protected virtual void CloseButtonPressed(ControlBase control, EventArgs args)
        {
            Close();
        }

        /// <summary>
        ///     Makes the window modal: covers the whole canvas and gets all input.
        /// </summary>
        /// <param name="dim">Determines whether all the background should be dimmed.</param>
        /// <param name="backgroundColor">Determines background color.</param>
        public void MakeModal(Boolean dim = false, Color? backgroundColor = null)
        {
            if (modal != null)
            {
                return;
            }

            modal = new Modal(GetCanvas());
            Parent = modal;

            if (dim)
            {
                modal.ShouldDrawBackground = true;
            }
            else
            {
                modal.ShouldDrawBackground = false;
            }

            if (backgroundColor != null)
            {
                modal.ShouldDrawBackground = true;
                modal.BackgroundColor = backgroundColor;
            }
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            Boolean hasFocus = IsOnTop;

            if (hasFocus)
            {
                titleBar.Title.TextColor = Skin.colors.windowColors.titleActive;
            }
            else
            {
                titleBar.Title.TextColor = Skin.colors.windowColors.titleInactive;
            }

            currentSkin.DrawWindow(this, titleBar.ActualHeight, hasFocus);
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderUnder(SkinBase currentSkin)
        {
            base.RenderUnder(currentSkin);
            currentSkin.DrawShadow(this);
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void RenderFocus(SkinBase currentSkin) {}

        protected override Size Measure(Size availableSize)
        {
            Size titleBarSize = titleBar.DoMeasure(new Size(availableSize.Width, availableSize.Height));

            if (innerPanel != null)
            {
                innerPanel.DoMeasure(new Size(availableSize.Width, availableSize.Height - titleBarSize.Height));
            }

            return base.Measure(
                new Size(innerPanel.MeasuredSize.Width, innerPanel.MeasuredSize.Height + titleBarSize.Height));
        }

        protected override Size Arrange(Size finalSize)
        {
            titleBar.DoArrange(new Rectangle(x: 0, y: 0, finalSize.Width, titleBar.MeasuredSize.Height));

            if (innerPanel != null)
            {
                innerPanel.DoArrange(
                    new Rectangle(
                        x: 0,
                        titleBar.MeasuredSize.Height,
                        finalSize.Width,
                        finalSize.Height - titleBar.MeasuredSize.Height));
            }

            return base.Arrange(finalSize);
        }

        public override void EnableResizing(Boolean left = true, Boolean top = true, Boolean right = true, Boolean bottom = true)
        {
            base.EnableResizing(left, top: false, right, bottom);
        }

        public override void Dispose()
        {
            if (modal != null)
            {
                modal.DelayedDelete();
                modal = null;
            }
            else
            {
                base.Dispose();
            }
        }
    }
}
