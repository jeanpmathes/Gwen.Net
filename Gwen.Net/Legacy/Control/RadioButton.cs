using System;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     Radio button.
    /// </summary>
    public class RadioButton : CheckBox
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RadioButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public RadioButton(ControlBase parent)
            : base(parent)
        {
            MouseInputEnabled = true;
            IsTabable = false;
            IsToggle = true; //[halfofastaple] technically true. "Toggle" isn't the best word, "Sticky" is a better one.
        }

        /// <summary>
        ///     Determines whether unchecking is allowed.
        /// </summary>
        protected override Boolean AllowUncheck => false;

        protected override Size Measure(Size availableSize)
        {
            return new(width: 15, height: 15);
        }

        protected override Size Arrange(Size finalSize)
        {
            return MeasuredSize;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawRadioButton(this, IsChecked, IsDepressed);
        }
    }
}
