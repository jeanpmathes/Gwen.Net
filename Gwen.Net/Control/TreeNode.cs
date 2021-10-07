using System;
using System.Collections.Generic;
using System.Linq;
using Gwen.Net.Control.Internal;
using Gwen.Net.Control.Layout;
using Gwen.Net.Skin;
using Gwen.Net.Xml;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Tree control node.
    /// </summary>
    [XmlControl(CustomHandler = "XmlElementHandler")]
    public class TreeNode : ContentControl
    {
        private bool m_Selected;
        protected TreeNodeLabel m_Title;
        protected TreeToggleButton m_ToggleButton;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TreeNode" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TreeNode(ControlBase parent)
            : base(parent)
        {
            // Make sure that the tree control has only one root node.
            if (TreeControl == null && parent is TreeControl)
            {
                TreeControl = parent as TreeControl;
                IsRoot = true;
            }
            else
            {
                m_ToggleButton = new TreeToggleButton(this);
                m_ToggleButton.Toggled += OnToggleButtonPress;

                m_Title = new TreeNodeLabel(this);
                m_Title.DoubleClicked += OnDoubleClickName;
                m_Title.Clicked += OnClickName;
            }

            m_InnerPanel = new VerticalLayout(this);
            m_InnerPanel.Collapse(!IsRoot, measure: false); // Root node is always expanded

            m_Selected = false;
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
        [XmlProperty] public bool IsSelectable { get; set; }

        public int NodeCount => Children.Count;

        /// <summary>
        ///     Indicates whether the node is selected.
        /// </summary>
        [XmlProperty] public bool IsSelected
        {
            get => m_Selected;
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

                m_Selected = value;

                if (m_Title != null)
                {
                    m_Title.ToggleState = value;
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
        [XmlProperty] public string Text
        {
            get => m_Title.Text;
            set => m_Title.Text = value;
        }

        /// <summary>
        ///     List of selected nodes.
        /// </summary>
        public IEnumerable<TreeNode> SelectedChildren
        {
            get
            {
                List<TreeNode> Trees = new();

                foreach (ControlBase child in Children)
                {
                    TreeNode node = child as TreeNode;

                    if (node == null)
                    {
                        continue;
                    }

                    Trees.AddRange(node.SelectedChildren);
                }

                if (IsSelected)
                {
                    Trees.Add(this);
                }

                return Trees;
            }
        }

        /// <summary>
        ///     Invoked when the node label has been pressed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> LabelPressed;

        /// <summary>
        ///     Invoked when the node's selected state has changed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> SelectionChanged;

        /// <summary>
        ///     Invoked when the node has been selected.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Selected;

        /// <summary>
        ///     Invoked when the node has been double clicked and contains no child nodes.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> NodeDoubleClicked;

        /// <summary>
        ///     Invoked when the node has been unselected.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Unselected;

        /// <summary>
        ///     Invoked when the node has been expanded.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Expanded;

        /// <summary>
        ///     Invoked when the node has been collapsed.
        /// </summary>
        [XmlEvent] public event GwenEventHandler<EventArgs> Collapsed;

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void Render(SkinBase skin)
        {
            if (!IsRoot)
            {
                int bottom = 0;

                if (m_InnerPanel.Children.Count > 0)
                {
                    bottom = m_InnerPanel.Children.Last().ActualTop + m_InnerPanel.ActualTop;
                }

                skin.DrawTreeNode(
                    this,
                    m_InnerPanel.IsVisible,
                    IsSelected,
                    m_Title.ActualHeight,
                    m_Title.ActualWidth,
                    (int)(m_ToggleButton.ActualTop + (m_ToggleButton.ActualHeight * 0.5f)),
                    bottom,
                    RootNode == Parent,
                    m_ToggleButton.ActualWidth);
            }
        }

        protected override Size Measure(Size availableSize)
        {
            if (!IsRoot)
            {
                Size buttonSize = m_ToggleButton.DoMeasure(availableSize);
                Size labelSize = m_Title.DoMeasure(availableSize);
                Size innerSize = Size.Zero;

                if (m_InnerPanel.Children.Count == 0)
                {
                    m_ToggleButton.Hide();
                    m_ToggleButton.ToggleState = false;
                    m_InnerPanel.Collapse(collapsed: true, measure: false);
                }
                else
                {
                    m_ToggleButton.Show();

                    if (!m_InnerPanel.IsCollapsed)
                    {
                        innerSize = m_InnerPanel.DoMeasure(availableSize);
                    }
                }

                return new Size(
                    Math.Max(buttonSize.Width + labelSize.Width, m_ToggleButton.MeasuredSize.Width + innerSize.Width),
                    Math.Max(buttonSize.Height, labelSize.Height) + innerSize.Height) + Padding;
            }

            return m_InnerPanel.DoMeasure(availableSize) + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            if (!IsRoot)
            {
                m_ToggleButton.DoArrange(
                    new Rectangle(
                        Padding.Left,
                        Padding.Top + ((m_Title.MeasuredSize.Height - m_ToggleButton.MeasuredSize.Height) / 2),
                        m_ToggleButton.MeasuredSize.Width,
                        m_ToggleButton.MeasuredSize.Height));

                m_Title.DoArrange(
                    new Rectangle(
                        Padding.Left + m_ToggleButton.MeasuredSize.Width,
                        Padding.Top,
                        m_Title.MeasuredSize.Width,
                        m_Title.MeasuredSize.Height));

                if (!m_InnerPanel.IsCollapsed)
                {
                    m_InnerPanel.DoArrange(
                        new Rectangle(
                            Padding.Left + m_ToggleButton.MeasuredSize.Width,
                            Padding.Top + Math.Max(m_ToggleButton.MeasuredSize.Height, m_Title.MeasuredSize.Height),
                            m_InnerPanel.MeasuredSize.Width,
                            m_InnerPanel.MeasuredSize.Height));
                }
            }
            else
            {
                m_InnerPanel.DoArrange(
                    new Rectangle(
                        Padding.Left,
                        Padding.Top,
                        m_InnerPanel.MeasuredSize.Width,
                        m_InnerPanel.MeasuredSize.Height));
            }

            return MeasuredSize;
        }

        /// <summary>
        ///     Adds a new child node.
        /// </summary>
        /// <param name="label">Node's label.</param>
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
                TreeNode node = Children[index: 0] as TreeNode;

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
            m_InnerPanel.Show();

            if (m_ToggleButton != null)
            {
                m_ToggleButton.ToggleState = true;
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
            m_InnerPanel.Collapse();

            if (m_ToggleButton != null)
            {
                m_ToggleButton.ToggleState = false;
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
                TreeNode node = child as TreeNode;

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

            if (m_Title != null)
            {
                m_Title.ToggleState = false;
            }

            foreach (ControlBase child in Children)
            {
                TreeNode node = child as TreeNode;

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
            TreeNode node = Children.Where(x => x is TreeNode && x.UserData == userData).FirstOrDefault() as TreeNode;

            if (node != null)
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
        protected virtual void OnToggleButtonPress(ControlBase control, EventArgs args)
        {
            if (m_ToggleButton.ToggleState)
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
        protected virtual void OnDoubleClickName(ControlBase control, EventArgs args)
        {
            if (!m_ToggleButton.IsVisible)
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

            m_ToggleButton.Toggle();
        }

        /// <summary>
        ///     Handler for label click.
        /// </summary>
        /// <param name="control">Event source.</param>
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
            m_Title.SetImage(textureName);
        }

        protected override void OnChildAdded(ControlBase child)
        {
            TreeNode node = child as TreeNode;

            if (node != null)
            {
                node.TreeControl = TreeControl;

                TreeControl.OnNodeAdded(node);
            }

            base.OnChildAdded(child);
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> Clicked
        {
            add => m_Title.Clicked += delegate(ControlBase sender, ClickedEventArgs args) { value(this, args); };
            remove => m_Title.Clicked -= delegate(ControlBase sender, ClickedEventArgs args) { value(this, args); };
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> DoubleClicked
        {
            add
            {
                if (value != null)
                {
                    m_Title.DoubleClicked += delegate(ControlBase sender, ClickedEventArgs args) { value(this, args); };
                }
            }
            remove => m_Title.DoubleClicked -= delegate(ControlBase sender, ClickedEventArgs args)
            {
                value(this, args);
            };
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> RightClicked
        {
            add => m_Title.RightClicked += delegate(ControlBase sender, ClickedEventArgs args) { value(this, args); };
            remove =>
                m_Title.RightClicked -= delegate(ControlBase sender, ClickedEventArgs args) { value(this, args); };
        }

        [XmlEvent] public override event GwenEventHandler<ClickedEventArgs> DoubleRightClicked
        {
            add
            {
                if (value != null)
                {
                    m_Title.DoubleRightClicked += delegate(ControlBase sender, ClickedEventArgs args)
                    {
                        value(this, args);
                    };
                }
            }
            remove => m_Title.DoubleRightClicked -= delegate(ControlBase sender, ClickedEventArgs args)
            {
                value(this, args);
            };
        }

        internal static ControlBase XmlElementHandler(Parser parser, Type type, ControlBase parent)
        {
            TreeNode element = new(parent);
            parser.ParseAttributes(element);

            if (parser.MoveToContent())
            {
                foreach (string elementName in parser.NextElement())
                {
                    if (elementName == "TreeNode")
                    {
                        parser.ParseElement<TreeNode>(element);
                    }
                }
            }

            return element;
        }
    }
}