using System;
using Gwen.Net.Legacy.Control.Internal;
using Gwen.Net.Legacy.Control.Property;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control
{
    /// <summary>
    ///     Single property row.
    /// </summary>
    public class PropertyRow : ControlBase
    {
        private readonly Label label;
        private readonly PropertyBase property;
        private Boolean lastEditing;
        private Boolean lastHover;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyRow" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        /// <param name="prop">Property control associated with this row.</param>
        public PropertyRow(ControlBase parent, PropertyBase prop)
            : base(parent)
        {
            Padding = new Padding(left: 2, top: 2, right: 2, bottom: 2);

            label = new PropertyRowLabel(this);
            label.Alignment = Alignment.Left | Alignment.Top;

            property = prop;
            property.Parent = this;
            property.ValueChanged += OnValueChanged;
        }

        /// <summary>
        ///     Indicates whether the property value is being edited.
        /// </summary>
        public Boolean IsEditing => property != null && property.IsEditing;

        /// <summary>
        ///     Property value.
        /// </summary>
        public String Value
        {
            get => property.Value;
            set => property.Value = value;
        }

        /// <summary>
        ///     Indicates whether the control is hovered by mouse pointer.
        /// </summary>
        public override Boolean IsHovered => base.IsHovered || (property != null && property.IsHovered);

        /// <summary>
        ///     Property name.
        /// </summary>
        public String Label
        {
            get => label.Text;
            set => label.Text = value;
        }

        /// <summary>
        ///     Invoked when the property value has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            /* SORRY */
            if (IsEditing != lastEditing)
            {
                OnEditingChanged();
                lastEditing = IsEditing;
            }

            if (IsHovered != lastHover)
            {
                OnHoverChanged();
                lastHover = IsHovered;
            }
            /* SORRY */

            currentSkin.DrawPropertyRow(this, label.ActualRight, IsEditing, IsHovered | property.IsHovered);
        }

        protected override Size Measure(Size availableSize)
        {
            var parent = Parent as Properties;

            if (parent != null)
            {
                Size labelSize = label.DoMeasure(
                    new Size(parent.LabelWidth - Padding.Left - Padding.Right, availableSize.Height)) + Padding;

                Size propertySize =
                    property.DoMeasure(new Size(availableSize.Width - parent.LabelWidth, availableSize.Height)) +
                    Padding;

                return new Size(labelSize.Width + propertySize.Width, Math.Max(labelSize.Height, propertySize.Height));
            }

            return Size.Zero;
        }

        protected override Size Arrange(Size finalSize)
        {
            var parent = Parent as Properties;

            if (parent != null)
            {
                label.DoArrange(
                    new Rectangle(
                        Padding.Left,
                        Padding.Top,
                        parent.LabelWidth - Padding.Left - Padding.Right,
                        label.MeasuredSize.Height));

                property.DoArrange(
                    new Rectangle(
                        parent.LabelWidth + Padding.Left,
                        Padding.Top,
                        finalSize.Width - parent.LabelWidth - Padding.Left - Padding.Right,
                        property.MeasuredSize.Height));

                return new Size(
                    finalSize.Width,
                    Math.Max(label.MeasuredSize.Height, property.MeasuredSize.Height) + Padding.Top +
                    Padding.Bottom);
            }

            return Size.Zero;
        }

        protected virtual void OnValueChanged(ControlBase control, EventArgs args)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnEditingChanged()
        {
            label.Redraw();
        }

        private void OnHoverChanged()
        {
            label.Redraw();
        }
    }
}
