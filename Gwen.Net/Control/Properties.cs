using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Control.Property;
using Text = Gwen.Net.Control.Property.Text;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Properties table.
    /// </summary>
    public class Properties : ContentControl
    {
        internal const int DefaultLabelWidth = 80;
        private readonly SplitterBar splitterBar;
        private int labelWidth;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Properties" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Properties(ControlBase parent)
            : base(parent)
        {
            splitterBar = new SplitterBar(this);
            splitterBar.Width = 3;
            splitterBar.Cursor = Cursor.SizeWE;
            splitterBar.Dragged += OnSplitterMoved;
            splitterBar.ShouldDrawBackground = false;

            labelWidth = DefaultLabelWidth;

            innerPanel = new VerticalLayout(this);
        }

        /// <summary>
        ///     Width of the first column (property names).
        /// </summary>
        internal int LabelWidth
        {
            get => labelWidth;
            set
            {
                if (value == labelWidth)
                {
                    return;
                }

                labelWidth = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Invoked when a property value has been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> ValueChanged;

        protected override Size Measure(Size availableSize)
        {
            availableSize -= Padding;

            Size size = innerPanel.DoMeasure(availableSize);

            splitterBar.DoMeasure(new Size(availableSize.Width, size.Height));

            return size + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            finalSize -= Padding;

            innerPanel.DoArrange(Padding.Left, Padding.Top, finalSize.Width, finalSize.Height);

            splitterBar.DoArrange(
                Padding.Left + labelWidth - 2,
                Padding.Top,
                splitterBar.MeasuredSize.Width,
                innerPanel.MeasuredSize.Height);

            return new Size(finalSize.Width, innerPanel.MeasuredSize.Height) + Padding;
        }

        /// <summary>
        ///     Handles the splitter moved event.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnSplitterMoved(ControlBase control, EventArgs args)
        {
            LabelWidth = splitterBar.ActualLeft - Padding.Left;

            var node = Parent as PropertyTreeNode;

            if (node != null)
            {
                node.PropertyTree.LabelWidth = LabelWidth;
            }
        }

        /// <summary>
        ///     Adds a new text property row.
        /// </summary>
        /// <param name="label">Property name.</param>
        /// <param name="value">Initial value.</param>
        /// <returns>Newly created row.</returns>
        public PropertyRow Add(string label, string value = "")
        {
            return Add(label, new Text(this), value);
        }

        /// <summary>
        ///     Adds a new property row.
        /// </summary>
        /// <param name="label">Property name.</param>
        /// <param name="prop">Property control.</param>
        /// <param name="value">Initial value.</param>
        /// <returns>Newly created row.</returns>
        public PropertyRow Add(string label, PropertyBase prop, string value = "")
        {
            PropertyRow row = new(this, prop);
            row.Label = label;
            row.ValueChanged += OnRowValueChanged;

            prop.SetValue(value, fireEvents: true);

            splitterBar.BringToFront();

            return row;
        }

        private void OnRowValueChanged(ControlBase control, EventArgs args)
        {
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(control, EventArgs.Empty);
            }
        }

        /// <summary>
        ///     Deletes all rows.
        /// </summary>
        public void DeleteAll()
        {
            innerPanel.DeleteAllChildren();
        }
    }
}
