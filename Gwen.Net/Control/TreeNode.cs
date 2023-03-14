using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Tree control node.
    /// </summary>
    public class TreeNode : ContentControl
    {
        private bool selected;
        protected readonly TreeNodeLabel title;
        protected readonly TreeToggleButton toggleButton;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeNode" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse", Justification = "False positive. Value is set by virtual call in base constructor.")]
        public TreeNode(ControlBase parent)
            : base(parent)
        {
            // Make sure that the tree control has only one root node.
            if (TreeControl == null && parent is TreeControl control)
            {
                TreeControl = control;
                IsRoot = true;
            }
            else
            {
                toggleButton = new TreeToggleButton(this);
                toggleButton.Toggled += OnToggleButtonPress;

                title = new TreeNodeLabel(this);
                title.DoubleClicked += OnDoubleClickName;
                title.Clicked += OnClickName;
            }

            innerPanel = new VerticalLayout(this);
            innerPanel.Collapse(!IsRoot, measure: false); // Root node is always expanded

            selected = false;
            IsSelectable = true;
        }

        /// <summary>
        ///     Root node of the tree view.
        /// </summary>
        private TreeNode RootNode => TreeControl.RootNode;

        /// <summary>
        ///     Parent tree control.
        /// </summary>
        public TreeControl TreeControl { get; private set; }

        /// <summary>
        ///     Indicates whether this is a root node.
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        ///     Determines whether the node is selectable.
        /// </summary>
        public bool IsSelectable { get; set; }

        public int NodeCount => Children.Count;

        /// <summary>
        ///     Indicates whether the node is selected.
        /// </summary>
        public bool IsSelected
        {
            get => selected;
            set
            {
                if (!IsSelectable)
                {
                    return;
                }

                if (IsSelected == value)
                {
                    return;
                }

                if (value && !TreeControl.AllowMultiSelect)
                {
                    RootNode.UnselectAll();
                }

                selected = value;

                if (title != null)
                {
                    title.ToggleState = value;
                }

                if (SelectionChanged != null)
                {
                    SelectionChanged.Invoke(this, EventArgs.Empty);
                }

                // propagate to root parent (tree)
                if (RootNode != null && RootNode.SelectionChanged != null)
                {
                    RootNode.SelectionChanged.Invoke(this, EventArgs.Empty);
                }

                if (value)
                {
                    if (Selected != null)
                    {
                        Selected.Invoke(this, EventArgs.Empty);
                    }

                    if (RootNode != null && RootNode.Selected != null)
                    {
                        RootNode.Selected.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (Unselected != null)
                    {
                        Unselected.Invoke(this, EventArgs.Empty);
                    }

                    if (RootNode != null && RootNode.Unselected != null)
                    {
                        RootNode.Unselected.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///     Node's label.
        /// </summary>
        public string Text
        {
            get => title.Text;
            set => title.Text = value;
        }

        /// <summary>
        ///     List of selected nodes.
        /// </summary>
        public IEnumerable<TreeNode> SelectedChildren
        {
            get
            {
                List<TreeNode> trees = new();

                foreach (ControlBase child in Children)
                {
                    var node = child as TreeNode;

                    if (node == null)
                    {
                        continue;
                    }

                    trees.AddRange(node.SelectedChildren);
                }

                if (IsSelected)
                {
                    trees.Add(this);
                }

                return trees;
            }
        }

        /// <summary>
        ///     Invoked when the node label has been pressed.
        /// </summary>
        public event GwenEventHandler<EventArgs> LabelPressed;

        /// <summary>
        ///     Invoked when the node's selected state has changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> SelectionChanged;

        /// <summary>
        ///     Invoked when the node has been selected.
        /// </summary>
        public event GwenEventHandler<EventArgs> Selected;

        /// <summary>
        ///     Invoked when the node has been double clicked and contains no child nodes.
        /// </summary>
        public event GwenEventHandler<EventArgs> NodeDoubleClicked;

        /// <summary>
        ///     Invoked when the node has been unselected.
        /// </summary>
        public event GwenEventHandler<EventArgs> Unselected;

        /// <summary>
        ///     Invoked when the node has been expanded.
        /// </summary>
        public event GwenEventHandler<EventArgs> Expanded;

        /// <summary>
        ///     Invoked when the node has been collapsed.
        /// </summary>
        public event GwenEventHandler<EventArgs> Collapsed;

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            if (!IsRoot)
            {
                var bottom = 0;

                if (innerPanel.Children.Count > 0)
                {
                    bottom = innerPanel.Children.Last().ActualTop + innerPanel.ActualTop;
                }

                currentSkin.DrawTreeNode(
                    this,
                    innerPanel.IsVisible,
                    IsSelected,
                    title.ActualHeight,
                    title.ActualWidth,
                    (int) (toggleButton.ActualTop + (toggleButton.ActualHeight * 0.5f)),
                    bottom,
                    RootNode == Parent,
                    toggleButton.ActualWidth);
            }
        }

        protected override Size Measure(Size availableSize)
        {
            if (!IsRoot)
            {
                Size buttonSize = toggleButton.DoMeasure(availableSize);
                Size labelSize = title.DoMeasure(availableSize);
                Size innerSize = Size.Zero;

                if (innerPanel.Children.Count == 0)
                {
                    toggleButton.Hide();
                    toggleButton.ToggleState = false;
                    innerPanel.Collapse(collapsed: true, measure: false);
                }
                else
                {
                    toggleButton.Show();

                    if (!innerPanel.IsCollapsed)
                    {
                        innerSize = innerPanel.DoMeasure(availableSize);
                    }
                }

                return new Size(
                    Math.Max(buttonSize.Width + labelSize.Width, toggleButton.MeasuredSize.Width + innerSize.Width),
                    Math.Max(buttonSize.Height, labelSize.Height) + innerSize.Height) + Padding;
            }

            return innerPanel.DoMeasure(availableSize) + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            if (!IsRoot)
            {
                toggleButton.DoArrange(
                    new Rectangle(
                        Padding.Left,
                        Padding.Top + ((title.MeasuredSize.Height - toggleButton.MeasuredSize.Height) / 2),
                        toggleButton.MeasuredSize.Width,
                        toggleButton.MeasuredSize.Height));

                title.DoArrange(
                    new Rectangle(
                        Padding.Left + toggleButton.MeasuredSize.Width,
                        Padding.Top,
                        title.MeasuredSize.Width,
                        title.MeasuredSize.Height));

                if (!innerPanel.IsCollapsed)
                {
                    innerPanel.DoArrange(
                        new Rectangle(
                            Padding.Left + toggleButton.MeasuredSize.Width,
                            Padding.Top + Math.Max(toggleButton.MeasuredSize.Height, title.MeasuredSize.Height),
                            innerPanel.MeasuredSize.Width,
                            innerPanel.MeasuredSize.Height));
                }
            }
            else
            {
                innerPanel.DoArrange(
                    new Rectangle(
                        Padding.Left,
                        Padding.Top,
                        innerPanel.MeasuredSize.Width,
                        innerPanel.MeasuredSize.Height));
            }

            return MeasuredSize;
        }

        /// <summary>
        ///     Adds a new child node.
        /// </summary>
        /// <param name="label">Node's label.</param>
        /// <param name="name">Node's name.</param>
        /// <param name="userData">User data.</param>
        /// <returns>Newly created control.</returns>
        public TreeNode AddNode(string label, string name = null, object userData = null)
        {
            TreeNode node = new(this);
            node.Text = label;
            node.Name = name;
            node.UserData = userData;

            return node;
        }

        public TreeNode InsertNode(int index, string label, string name = null, object userData = null)
        {
            TreeNode node = AddNode(label, name, userData);

            if (index == 0)
            {
                node.SendToBack();
            }
            else if (index < Children.Count)
            {
                node.BringNextToControl(Children[index], behind: false);
            }

            return node;
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

            node.RemoveAllNodes();

            RemoveChild(node, dispose: true);

            Invalidate();
        }

        /// <summary>
        ///     Remove all nodes.
        /// </summary>
        public void RemoveAllNodes()
        {
            while (NodeCount > 0)
            {
                var node = Children[index: 0] as TreeNode;

                if (node == null)
                {
                    continue;
                }

                RemoveNode(node);
            }

            Invalidate();
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

            if (Expanded != null)
            {
                Expanded.Invoke(this, EventArgs.Empty);
            }

            if (RootNode != null && RootNode.Expanded != null)
            {
                RootNode.Expanded.Invoke(this, EventArgs.Empty);
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

            if (Collapsed != null)
            {
                Collapsed.Invoke(this, EventArgs.Empty);
            }

            if (RootNode != null && RootNode.Collapsed != null)
            {
                RootNode.Collapsed.Invoke(this, EventArgs.Empty);
            }

            Invalidate();
        }

        /// <summary>
        ///     Opens the node and all child nodes.
        /// </summary>
        public void ExpandAll()
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
        ///     Clears the selection on the node and all child nodes.
        /// </summary>
        public void UnselectAll()
        {
            IsSelected = false;

            if (title != null)
            {
                title.ToggleState = false;
            }

            foreach (ControlBase child in Children)
            {
                var node = child as TreeNode;

                if (node == null)
                {
                    continue;
                }

                node.UnselectAll();
            }
        }

        /// <summary>
        ///     Find a node bu user data.
        /// </summary>
        /// <param name="userData">Node user data.</param>
        /// <param name="recursive">Determines whether the search should be recursive.</param>
        /// <returns>Found node or null.</returns>
        public TreeNode FindNodeByUserData(object userData, bool recursive = true)
        {
            if (Children.FirstOrDefault(x => x is TreeNode && x.UserData == userData) is TreeNode node)
            {
                return node;
            }

            if (recursive)
            {
                foreach (ControlBase child in Children)
                {
                    node = child as TreeNode;

                    if (node != null)
                    {
                        node = node.FindNodeByUserData(userData);

                        if (node != null)
                        {
                            return node;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Find a node by name.
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="recursive">Determines whether the search should be recursive.</param>
        /// <returns>Found node or null.</returns>
        public TreeNode FindNodeByName(string name, bool recursive = true)
        {
            return FindChildByName(name, recursive) as TreeNode;
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
                // Invoke double click events only if node hasn't child nodes.
                // Otherwise toggle expand/collapse.
                if (NodeDoubleClicked != null)
                {
                    NodeDoubleClicked.Invoke(this, EventArgs.Empty);
                }

                if (RootNode != null && RootNode.NodeDoubleClicked != null)
                {
                    RootNode.NodeDoubleClicked.Invoke(this, EventArgs.Empty);
                }

                return;
            }

            toggleButton.Toggle();
        }

        /// <summary>
        ///     Handler for label click.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnClickName(ControlBase control, EventArgs args)
        {
            if (LabelPressed != null)
            {
                LabelPressed.Invoke(this, EventArgs.Empty);
            }

            IsSelected = !IsSelected;
        }

        public void SetImage(string textureName)
        {
            title.SetImage(textureName);
        }

        protected override void OnChildAdded(ControlBase child)
        {
            if (child is TreeNode node)
            {
                node.TreeControl = TreeControl;

                TreeControl.OnNodeAdded(node);
            }

            base.OnChildAdded(child);
        }

        private readonly Dictionary<GwenEventHandler<ClickedEventArgs>, GwenEventHandler<ClickedEventArgs>> clickedHandlers = new();

        private GwenEventHandler<ClickedEventArgs> CreateClickedHandler(GwenEventHandler<ClickedEventArgs> handler)
        {
            if (clickedHandlers.ContainsKey(handler))
            {
                return clickedHandlers[handler];
            }
            
            GwenEventHandler<ClickedEventArgs> clicked = delegate(ControlBase _, ClickedEventArgs args)
            {
                handler(this, args);
            };
            
            clickedHandlers.Add(handler, clicked);
            
            return clicked;
        }
        
        private GwenEventHandler<ClickedEventArgs> RemoveClickedHandler(GwenEventHandler<ClickedEventArgs> handler)
        {
            GwenEventHandler<ClickedEventArgs> clicked = clickedHandlers[handler];
            clickedHandlers.Remove(handler);
            return clicked;
        }

        public override event GwenEventHandler<ClickedEventArgs> Clicked
        {
            add
            {
                if (value != null)
                {
                    title.Clicked += CreateClickedHandler(value);
                }
            }
            remove => title.Clicked -= RemoveClickedHandler(value);
        }

        public override event GwenEventHandler<ClickedEventArgs> DoubleClicked
        {
            add
            {
                if (value != null)
                {
                    title.DoubleClicked += CreateClickedHandler(value);
                }
            }
            remove => title.DoubleClicked -= RemoveClickedHandler(value);
        }

        public override event GwenEventHandler<ClickedEventArgs> RightClicked
        {
            add
            {
                if (value != null)
                {
                    title.RightClicked += CreateClickedHandler(value);
                }
            }
            remove => title.RightClicked -= RemoveClickedHandler(value);
        }

        public override event GwenEventHandler<ClickedEventArgs> DoubleRightClicked
        {
            add
            {
                if (value != null)
                {
                    title.DoubleRightClicked += CreateClickedHandler(value);
                }
            }
            remove => title.DoubleRightClicked -= RemoveClickedHandler(value);
        }
    }
}
