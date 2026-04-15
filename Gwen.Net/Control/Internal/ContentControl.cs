using System;
using System.Collections.Generic;

namespace Gwen.Net.Control.Internal
{
    public class ContentControl : ControlBase
    {
        /// <summary>
        ///     If the inner panel exists our children will automatically become children of that instead of us.
        /// </summary>
        protected ControlBase innerPanel;

        public ContentControl(ControlBase parent)
            : base(parent) {}

        /// <summary>
        ///     Logical list of children. If InnerPanel is not null, returns InnerPanel's children.
        /// </summary>
        public override List<ControlBase> Children => innerPanel != null ? innerPanel.Children : base.Children;

        /// <summary>
        ///     Get the content of the control.
        /// </summary>
        public virtual ControlBase Content => innerPanel;

        /// <summary>
        ///     Attaches specified control as a child of this one.
        /// </summary>
        /// <remarks>
        ///     If InnerPanel is not null, it will become the parent.
        /// </remarks>
        /// <param name="child">Control to be added as a child.</param>
        public override void AddChild(ControlBase child)
        {
            if (innerPanel != null)
            {
                innerPanel.AddChild(child);
            }
            else
            {
                base.AddChild(child);
            }

            OnChildAdded(child);
        }

        /// <summary>
        ///     Detaches specified control from this one.
        /// </summary>
        /// <param name="child">Child to be removed.</param>
        /// <param name="dispose">Determines whether the child should be disposed (added to delayed delete queue).</param>
        public override void RemoveChild(ControlBase child, Boolean dispose)
        {
            // If we removed our inner panel, remove our pointer to it.
            if (innerPanel == child)
            {
                base.RemoveChild(child, dispose);
                innerPanel = null;

                return;
            }

            if (innerPanel != null && innerPanel.Children.Contains(child))
            {
                innerPanel.RemoveChild(child, dispose);

                return;
            }

            base.RemoveChild(child, dispose);
        }

        /// <summary>
        ///     Finds a child by name.
        /// </summary>
        /// <param name="name">Child name.</param>
        /// <param name="recursive">Determines whether the search should be recursive.</param>
        /// <returns>Found control or null.</returns>
        public override ControlBase FindChildByName(String name, Boolean recursive = false)
        {
            return innerPanel is InnerContentControl ? innerPanel.FindChildByName(name, recursive) : base.FindChildByName(name, recursive);
        }
    }
}
