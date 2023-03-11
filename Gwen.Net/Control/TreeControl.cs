using System;
using System.Collections.Generic;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Tree control.
    /// </summary>
    public class TreeControl : ScrollControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeControl" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TreeControl(ControlBase parent)
            : base(parent)
        {
            Padding = Padding.One;

            MouseInputEnabled = true;
            EnableScroll(horizontal: true, vertical: true);
            AutoHideBars = true;

            AllowMultiSelect = false;

            RootNode = new TreeNode(this);
        }

        /// <summary>
        ///     List of selected nodes.
        /// </summary>
        public IEnumerable<TreeNode> SelectedNodes
        {
            get
            {
                List<TreeNode> selectedNodes = new();

                foreach (ControlBase child in RootNode.Children)
                {
                    var node = child as TreeNode;

                    if (node == null)
                    {
                        continue;
                    }

                    selectedNodes.AddRange(node.SelectedChildren);
                }

                return selectedNodes;
            }
        }

        /// <summary>
        ///     First selected node (and only if nodes are not multiselectable).
        /// </summary>
        public TreeNode SelectedNode
        {
            get
            {
                var selectedNodes = SelectedNodes as List<TreeNode>;

                if (selectedNodes.Count > 0)
                {
                    return selectedNodes[index: 0];
                }

                return null;
            }
        }

        /// <summary>
        ///     Determines if multiple nodes can be selected at the same time.
        /// </summary>
        public bool AllowMultiSelect { get; set; }

        /// <summary>
        ///     Get the root node of the tree view. Root node is an invisible always expanded node that works
        ///     as a parent node for all first tier nodes visible on the control.
        /// </summary>
        public TreeNode RootNode { get; }

        /// <summary>
        ///     Invoked when the node's selected state has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> SelectionChanged
        {
            add => RootNode.SelectionChanged += value;
            remove => RootNode.SelectionChanged -= value;
        }

        /// <summary>
        ///     Invoked when the node has been selected.
        /// </summary>
        public event GwenEventHandler<EventArgs> Selected
        {
            add => RootNode.Selected += value;
            remove => RootNode.Selected -= value;
        }

        /// <summary>
        ///     Invoked when the node has been unselected.
        /// </summary>
        public event GwenEventHandler<EventArgs> Unselected
        {
            add => RootNode.Unselected += value;
            remove => RootNode.Unselected -= value;
        }

        /// <summary>
        ///     Invoked when the node has been double clicked and contains no child nodes.
        /// </summary>
        public event GwenEventHandler<EventArgs> NodeDoubleClicked
        {
            add => RootNode.NodeDoubleClicked += value;
            remove => RootNode.NodeDoubleClicked -= value;
        }

        /// <summary>
        ///     Invoked when the node has been expanded.
        /// </summary>
        public event GwenEventHandler<EventArgs> Expanded
        {
            add => RootNode.Expanded += value;
            remove => RootNode.Expanded -= value;
        }

        /// <summary>
        ///     Invoked when the node has been collapsed.
        /// </summary>
        public event GwenEventHandler<EventArgs> Collapsed
        {
            add => RootNode.Collapsed += value;
            remove => RootNode.Collapsed -= value;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            if (ShouldDrawBackground)
            {
                skin.DrawTreeControl(this);
            }
        }

        /// <summary>
        ///     Adds a new child node.
        /// </summary>
        /// <param name="label">Node's label.</param>
        /// <returns>Newly created control.</returns>
        public TreeNode AddNode(string label, string name = null, object userData = null)
        {
            return RootNode.AddNode(label, name, userData);
        }

        /// <summary>
        ///     Removes all child nodes.
        /// </summary>
        public virtual void RemoveAll()
        {
            RootNode.DeleteAllChildren();
        }

        /// <summary>
        ///     Remove node and all of it's child nodes.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        public void RemoveNode(TreeNode node)
        {
            if (node == null)
            {
                return;
            }

            RootNode.RemoveNode(node);
        }

        /// <summary>
        ///     Remove all nodes.
        /// </summary>
        public void RemoveAllNodes()
        {
            RootNode.RemoveAllNodes();
        }

        /// <summary>
        ///     Opens the node and all child nodes.
        /// </summary>
        public void ExpandAll()
        {
            RootNode.ExpandAll();
        }

        /// <summary>
        ///     Clears the selection on the node and all child nodes.
        /// </summary>
        public void UnselectAll()
        {
            RootNode.UnselectAll();
        }

        /// <summary>
        ///     Find a node bu user data.
        /// </summary>
        /// <param name="userData">Node user data.</param>
        /// <param name="recursive">Determines whether the search should be recursive.</param>
        /// <returns>Found node or null.</returns>
        public TreeNode FindNodeByUserData(object userData, bool recursive = true)
        {
            return RootNode.FindNodeByUserData(userData, recursive);
        }

        /// <summary>
        ///     Find a node by name.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="recursive">Determines whether the search should be recursive.</param>
        /// <returns>Found node or null.</returns>
        public TreeNode FindNodeByName(string name, bool recursive = true)
        {
            return RootNode.FindNodeByName(name, recursive);
        }

        /// <summary>
        ///     Handler for node added event.
        /// </summary>
        /// <param name="node">Node added.</param>
        public virtual void OnNodeAdded(TreeNode node)
        {
            node.LabelPressed += OnNodeSelected;
        }

        /// <summary>
        ///     Handler for node selected event.
        /// </summary>
        /// <param name="Control">Node selected.</param>
        protected virtual void OnNodeSelected(ControlBase Control, EventArgs args)
        {
            if (!AllowMultiSelect /*|| InputHandler.InputHandler.IsKeyDown(Key.Control)*/)
            {
                UnselectAll();
            }
        }
    }
}
