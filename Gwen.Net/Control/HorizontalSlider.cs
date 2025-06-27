using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Horizontal slider.
    /// </summary>
    public class HorizontalSlider : Slider
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HorizontalSlider" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public HorizontalSlider(ControlBase parent)
            : base(parent)
        {
            Height = BaseUnit;

            sliderBar.IsHorizontal = true;
        }

        protected override void AdaptToScaleChange()
        {
            Height = BaseUnit;
        }

        protected override Single CalculateValue()
        {
            return (Single) sliderBar.ActualLeft / (ActualWidth - sliderBar.ActualWidth);
        }

        protected override void UpdateBarFromValue()
        {
            sliderBar.MoveTo(
                (Int32) ((ActualWidth - sliderBar.ActualWidth) * value),
                (ActualHeight - sliderBar.ActualHeight) / 2);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected override void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            base.OnMouseClickedLeft(x, y, down);

            sliderBar.MoveTo(
                CanvasPosToLocal(new Point(x, y)).X - (sliderBar.ActualWidth / 2),
                (ActualHeight - sliderBar.ActualHeight) / 2);

            sliderBar.InputMouseClickedLeft(x, y, down);
            OnMoved(sliderBar, EventArgs.Empty);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawSlider(this, horizontal: true, snapToNotches ? notchCount : 0, sliderBar.ActualWidth);
        }
    }
}
