using System;

namespace Gwen.Net.Legacy.Control.Property
{
    /// <summary>
    ///     Checkable property.
    /// </summary>
    public class CheckProperty : PropertyBase
    {
        protected readonly CheckBox checkBox;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CheckProperty" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CheckProperty(ControlBase parent)
            : base(parent)
        {
            checkBox = new CheckBox(this);
            checkBox.Dock = Dock.Left;
            checkBox.ShouldDrawBackground = false;
            checkBox.CheckChanged += OnValueChanged;
            checkBox.IsTabable = true;
            checkBox.KeyboardInputEnabled = true;
        }

        /// <summary>
        ///     Property value.
        /// </summary>
        public override String Value
        {
            get => checkBox.IsChecked ? "1" : "0";
            set => base.Value = value;
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public override Boolean IsEditing => checkBox.HasFocus;

        /// <summary>
        ///     Indicates whether the control is hovered by mouse pointer.
        /// </summary>
        public override Boolean IsHovered => base.IsHovered || checkBox.IsHovered;

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public override void SetValue(String value, Boolean fireEvents = false)
        {
            if (value == "1" || value.ToLower() == "true" || value.ToLower() == "yes")
            {
                checkBox.IsChecked = true;
            }
            else
            {
                checkBox.IsChecked = false;
            }
        }
    }
}
