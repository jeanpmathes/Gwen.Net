using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Property table/tree.
    /// </summary>
    public class PropertyTree : ScrollControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyTree" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public PropertyTree(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.One;

            MouseInputEnabled = true;
            EnableScroll(horizontal: false, vertical: true);
            AutoHideBars = true;

            new VerticalLayout(this);
        }

        /// <summary>
        ///     Width of the first column (property names).
        /// </summary>
        public int LabelWidth
        {
            get
            {
                foreach (ControlBase child in Children)
                {
                    var node = child as PropertyTreeNode;

                    if (node != null)
                    {
                        return node.Properties.LabelWidth;
                    }
                }

                return Properties.DefaultLabelWidth;
            }
            set
            {
                foreach (ControlBase child in Children)
                {
                    var node = child as PropertyTreeNode;

                    if (node != null)
                    {
                        node.Properties.LabelWidth = value;
                    }
                }
            }
        }

        /// <summary>
        ///     Adds a new properties node.
        /// </summary>
        /// <param name="label">Node label.</param>
        /// <returns>Newly created control</returns>
        public Properties Add(string label)
        {
            PropertyTreeNode node = new(this);
            node.Text = label;

            return node.Properties;
        }

        /// <summary>
        ///     Opens the node and all child nodes.
        /// </summary>
        public void ExpandAll()
        {
            foreach (ControlBase child in Children)
            {
                var node = child as PropertyTreeNode;

                if (node == null)
                {
                    continue;
                }

                node.Open();
            }
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            currentSkin.DrawCategoryHolder(this);
        }
    }
}
