using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     CheckBox control.
    /// </summary>
    public class CheckBox : ButtonBase
    {
        private Boolean @checked;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CheckBox" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CheckBox(ControlBase parent)
            : base(parent)
        {
            Size = new Size(BaseUnit);
            IsToggle = true;
        }

        /// <summary>
        ///     Indicates whether the checkbox is checked.
        /// </summary>
        public Boolean IsChecked
        {
            get => @checked;
            set
            {
                if (@checked == value)
                {
                    return;
                }

                @checked = value;
                OnCheckChanged();
            }
        }

        /// <summary>
        ///     Determines whether unchecking is allowed.
        /// </summary>
        protected virtual Boolean AllowUncheck => true;

        protected override void AdaptToScaleChange()
        {
            Size = new Size(BaseUnit);
        }

        /// <summary>
        ///     Toggles the checkbox.
        /// </summary>
        public override void Toggle()
        {
            base.Toggle();
            IsChecked = !IsChecked;
        }

        /// <summary>
        ///     Invoked when the checkbox has been checked.
        /// </summary>
        public event GwenEventHandler<EventArgs> Checked;

        /// <summary>
        ///     Invoked when the checkbox has been unchecked.
        /// </summary>
        public event GwenEventHandler<EventArgs> UnChecked;

        /// <summary>
        ///     Invoked when the checkbox state has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> CheckChanged;

        /// <summary>
        ///     Handler for CheckChanged event.
        /// </summary>
        protected virtual void OnCheckChanged()
        {
            if (IsChecked)
            {
                if (Checked != null)
                {
                    Checked.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                if (UnChecked != null)
                {
                    UnChecked.Invoke(this, EventArgs.Empty);
                }
            }

            if (CheckChanged != null)
            {
                CheckChanged.Invoke(this, EventArgs.Empty);
            }
        }

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
            base.Render(currentSkin);
            currentSkin.DrawCheckBox(this, @checked, IsDepressed);
        }

        /// <summary>
        ///     Internal OnPressed implementation.
        /// </summary>
        protected override void OnClicked(Int32 x, Int32 y)
        {
            if (IsDisabled) return;
            if (IsChecked && !AllowUncheck) return;

            base.OnClicked(x, y);
        }
    }
}
