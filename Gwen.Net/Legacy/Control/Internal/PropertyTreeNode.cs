using System;
using Gwen.Net.Legacy.Skin;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Properties node.
    /// </summary>
    public class PropertyTreeNode : ContentControl
    {
        public const Int32 TreeIndentation = 14;
        protected readonly Properties properties;

        protected readonly PropertyTree propertyTree;
        protected readonly TreeNodeLabel title;
        protected readonly TreeToggleButton toggleButton;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyTreeNode" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public PropertyTreeNode(ControlBase parent)
            : base(parent)
        {
            propertyTree = parent as PropertyTree;

            toggleButton = new TreeToggleButton(this);
            toggleButton.Toggled += OnToggleButtonPress;

            title = new TreeNodeLabel(this);
            title.DoubleClicked += OnDoubleClickName;

            properties = new Properties(this);

            innerPanel = properties;

            title.TextColorOverride = Skin.colors.propertiesColors.title;
        }

        public PropertyTree PropertyTree => propertyTree;

        public Properties Properties => properties;

        /// <summary>
        ///     Node's label.
        /// </summary>
        public String Text
        {
            get => title.Text;
            set => title.Text = value;
        }

        protected override Size Measure(Size availableSize)
        {
            Size buttonSize = toggleButton.DoMeasure(availableSize);
            Size labelSize = title.DoMeasure(availableSize);
            Size innerSize = Size.Zero;

            if (!innerPanel.IsCollapsed)
            {
                innerSize = innerPanel.DoMeasure(availableSize);
            }

            return new Size(
                Math.Max(buttonSize.Width + labelSize.Width, TreeIndentation + innerSize.Width),
                Math.Max(buttonSize.Height, labelSize.Height) + innerSize.Height);
        }

        protected override Size Arrange(Size finalSize)
        {
            toggleButton.DoArrange(
                new Rectangle(
                    x: 0,
                    (title.MeasuredSize.Height - toggleButton.MeasuredSize.Height) / 2,
                    toggleButton.MeasuredSize.Width,
                    toggleButton.MeasuredSize.Height));

            title.DoArrange(
                new Rectangle(
                    toggleButton.MeasuredSize.Width,
                    y: 0,
                    finalSize.Width - toggleButton.MeasuredSize.Width,
                    title.MeasuredSize.Height));

            if (!innerPanel.IsCollapsed)
            {
                innerPanel.DoArrange(
                    new Rectangle(
                        TreeIndentation,
                        Math.Max(toggleButton.MeasuredSize.Height, title.MeasuredSize.Height),
                        finalSize.Width - TreeIndentation,
                        innerPanel.MeasuredSize.Height));
            }

            return new Size(finalSize.Width, MeasuredSize.Height);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawPropertyTreeNode(this, innerPanel.ActualLeft, innerPanel.ActualTop);
        }

        /// <summary>
        ///     Opens the node.
        /// </summary>
        public void Open()
        {
            innerPanel.Show();

            if (toggleButton != null)
            {
                toggleButton.ToggleState = true;
            }

            Invalidate();
        }

        /// <summary>
        ///     Closes the node.
        /// </summary>
        public void Close()
        {
            innerPanel.Collapse();

            if (toggleButton != null)
            {
                toggleButton.ToggleState = false;
            }

            Invalidate();
        }

        /// <summary>
        ///     Opens the node and all child nodes.
        /// </summary>
        public void Expand()
        {
            Open();

            foreach (ControlBase child in Children)
            {
                var node = child as TreeNode;

                if (node == null)
                {
                    continue;
                }

                node.ExpandAll();
            }
        }

        /// <summary>
        ///     Handler for the toggle button.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnToggleButtonPress(ControlBase control, EventArgs args)
        {
            if (toggleButton.ToggleState)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        ///     Handler for label double click.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnDoubleClickName(ControlBase control, EventArgs args)
        {
            if (!toggleButton.IsVisible)
            {
                return;
            }

            toggleButton.Toggle();
        }
    }
}
