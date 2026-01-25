using System;

namespace Gwen.Net.Legacy.Control.Property
{
    /// <summary>
    ///     Base control for property entry.
    /// </summary>
    public class PropertyBase : ControlBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyBase" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public PropertyBase(ControlBase parent) : base(parent) {}

        /// <summary>
        ///     Property value (todo: always string, which is ugly. do something about it).
        /// </summary>
        public virtual String Value
        {
            get => null;
            set => SetValue(value);
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public virtual Boolean IsEditing => false;

        /// <summary>
        ///     Invoked when the property value has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        protected virtual void DoChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void OnValueChanged(ControlBase control, EventArgs args)
        {
            DoChanged();
        }

        /// <summary>
        ///     Sets the property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <param name="fireEvents">Determines whether to fire "value changed" event.</param>
        public virtual void SetValue(String value, Boolean fireEvents = false) {}
    }
}