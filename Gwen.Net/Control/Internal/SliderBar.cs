using Gwen.Net.Skin;

namespace Gwen.Net.Control.Internal
{
    /// <summary>
    ///     Slider bar.
    /// </summary>
    public class SliderBar : Dragger
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SliderBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public SliderBar(ControlBase parent)
            : base(parent)
        {
            Size = new Size(BaseUnit);

            Target = this;
            RestrictToParent = true;
        }

        /// <summary>
        ///     Indicates whether the bar is horizontal.
        /// </summary>
        public bool IsHorizontal { get; set; }

        protected override void AdaptToScaleChange()
        {
            Size = new Size(BaseUnit);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            skin.DrawSliderButton(this, IsHeld, IsHorizontal);
        }
    }
}