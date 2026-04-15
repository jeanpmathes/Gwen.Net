using System;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Status bar.
    /// </summary>
    public class StatusBar : ControlBase
    {
        private readonly Label label;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StatusBar" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public StatusBar(ControlBase parent) : base(parent)
        {
            Height = BaseUnit + 11;
            Dock = Dock.Bottom;
            Padding = new Padding(left: 6, top: 2, right: 6, bottom: 1);

            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Bottom;

            label = new Label(this);
            label.AutoSizeToContents = true;
            label.Alignment = Alignment.Left | Alignment.CenterV;
            label.Dock = Dock.Fill;
        }

        public String Text
        {
            get => label.Text;
            set => label.Text = value;
        }

        public Color TextColor
        {
            get => label.TextColor;
            set => label.TextColor = value;
        }

        protected override void AdaptToScaleChange()
        {
            Height = BaseUnit + 11;
        }

        /// <summary>
        ///     Adds a control to the bar.
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="right">Determines whether the control should be added to the right side of the bar.</param>
        public void AddControl(ControlBase control, Boolean right)
        {
            control.Parent = this;
            control.Dock = right ? Dock.Right : Dock.Left;
            control.VerticalAlignment = VerticalAlignment.Center;
        }

        protected override void OnChildAdded(ControlBase child)
        {
            child.VerticalAlignment = VerticalAlignment.Center;

            if (child.Dock != Dock.Left)
            {
                child.Dock = Dock.Right;
            }

            base.OnChildAdded(child);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawStatusBar(this);
        }
    }
}
