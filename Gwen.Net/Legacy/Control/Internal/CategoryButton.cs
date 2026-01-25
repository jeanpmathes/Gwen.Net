using System;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Item in CollapsibleCategory.
    /// </summary>
    public class CategoryButton : Button
    {
        internal Boolean alt; // for alternate coloring

        /// <summary>
        ///     Initializes a new instance of the <see cref="CategoryButton" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CategoryButton(ControlBase parent) : base(parent)
        {
            Alignment = Alignment.Left | Alignment.CenterV;
            alt = false;
            IsToggle = true;
            TextPadding = new Padding(left: 3, top: 0, right: 3, bottom: 0);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            if (alt)
            {
                if (IsDepressed || ToggleState)
                {
                    Skin.Renderer.DrawColor = currentSkin.colors.categoryColors.lineAltColors.buttonSelected;
                }
                else if (IsHovered)
                {
                    Skin.Renderer.DrawColor = currentSkin.colors.categoryColors.lineAltColors.buttonHover;
                }
                else
                {
                    Skin.Renderer.DrawColor = currentSkin.colors.categoryColors.lineAltColors.button;
                }
            }
            else
            {
                if (IsDepressed || ToggleState)
                {
                    Skin.Renderer.DrawColor = currentSkin.colors.categoryColors.lineColors.buttonSelected;
                }
                else if (IsHovered)
                {
                    Skin.Renderer.DrawColor = currentSkin.colors.categoryColors.lineColors.buttonHover;
                }
                else
                {
                    Skin.Renderer.DrawColor = currentSkin.colors.categoryColors.lineColors.button;
                }
            }

            currentSkin.Renderer.DrawFilledRect(RenderBounds);
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (alt)
            {
                if (IsDepressed || ToggleState)
                {
                    TextColor = Skin.colors.categoryColors.lineAltColors.textSelected;

                    return;
                }

                if (IsHovered)
                {
                    TextColor = Skin.colors.categoryColors.lineAltColors.textHover;

                    return;
                }

                TextColor = Skin.colors.categoryColors.lineAltColors.text;

                return;
            }

            if (IsDepressed || ToggleState)
            {
                TextColor = Skin.colors.categoryColors.lineColors.textSelected;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.colors.categoryColors.lineColors.textHover;

                return;
            }

            TextColor = Skin.colors.categoryColors.lineColors.text;
        }
    }
}
