using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gwen.Net.Components;
using Gwen.Net.Control.Internal;
using Gwen.Net.DragDrop;
using Gwen.Net.Input;
using Gwen.Net.Platform;
using Gwen.Net.Renderer;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Base control class.
    /// </summary>
    public abstract class ControlBase : IDisposable
    {
        /// <summary>
        ///     Delegate used for all control event handlers.
        /// </summary>
        /// <param name="sender">Event source.</param>
        /// <param name="arguments">Additional arguments. May be empty (EventArgs.Empty).</param>
        public delegate void GwenEventHandler<in T>(ControlBase sender, T arguments) where T : EventArgs;

        private Boolean disposed;

        private ControlBase? parent;

        /// <summary>
        ///     This is the panel's actual parent - most likely the logical
        ///     parent's InnerPanel (if it has one). You should rarely need this.
        /// </summary>
        private ControlBase actualParent;

        private ControlBase toolTip;

        private SkinBase skin;

        private Rectangle bounds;
        private Rectangle renderBounds;

        private Rectangle desiredBounds;

        private Rectangle anchorBounds;
        private Anchor anchor;

        private Size measuredSize;

        private Size minimumSize = Size.One;
        private Size maximumSize = Size.Infinity;

        protected Padding padding;
        private Margin margin;

        private Package dragAndDropPackage;

        /// <summary>
        ///     Real list of children.
        /// </summary>
        private readonly List<ControlBase> children;

        /// <summary>
        ///     Invoked when mouse pointer enters the control.
        /// </summary>
        public event GwenEventHandler<EventArgs> HoverEnter;

        /// <summary>
        ///     Invoked when mouse pointer leaves the control.
        /// </summary>
        public event GwenEventHandler<EventArgs> HoverLeave;

        /// <summary>
        ///     Invoked when control's bounds have been changed.
        /// </summary>
        public event GwenEventHandler<EventArgs> BoundsChanged;

        /// <summary>
        ///     Invoked when the control has been left-clicked. If the control is disabled, this event is still invoked.
        /// </summary>
        public virtual event GwenEventHandler<ClickedEventArgs> Clicked;

        /// <summary>
        ///     Invoked when the control has been double-left-clicked. If the control is disabled, this event is still invoked.
        /// </summary>
        public virtual event GwenEventHandler<ClickedEventArgs> DoubleClicked;

        /// <summary>
        ///     Invoked when the control has been right-clicked. If the control is disabled, this event is still invoked.
        /// </summary>
        public virtual event GwenEventHandler<ClickedEventArgs> RightClicked;

        /// <summary>
        ///     Invoked when the control has been double-right-clicked. If the control is disabled, this event is still invoked.
        /// </summary>
        public virtual event GwenEventHandler<ClickedEventArgs> DoubleRightClicked;

        /// <summary>
        ///     Returns true if any on click events are set.
        /// </summary>
        internal Boolean ClickEventAssigned => Clicked != null || RightClicked != null || DoubleClicked != null ||
                                               DoubleRightClicked != null;

        /// <summary>
        ///     Accelerator map.
        /// </summary>
        private readonly Dictionary<String, GwenEventHandler<EventArgs>> accelerators;

        /// <summary>
        ///     Logical list of children.
        /// </summary>
        public virtual List<ControlBase> Children => children;

        /// <summary>
        ///     The logical parent. It's usually what you expect, the control you've parented it to.
        /// </summary>
        public ControlBase? Parent
        {
            get => parent;
            set
            {
                if (parent == value)
                {
                    return;
                }

                if (parent != null)
                {
                    parent.RemoveChild(this, dispose: false);
                }

                parent = value;
                actualParent = null;

                if (parent != null)
                {
                    parent.AddChild(this);
                }
            }
        }

        /// <summary>
        ///     Dock position.
        /// </summary>
        public Dock Dock
        {
            get => (Dock) GetInternalFlag(InternalFlags.DockMask);
            set
            {
                if (CheckAndChangeInternalFlag(InternalFlags.DockMask, (InternalFlags) value))
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///     Is layout needed.
        /// </summary>
        protected Boolean NeedsLayout
        {
            get => IsSetInternalFlag(InternalFlags.NeedsLayout);
            set => SetInternalFlag(InternalFlags.NeedsLayout, value);
        }

        /// <summary>
        ///     Is layout done at least once for the control.
        /// </summary>
        protected Boolean LayoutDone
        {
            get => IsSetInternalFlag(InternalFlags.LayoutDone);
            set => SetInternalFlag(InternalFlags.LayoutDone, value);
        }

        /// <summary>
        ///     Current skin.
        /// </summary>
        public SkinBase Skin
        {
            get
            {
                if (skin != null)
                {
                    return skin;
                }

                if (parent != null)
                {
                    return parent.Skin;
                }

                throw new InvalidOperationException("GetSkin: null");
            }
        }

        /// <summary>
        ///     Current tooltip.
        /// </summary>
        public ControlBase ToolTip
        {
            get => toolTip;
            set
            {
                toolTip = value;

                if (toolTip != null)
                {
                    toolTip.Collapse(collapsed: true, measure: false);
                }
            }
        }

        /// <summary>
        ///     Label typed tool tip text.
        /// </summary>
        public String ToolTipText
        {
            get
            {
                if (toolTip != null && toolTip is Label)
                {
                    return ((Label) toolTip).Text;
                }

                return String.Empty;
            }
            set => SetToolTipText(value);
        }

        /// <summary>
        ///     Indicates whether this control is a menu component.
        /// </summary>
        internal virtual Boolean IsMenuComponent
        {
            get
            {
                if (parent == null)
                {
                    return false;
                }

                return parent.IsMenuComponent;
            }
        }

        /// <summary>
        ///     Determines whether the control should be clipped to its bounds while rendering.
        /// </summary>
        protected virtual Boolean ShouldClip => true;

        /// <summary>
        ///     Minimum size that the control needs to draw itself correctly. Valid after DoMeasure call. This includes margins.
        /// </summary>
        public Size MeasuredSize => measuredSize;

        public virtual Single Scale
        {
            get
            {
                if (parent != null)
                {
                    return parent.Scale;
                }

                return 1.0f;
            }
            set => throw new NotImplementedException();
        }

        public Int32 BaseUnit => Util.Ceil(Skin.BaseUnit * Scale);

        /// <summary>
        ///     Current padding - inner spacing. Padding is not valid for all controls.
        /// </summary>
        public virtual Padding Padding
        {
            get => padding;
            set
            {
                if (padding == value)
                {
                    return;
                }

                padding = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Current margin - outer spacing.
        /// </summary>
        public Margin Margin
        {
            get => margin;
            set
            {
                if (margin == value)
                {
                    return;
                }

                margin = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Vertical alignment of the control if the control is smaller than the available space.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get => (VerticalAlignment) GetInternalFlag(InternalFlags.AlignMaskV);
            set
            {
                if (CheckAndChangeInternalFlag(InternalFlags.AlignMaskV, (InternalFlags) value))
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///     Horizontal alignment of the control if the control is smaller than the available space.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get => (HorizontalAlignment) GetInternalFlag(InternalFlags.AlignMaskH);
            set
            {
                if (CheckAndChangeInternalFlag(InternalFlags.AlignMaskH, (InternalFlags) value))
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///     Indicates whether the control is on top of its parent's children.
        /// </summary>
        public virtual Boolean IsOnTop => this == Parent.children.First(); // todo: validate

        /// <summary>
        ///     Component if this control is the base of the user defined control group.
        /// </summary>
        public Component Component { get; set; }

        /// <summary>
        ///     User data associated with the control.
        /// </summary>
        public Object? UserData { get; set; }

        /// <summary>
        ///     Indicates whether the control is hovered by mouse pointer.
        /// </summary>
        public virtual Boolean IsHovered => InputHandler.HoveredControl == this;

        /// <summary>
        ///     Indicates whether the control has focus.
        /// </summary>
        public Boolean HasFocus => InputHandler.KeyboardFocus == this;

        /// <summary>
        ///     Indicates whether the control is disabled.
        /// </summary>
        public Boolean IsDisabled
        {
            get => IsSetInternalFlag(InternalFlags.Disabled);
            set => SetInternalFlag(InternalFlags.Disabled, value);
        }

        /// <summary>
        ///     Indicates whether the control is hidden.
        /// </summary>
        public virtual Boolean IsHidden
        {
            get => IsSetInternalFlag(InternalFlags.Hidden);
            set
            {
                if (CheckAndChangeInternalFlag(InternalFlags.Hidden, value))
                {
                    Redraw();
                }
            }
        }

        /// <summary>
        ///     Indicates whether the control is hidden.
        /// </summary>
        public virtual Boolean IsCollapsed
        {
            get => IsSetInternalFlag(InternalFlags.Collapsed);
            set
            {
                if (CheckAndChangeInternalFlag(InternalFlags.Collapsed, value))
                {
                    InvalidateParent();
                }
            }
        }

        /// <summary>
        ///     Determines whether the control's position should be restricted to parent's bounds.
        /// </summary>
        public Boolean RestrictToParent
        {
            get => IsSetInternalFlag(InternalFlags.RestrictToParent);
            set => SetInternalFlag(InternalFlags.RestrictToParent, value);
        }

        /// <summary>
        ///     Determines whether the control receives mouse input events.
        /// </summary>
        public Boolean MouseInputEnabled
        {
            get => IsSetInternalFlag(InternalFlags.MouseInputEnabled);
            set => SetInternalFlag(InternalFlags.MouseInputEnabled, value);
        }

        /// <summary>
        ///     Determines whether the control receives keyboard input events.
        /// </summary>
        public Boolean KeyboardInputEnabled
        {
            get => IsSetInternalFlag(InternalFlags.KeyboardInputEnabled);
            set => SetInternalFlag(InternalFlags.KeyboardInputEnabled, value);
        }

        /// <summary>
        ///     Determines whether the control receives keyboard character events.
        /// </summary>
        public Boolean KeyboardNeeded
        {
            get => IsSetInternalFlag(InternalFlags.KeyboardNeeded);
            set => SetInternalFlag(InternalFlags.KeyboardNeeded, value);
        }

        /// <summary>
        ///     Gets or sets the mouse cursor when the cursor is hovering the control.
        /// </summary>
        public Cursor Cursor { get; set; }

        /// <summary>
        ///     Indicates whether the control is tabable (can be focused by pressing Tab).
        /// </summary>
        public Boolean IsTabable
        {
            get => IsSetInternalFlag(InternalFlags.Tabable);
            set => SetInternalFlag(InternalFlags.Tabable, value);
        }

        /// <summary>
        ///     Indicates whether control's background should be drawn during rendering.
        /// </summary>
        public Boolean ShouldDrawBackground
        {
            get => IsSetInternalFlag(InternalFlags.DrawBackground);
            set => SetInternalFlag(InternalFlags.DrawBackground, value);
        }

        /// <summary>
        ///     Gets or sets the control's internal name.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        ///     Control's size and position relative to the parent.
        /// </summary>
        public Rectangle Bounds => bounds;

        /// <summary>
        ///     Bounds for the renderer.
        /// </summary>
        public Rectangle RenderBounds => renderBounds;

        /// <summary>
        ///     Bounds adjusted by padding.
        /// </summary>
        public Rectangle InnerBounds { get; private set; }

        /// <summary>
        ///     Size restriction.
        /// </summary>
        public Size MinimumSize
        {
            get => minimumSize;
            set
            {
                minimumSize = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Size restriction.
        /// </summary>
        public Size MaximumSize
        {
            get => maximumSize;
            set
            {
                maximumSize = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Determines whether hover should be drawn during rendering.
        /// </summary>
        protected Boolean ShouldDrawHover => InputHandler.MouseFocus == this || InputHandler.MouseFocus == null;

        protected virtual Boolean AccelOnlyFocus => false;
        protected virtual Boolean NeedsInputChars => false;

        /// <summary>
        ///     Indicates whether the control and its parents are visible.
        /// </summary>
        public Boolean IsVisible
        {
            get
            {
                if (IsHidden)
                {
                    return false;
                }

                if (IsCollapsed)
                {
                    return false;
                }

                if (Parent != null)
                {
                    return Parent.IsVisible;
                }

                return true;
            }
        }

        /// <summary>
        ///     Location of the control. Valid after DoArrange call.
        /// </summary>
        public Int32 ActualLeft => bounds.X;

        /// <summary>
        ///     Location of the control. Valid after DoArrange call.
        /// </summary>
        public Int32 ActualTop => bounds.Y;

        /// <summary>
        ///     Width of the control. Valid after DoArrange call.
        /// </summary>
        public Int32 ActualWidth => bounds.Width;

        /// <summary>
        ///     Height of the control. Valid after DoArrange call.
        /// </summary>
        public Int32 ActualHeight => bounds.Height;

        /// <summary>
        ///     Location of the control. Valid after DoArrange call.
        /// </summary>
        public Point ActualPosition => bounds.Location;

        /// <summary>
        ///     Size of the control. Valid after DoArrange call.
        /// </summary>
        public Size ActualSize => bounds.Size;

        /// <summary>
        ///     Location of the control. Valid after DoArrange call.
        /// </summary>
        public Int32 ActualRight => bounds.Right;

        /// <summary>
        ///     Location of the control. Valid after DoArrange call.
        /// </summary>
        public Int32 ActualBottom => bounds.Bottom;

        /// <summary>
        ///     Desired location of the control. Used only on default layout (DockLayout) if Dock property is None.
        /// </summary>
        public virtual Int32 Left
        {
            get => desiredBounds.X;
            set
            {
                if (desiredBounds.X == value)
                {
                    return;
                }

                desiredBounds.X = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Desired location of the control. Used only on default layout (DockLayout) if Dock property is None.
        /// </summary>
        public virtual Int32 Top
        {
            get => desiredBounds.Y;
            set
            {
                if (desiredBounds.Y == value)
                {
                    return;
                }

                desiredBounds.Y = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Desired size of the control. Set this value only if HorizontalAlignment is not Stretch. By default this value is
        ///     ignored.
        /// </summary>
        public virtual Int32 Width
        {
            get => desiredBounds.Width;
            set
            {
                if (desiredBounds.Width == value)
                {
                    return;
                }

                desiredBounds.Width =
                    value; /*if (m_HorizontalAlignment == HorizontalAlignment.Stretch) m_HorizontalAlignment = HorizontalAlignment.Left;*/

                InvalidateParent();
            }
        }

        /// <summary>
        ///     Desired size of the control. Set this value only if VerticalAlignment is not Stretch. By default this value is
        ///     ignored.
        /// </summary>
        public virtual Int32 Height
        {
            get => desiredBounds.Height;
            set
            {
                if (desiredBounds.Height == value)
                {
                    return;
                }

                desiredBounds.Height =
                    value; /*if (m_VerticalAlignment == VerticalAlignment.Stretch) m_VerticalAlignment = VerticalAlignment.Top;*/

                InvalidateParent();
            }
        }

        /// <summary>
        ///     Desired location of the control. Used only on default layout (DockLayout) if Dock property is None.
        /// </summary>
        public virtual Point Position
        {
            get => desiredBounds.Location;
            set
            {
                if (desiredBounds.Location == value)
                {
                    return;
                }

                desiredBounds.Location = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Desired size of the control. Set this only if both of alignments are not Stretch. By default this value is ignored.
        /// </summary>
        public virtual Size Size
        {
            get => desiredBounds.Size;
            set
            {
                if (desiredBounds.Size == value)
                {
                    return;
                }

                desiredBounds.Size = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Desired location and size of the control. Set this only if both of alignments are not Stretch. Used only on default
        ///     layout (DockLayout) if Dock property is None. By default size is ignored.
        /// </summary>
        public virtual Rectangle DesiredBounds
        {
            get => desiredBounds;
            set
            {
                if (desiredBounds == value)
                {
                    return;
                }

                desiredBounds = value;
                InvalidateParent();
            }
        }

        /// <summary>
        ///     Default location and size of the control insize the container. Used only on AnchorLayout.
        /// </summary>
        public Rectangle AnchorBounds
        {
            get => anchorBounds;
            set
            {
                if (anchorBounds == value)
                {
                    return;
                }

                anchorBounds = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     How the control is moved and/or stretched if the container size changes. Used only on AnchorLayout.
        /// </summary>
        public Anchor Anchor
        {
            get => anchor;
            set
            {
                if (anchor == value)
                {
                    return;
                }

                anchor = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Enable this if the parent of the control doesn't need to know if a new layout is needed.
        /// </summary>
        protected Boolean IsVirtualControl
        {
            get => IsSetInternalFlag(InternalFlags.VirtualControl);
            set => SetInternalFlag(InternalFlags.VirtualControl, value);
        }

        /// <summary>
        ///     Determines whether margin, padding and bounds outlines for the control will be drawn. Applied recursively to all
        ///     children.
        /// </summary>
        public Boolean DrawDebugOutlines
        {
            get => IsSetInternalFlag(InternalFlags.DrawDebugOutlines);
            set
            {
                if (!CheckAndChangeInternalFlag(InternalFlags.DrawDebugOutlines, value))
                {
                    return;
                }

                foreach (ControlBase child in Children)
                {
                    child.DrawDebugOutlines = value;
                }
            }
        }

        public Color PaddingOutlineColor { get; set; }
        public Color MarginOutlineColor { get; set; }
        public Color BoundsOutlineColor { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ControlBase" /> class.
        /// </summary>
        /// <param name="parent">Parent control. If set to null, a skin must be provided.</param>
        public ControlBase(ControlBase? parent)
        {
            children = new List<ControlBase>();
            accelerators = new Dictionary<String, GwenEventHandler<EventArgs>>();

            bounds = new Rectangle(Point.Zero, Size.Infinity);
            padding = Padding.Zero;
            margin = Margin.Zero;

            anchor = Anchor.LeftTop;

            desiredBounds = new Rectangle(x: 0, y: 0, Util.Ignore, Util.Ignore);

            anchorBounds = new Rectangle(x: 0, y: 0, width: 0, height: 0);

            SetInternalFlag(
                InternalFlags.AlignHStretch | InternalFlags.AlignVStretch | InternalFlags.DockNone,
                value: true);

            Parent = parent;

            RestrictToParent = false;

            MouseInputEnabled = false; // Edit: Changed to false. Todo: Check if this is ok.
            KeyboardInputEnabled = false;

            Invalidate();
            Cursor = Cursor.Normal;
            toolTip = null;
            IsTabable = false;
            ShouldDrawBackground = true;

            BoundsOutlineColor = Color.Red;
            MarginOutlineColor = Color.Green;
            PaddingOutlineColor = Color.Blue;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (disposed)
            {
                Debug.Fail($"Control {this} disposed twice.");
                return;
            }

            if (InputHandler.HoveredControl == this)
            {
                InputHandler.HoveredControl = null;
            }

            if (InputHandler.KeyboardFocus == this)
            {
                InputHandler.KeyboardFocus = null;
            }

            if (InputHandler.MouseFocus == this)
            {
                InputHandler.MouseFocus = null;
            }

            DragAndDrop.ControlDeleted(this);
            Net.ToolTip.ControlDeleted(this);

            foreach (ControlBase child in children)
            {
                child.Dispose();
            }

            if (toolTip != null)
            {
                toolTip.Dispose();
            }

            children.Clear();

            disposed = true;
            GC.SuppressFinalize(this);
        }
        
        ~ControlBase()
        {
            Debug.Fail($"Control {this} not disposed.");
        }

        /// <summary>
        ///     Detaches the control from canvas and adds to the deletion queue (processed in Canvas.DoThink).
        /// </summary>
        public void DelayedDelete()
        {
            GetCanvas().AddDelayedDelete(this);
        }

        public override String ToString()
        {
            var type = GetType().ToString();
            String name = String.IsNullOrWhiteSpace(Name) ? "" : " Name: " + Name;

            if (this is MenuItem menuItem)
            {
                return type + name + " [MenuItem: " + menuItem.Text + "]";
            }

            if (this is Label label)
            {
                return type + name + " [Label: " + label.Text + "]";
            }

            if (this is Text text)
            {
                return type + name + " [Text: " + text.String + "]";
            }

            return type + name;
        }

        /// <summary>
        ///     Gets the canvas (root parent) of the control.
        /// </summary>
        /// <returns></returns>
        public virtual Canvas GetCanvas()
        {
            ControlBase canvas = parent;

            if (canvas == null)
            {
                return null;
            }

            return canvas.GetCanvas();
        }

        /// <summary>
        ///     Enables the control.
        /// </summary>
        public void Enable()
        {
            IsDisabled = false;
        }

        /// <summary>
        ///     Disables the control.
        /// </summary>
        public virtual void Disable()
        {
            IsDisabled = true;
        }

        /// <summary>
        ///     Default accelerator handler.
        /// </summary>
        /// <param name="control">Event source.</param>
        /// <param name="args">Event arguments.</param>
        private void DefaultAcceleratorHandler(ControlBase control, EventArgs args)
        {
            OnAccelerator();
        }

        /// <summary>
        ///     Default accelerator handler.
        /// </summary>
        protected virtual void OnAccelerator() {}

        /// <summary>
        ///     Hides the control. Hidden controls participate in the layout process. If you don't want to layout, use Collapse.
        /// </summary>
        public virtual void Hide()
        {
            IsHidden = true;
        }

        /// <summary>
        ///     Collapse or show the control. Collapsed controls don't participate in the layout process and are hidden.
        /// </summary>
        /// <param name="collapsed">Collapse or show.</param>
        /// <param name="measure">Is layout triggered.</param>
        public virtual void Collapse(Boolean collapsed = true, Boolean measure = true)
        {
            if (!measure)
            {
                SetInternalFlag(InternalFlags.Collapsed, collapsed);
            }
            else
            {
                IsCollapsed = collapsed;
            }
        }

        /// <summary>
        ///     Shows the control.
        /// </summary>
        public virtual void Show()
        {
            IsCollapsed = false;
            IsHidden = false;
        }

        /// <summary>
        ///     Creates a tooltip for the control.
        /// </summary>
        /// <param name="text">Tooltip text.</param>
        public virtual void SetToolTipText(String text)
        {
            Label tooltip = new(this)
            {
                Parent = null,
                skin = Skin,
                Text = text,
                TextColorOverride = Skin.colors.tooltipText,
                Padding = new Padding(left: 5, top: 3, right: 5, bottom: 3)
            };
            // ToolTip doesn't have a parent
            // and that's why we need to set skin here.

            ToolTip = tooltip;
        }

        /// <summary>
        ///     Trigger the layout process.
        /// </summary>
        public virtual void Invalidate()
        {
            if (!IsVirtualControl || !LayoutDone)
            {
                NeedsLayout = true;

                if (parent != null)
                {
                    if (!parent.NeedsLayout)
                    {
                        parent.Invalidate();
                    }
                }
            }
            else
            {
                Canvas canvas = GetCanvas();

                if (canvas != null)
                {
                    canvas.AddToMeasure(this);
                }
            }
        }

        /// <summary>
        ///     Trigger parent layout process. Use this instead of Invalidate() if you know that
        ///     the parent is affected some way by the change.
        /// </summary>
        public virtual void InvalidateParent()
        {
            if (parent != null)
            {
                parent.Invalidate();
            }
        }

        /// <summary>
        ///     Sends the control to the bottom of paren't visibility stack.
        /// </summary>
        public virtual void SendToBack()
        {
            if (actualParent == null)
            {
                return;
            }

            if (actualParent.children.Count == 0)
            {
                return;
            }

            if (actualParent.children.First() == this)
            {
                return;
            }

            actualParent.children.Remove(this);
            actualParent.children.Insert(index: 0, this);
        }

        /// <summary>
        ///     Brings the control to the top of paren't visibility stack.
        /// </summary>
        public virtual void BringToFront()
        {
            if (actualParent == null)
            {
                return;
            }

            if (actualParent.children.Last() == this)
            {
                return;
            }

            actualParent.children.Remove(this);
            actualParent.children.Add(this);
            Redraw();
        }

        public virtual void BringNextToControl(ControlBase child, Boolean behind)
        {
            if (null == actualParent)
            {
                return;
            }

            Int32 index = actualParent.children.IndexOf(this);
            Int32 newIndex = actualParent.children.IndexOf(child);

            if (index == -1 || newIndex == -1)
            {
                return;
            }

            if (newIndex == 0 && !behind)
            {
                SendToBack();

                return;
            }

            if (newIndex == actualParent.children.Count - 1 && behind)
            {
                BringToFront();

                return;
            }

            actualParent.children.Remove(this);

            if (newIndex > index)
            {
                newIndex--;
            }

            if (behind)
            {
                newIndex++;
            }

            actualParent.children.Insert(newIndex, this);
        }

        /// <summary>
        ///     Finds a child by name.
        /// </summary>
        /// <param name="name">Child name.</param>
        /// <param name="recursive">Determines whether the search should be recursive.</param>
        /// <returns>Found control or null.</returns>
        public virtual ControlBase FindChildByName(String name, Boolean recursive = false)
        {
            ControlBase b = Children.Where(x => x.Name == name).FirstOrDefault();

            if (b != null)
            {
                return b;
            }

            if (recursive)
            {
                foreach (ControlBase child in Children)
                {
                    b = child.FindChildByName(name, recursive: true);

                    if (b != null)
                    {
                        return b;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Attaches specified control as a child of this one.
        /// </summary>
        /// <param name="child">Control to be added as a child.</param>
        public virtual void AddChild(ControlBase child)
        {
            children.Add(child);
            child.actualParent = this;

            OnChildAdded(child);
        }

        /// <summary>
        ///     Detaches specified control from this one.
        /// </summary>
        /// <param name="child">Child to be removed.</param>
        /// <param name="dispose">Determines whether the child should be disposed (added to delayed delete queue).</param>
        public virtual void RemoveChild(ControlBase child, Boolean dispose)
        {
            children.Remove(child);
            OnChildRemoved(child);

            if (dispose)
            {
                child.DelayedDelete();
            }
        }

        /// <summary>
        ///     Removes all children (and disposes them).
        /// </summary>
        public virtual void DeleteAllChildren()
        {
            // todo: probably shouldn't invalidate after each removal
            while (children.Count > 0)
            {
                RemoveChild(children[index: 0], dispose: true);
            }
        }

        /// <summary>
        ///     Handler invoked when a child is added.
        /// </summary>
        /// <param name="child">Child added.</param>
        protected virtual void OnChildAdded(ControlBase child) {}

        /// <summary>
        ///     Handler invoked when a child is removed.
        /// </summary>
        /// <param name="child">Child removed.</param>
        protected virtual void OnChildRemoved(ControlBase child) {}

        /// <summary>
        ///     Moves the control to a specific point, clamping on paren't bounds if RestrictToParent is set.
        ///     This function will override control location set by layout or user.
        /// </summary>
        /// <param name="x">Target x coordinate.</param>
        /// <param name="y">Target y coordinate.</param>
        public virtual void MoveTo(Int32 x, Int32 y)
        {
            if (RestrictToParent && Parent != null)
            {
                ControlBase localParent = Parent;

                if (x < Padding.Left)
                {
                    x = Padding.Left;
                }
                else if (x + ActualWidth > localParent.ActualWidth - Padding.Right)
                {
                    x = localParent.ActualWidth - ActualWidth - Padding.Right;
                }

                if (y < Padding.Top)
                {
                    y = Padding.Top;
                }
                else if (y + ActualHeight > localParent.ActualHeight - Padding.Bottom)
                {
                    y = localParent.ActualHeight - ActualHeight - Padding.Bottom;
                }
            }

            SetBounds(x, y, ActualWidth, ActualHeight);

            desiredBounds.X = bounds.X;
            desiredBounds.Y = bounds.Y;
        }

        /// <summary>
        ///     Sets the control position.
        /// </summary>
        /// <param name="x">Target x coordinate.</param>
        /// <param name="y">Target y coordinate.</param>
        /// <remarks>Bounds are reset after the next layout pass.</remarks>
        public virtual void SetPosition(Single x, Single y)
        {
            SetPosition((Int32) x, (Int32) y);
        }

        /// <summary>
        ///     Sets the control position.
        /// </summary>
        /// <param name="x">Target x coordinate.</param>
        /// <param name="y">Target y coordinate.</param>
        /// <remarks>Bounds are reset after the next layout pass.</remarks>
        public virtual void SetPosition(Int32 x, Int32 y)
        {
            SetBounds(x, y, ActualWidth, ActualHeight);
        }

        /// <summary>
        ///     Sets the control size.
        /// </summary>
        /// <param name="width">New width.</param>
        /// <param name="height">New height.</param>
        /// <returns>True if bounds changed.</returns>
        /// <remarks>Bounds are reset after the next layout pass.</remarks>
        public virtual Boolean SetSize(Int32 width, Int32 height)
        {
            return SetBounds(ActualLeft, ActualTop, width, height);
        }

        /// <summary>
        ///     Sets the control bounds.
        /// </summary>
        /// <param name="newBounds">New bounds.</param>
        /// <returns>True if bounds changed.</returns>
        /// <remarks>Bounds are reset after the next layout pass.</remarks>
        public virtual Boolean SetBounds(Rectangle newBounds)
        {
            return SetBounds(newBounds.X, newBounds.Y, newBounds.Width, newBounds.Height);
        }

        /// <summary>
        ///     Sets the control bounds.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <returns>
        ///     True if bounds changed.
        /// </returns>
        /// <remarks>Bounds are reset after the next layout pass.</remarks>
        public virtual Boolean SetBounds(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            if (bounds.X == x &&
                bounds.Y == y &&
                bounds.Width == width &&
                bounds.Height == height)
            {
                return false;
            }

            Rectangle oldBounds = Bounds;

            bounds.X = x;
            bounds.Y = y;

            bounds.Width = width;
            bounds.Height = height;

            OnBoundsChanged(oldBounds);

            Redraw();
            UpdateRenderBounds();

            if (BoundsChanged != null)
            {
                BoundsChanged.Invoke(this, EventArgs.Empty);
            }

            return true;
        }

        /// <summary>
        ///     Handler invoked when control's bounds change.
        /// </summary>
        /// <param name="oldBounds">Old bounds.</param>
        protected virtual void OnBoundsChanged(Rectangle oldBounds) {}

        /// <summary>
        ///     Handler invoked when control's scale changes.
        /// </summary>
        protected virtual void OnScaleChanged()
        {
            foreach (ControlBase child in children)
            {
                child.OnScaleChanged();
            }

            AdaptToScaleChange();
        }

        protected virtual void AdaptToScaleChange() {}

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected virtual void Render(SkinBase currentSkin) {}

        /// <summary>
        ///     Rendering logic implementation.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        internal virtual void DoRender(SkinBase currentSkin)
        {
            // If this control has a different skin,
            // then so does its children.
            if (skin != null)
            {
                currentSkin = skin;
            }

            // Do think
            Think();

            RenderRecursive(currentSkin, Bounds);

            if (DrawDebugOutlines)
            {
                currentSkin.DrawDebugOutlines(this);
            }
        }

        /// <summary>
        ///     Recursive rendering logic.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        /// <param name="clipRect">Clipping rectangle.</param>
        protected virtual void RenderRecursive(SkinBase currentSkin, Rectangle clipRect)
        {
            RendererBase render = currentSkin.Renderer;
            Point oldRenderOffset = render.RenderOffset;

            render.AddRenderOffset(clipRect);

            RenderUnder(currentSkin);

            Rectangle oldRegion = render.ClipRegion;

            if (ShouldClip)
            {
                render.AddClipRegion(clipRect);

                if (!render.ClipRegionVisible)
                {
                    render.RenderOffset = oldRenderOffset;
                    render.ClipRegion = oldRegion;

                    return;
                }

                render.StartClip();
            }

            //Render myself first
            Render(currentSkin);

            if (children.Count > 0)
            {
                //Now render my kids
                foreach (ControlBase child in children)
                {
                    if (child.IsHidden || child.IsCollapsed)
                    {
                        continue;
                    }

                    child.DoRender(currentSkin);
                }
            }

            render.ClipRegion = oldRegion;
            render.StartClip();
            RenderOver(currentSkin);

            RenderFocus(currentSkin);

            render.RenderOffset = oldRenderOffset;
        }

        /// <summary>
        ///     Sets the control's skin.
        /// </summary>
        /// <param name="currentSkin">New skin.</param>
        /// <param name="doChildren">Determines whether to change children skin.</param>
        public virtual void SetSkin(SkinBase currentSkin, Boolean doChildren = false)
        {
            if (skin == currentSkin)
            {
                return;
            }

            skin = currentSkin;

            Redraw();
            OnSkinChanged(currentSkin);

            if (doChildren)
            {
                foreach (ControlBase child in children)
                {
                    child.SetSkin(currentSkin, doChildren: true);
                }
            }
        }

        /// <summary>
        ///     Handler invoked when control's skin changes.
        /// </summary>
        /// <param name="newSkin">New skin.</param>
        protected virtual void OnSkinChanged(SkinBase newSkin) {}

        /// <summary>
        ///     Handler invoked on mouse wheel event.
        /// </summary>
        /// <param name="delta">Scroll delta.</param>
        protected virtual Boolean OnMouseWheeled(Int32 delta)
        {
            if (actualParent != null)
            {
                return actualParent.OnMouseWheeled(delta);
            }

            return false;
        }

        /// <summary>
        ///     Invokes mouse wheeled event (used by input system).
        /// </summary>
        internal Boolean InputMouseWheeled(Int32 delta)
        {
            return OnMouseWheeled(delta);
        }

        /// <summary>
        ///     Handler invoked on mouse moved event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="dx">X change.</param>
        /// <param name="dy">Y change.</param>
        protected virtual void OnMouseMoved(Int32 x, Int32 y, Int32 dx, Int32 dy) {}

        /// <summary>
        ///     Invokes mouse moved event (used by input system).
        /// </summary>
        internal void InputMouseMoved(Int32 x, Int32 y, Int32 dx, Int32 dy)
        {
            OnMouseMoved(x, y, dx, dy);
        }

        /// <summary>
        ///     Handler invoked on mouse click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected virtual void OnMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            if (down && Clicked != null)
            {
                Clicked(this, new ClickedEventArgs(x, y, down: true));
            }
        }

        /// <summary>
        ///     Invokes left mouse click event (used by input system).
        /// </summary>
        internal void InputMouseClickedLeft(Int32 x, Int32 y, Boolean down)
        {
            OnMouseClickedLeft(x, y, down);
        }

        /// <summary>
        ///     Handler invoked on mouse click (right) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="down">If set to <c>true</c> mouse button is down.</param>
        protected virtual void OnMouseClickedRight(Int32 x, Int32 y, Boolean down)
        {
            if (down && RightClicked != null)
            {
                RightClicked(this, new ClickedEventArgs(x, y, down: true));
            }
        }

        /// <summary>
        ///     Invokes right mouse click event (used by input system).
        /// </summary>
        internal void InputMouseClickedRight(Int32 x, Int32 y, Boolean down)
        {
            OnMouseClickedRight(x, y, down);
        }

        /// <summary>
        ///     Handler invoked on mouse double click (left) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        protected virtual void OnMouseDoubleClickedLeft(Int32 x, Int32 y)
        {
            // [omeg] should this be called?
            // [halfofastaple] Maybe. Technically, a double click is still technically a single click. However, this shouldn't be called here, and
            //					Should be called by the event handler.
            OnMouseClickedLeft(x, y, down: true);

            if (DoubleClicked != null)
            {
                DoubleClicked(this, new ClickedEventArgs(x, y, down: true));
            }
        }

        /// <summary>
        ///     Invokes left double mouse click event (used by input system).
        /// </summary>
        internal void InputMouseDoubleClickedLeft(Int32 x, Int32 y)
        {
            OnMouseDoubleClickedLeft(x, y);
        }

        /// <summary>
        ///     Handler invoked on mouse double click (right) event.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        protected virtual void OnMouseDoubleClickedRight(Int32 x, Int32 y)
        {
            // [halfofastaple] See: OnMouseDoubleClicked for discussion on triggering single clicks in a double click event
            OnMouseClickedRight(x, y, down: true);

            if (DoubleRightClicked != null)
            {
                DoubleRightClicked(this, new ClickedEventArgs(x, y, down: true));
            }
        }

        /// <summary>
        ///     Invokes right double mouse click event (used by input system).
        /// </summary>
        internal void InputMouseDoubleClickedRight(Int32 x, Int32 y)
        {
            OnMouseDoubleClickedRight(x, y);
        }

        /// <summary>
        ///     Handler invoked on mouse cursor entering control's bounds.
        /// </summary>
        protected virtual void OnMouseEntered()
        {
            if (HoverEnter != null)
            {
                HoverEnter.Invoke(this, EventArgs.Empty);
            }

            if (ToolTip != null)
            {
                Net.ToolTip.Enable(this);
            }
            else if (Parent != null && Parent.ToolTip != null)
            {
                Net.ToolTip.Enable(Parent);
            }

            Redraw();
        }

        /// <summary>
        ///     Invokes mouse enter event (used by input system).
        /// </summary>
        internal void InputMouseEntered()
        {
            OnMouseEntered();
        }

        /// <summary>
        ///     Handler invoked on mouse cursor leaving control's bounds.
        /// </summary>
        protected virtual void OnMouseLeft()
        {
            if (HoverLeave != null)
            {
                HoverLeave.Invoke(this, EventArgs.Empty);
            }

            if (ToolTip != null)
            {
                Net.ToolTip.Disable(this);
            }

            Redraw();
        }

        /// <summary>
        ///     Invokes mouse leave event (used by input system).
        /// </summary>
        internal void InputMouseLeft()
        {
            OnMouseLeft();
        }

        /// <summary>
        ///     Focuses the control.
        /// </summary>
        public virtual void Focus()
        {
            if (InputHandler.KeyboardFocus == this)
            {
                return;
            }

            if (InputHandler.KeyboardFocus != null)
            {
                InputHandler.KeyboardFocus.OnLostKeyboardFocus();
            }

            InputHandler.KeyboardFocus = this;
            OnKeyboardFocus();
            Redraw();
        }

        /// <summary>
        ///     Unfocuses the control.
        /// </summary>
        public virtual void Blur()
        {
            if (InputHandler.KeyboardFocus != this)
            {
                return;
            }

            InputHandler.KeyboardFocus = null;
            OnLostKeyboardFocus();
            Redraw();
        }

        /// <summary>
        ///     Control has been clicked - invoked by input system. Windows use it to propagate activation.
        /// </summary>
        public virtual void Touch()
        {
            if (Parent != null)
            {
                Parent.OnChildTouched(this);
            }
        }

        protected virtual void OnChildTouched(ControlBase control)
        {
            Touch();
        }

        /// <summary>
        ///     Gets a child by its coordinates.
        /// </summary>
        /// <param name="x">Child X.</param>
        /// <param name="y">Child Y.</param>
        /// <returns>Control or null if not found.</returns>
        public virtual ControlBase GetControlAt(Int32 x, Int32 y)
        {
            if (IsHidden || IsCollapsed)
            {
                return null;
            }

            if (x < 0 || y < 0 || x >= ActualWidth || y >= ActualHeight)
            {
                return null;
            }

            // todo: convert to linq FindLast
            IEnumerable<ControlBase>
                rev = ((IList<ControlBase>) children)
                    .Reverse(); // IList.Reverse creates new list, List.Reverse works in place.. go figure

            foreach (ControlBase child in rev)
            {
                ControlBase found = child.GetControlAt(x - child.ActualLeft, y - child.ActualTop);

                if (found != null)
                {
                    return found;
                }
            }

            if (!MouseInputEnabled)
            {
                return null;
            }

            return this;
        }

        /// <summary>
        ///     Override this method if you need to customize the layout process.
        /// </summary>
        /// <param name="availableSize">
        ///     Available size for the control. The control doesn't need to use all the space that is
        ///     available.
        /// </param>
        /// <returns>Minimum size that the control needs to draw itself correctly.</returns>
        protected virtual Size Measure(Size availableSize)
        {
            Int32 parentWidth = padding.Left + padding.Right;
            Int32 parentHeight = padding.Top + padding.Bottom;
            Int32 childrenWidth = padding.Left + padding.Right;
            Int32 childrenHeight = padding.Top + padding.Bottom;

            foreach (ControlBase child in children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                Dock dock = child.Dock;

                if (dock == Dock.None || dock == Dock.Fill)
                {
                    continue;
                }

                Size childSize = new(
                    Math.Max(val1: 0, availableSize.Width - childrenWidth),
                    Math.Max(
                        val1: 0,
                        availableSize.Height - childrenHeight));

                childSize = child.DoMeasure(childSize);

                switch (child.Dock)
                {
                    case Dock.Left:
                    case Dock.Right:
                        parentHeight = Math.Max(parentHeight, childrenHeight + childSize.Height);
                        childrenWidth += childSize.Width;

                        break;
                    case Dock.Top:
                    case Dock.Bottom:
                        parentWidth = Math.Max(parentWidth, childrenWidth + childSize.Width);
                        childrenHeight += childSize.Height;

                        break;
                }
            }

            foreach (ControlBase child in children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                Dock dock = child.Dock;

                if (dock != Dock.Fill)
                {
                    continue;
                }

                Size childSize = new(
                    Math.Max(val1: 0, availableSize.Width - childrenWidth),
                    Math.Max(
                        val1: 0,
                        availableSize.Height - childrenHeight));

                childSize = child.DoMeasure(childSize);

                parentWidth = Math.Max(parentWidth, childrenWidth + childSize.Width);
                parentHeight = Math.Max(parentHeight, childrenHeight + childSize.Height);
            }

            foreach (ControlBase child in children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                Dock dock = child.Dock;

                if (dock != Dock.None)
                {
                    continue;
                }

                Size childSize = child.DoMeasure(availableSize);

                parentWidth = Math.Max(parentWidth, child.Left + childSize.Width);
                parentHeight = Math.Max(parentHeight, child.Top + childSize.Height);
            }

            parentWidth = Math.Max(parentWidth, childrenWidth);
            parentHeight = Math.Max(parentHeight, childrenHeight);

            return new Size(parentWidth, parentHeight);
        }

        /// <summary>
        ///     Call this method for all child controls.
        /// </summary>
        /// <param name="availableWidth">Width that is available for the control.</param>
        /// <param name="availableHeight">Height that is available for the control.</param>
        /// <returns>Minimum size that the control needs to draw itself correctly.</returns>
        public Size DoMeasure(Int32 availableWidth, Int32 availableHeight)
        {
            return DoMeasure(new Size(availableWidth, availableHeight));
        }

        /// <summary>
        ///     Call this method for all child controls.
        /// </summary>
        /// <param name="availableSize">Size that is available for the control.</param>
        /// <returns>Minimum size that the control needs to draw itself correctly.</returns>
        public Size DoMeasure(Size availableSize)
        {
            availableSize -= margin;

            if (!Util.IsIgnore(desiredBounds.Width))
            {
                availableSize.Width = Math.Min(availableSize.Width, desiredBounds.Width);
            }

            if (!Util.IsIgnore(desiredBounds.Height))
            {
                availableSize.Height = Math.Min(availableSize.Height, desiredBounds.Height);
            }

            availableSize.Width = Util.Clamp(availableSize.Width, minimumSize.Width, maximumSize.Width);
            availableSize.Height = Util.Clamp(availableSize.Height, minimumSize.Height, maximumSize.Height);

            Size size = Measure(availableSize);

            if (Util.IsInfinity(size.Width) || Util.IsInfinity(size.Height))
            {
                throw new InvalidOperationException("Measured size cannot be infinity.");
            }

            if (!Util.IsIgnore(desiredBounds.Width))
            {
                size.Width = desiredBounds.Width;
            }

            if (!Util.IsIgnore(desiredBounds.Height))
            {
                size.Height = desiredBounds.Height;
            }

            size.Width = Util.Clamp(size.Width, minimumSize.Width, maximumSize.Width);
            size.Height = Util.Clamp(size.Height, minimumSize.Height, maximumSize.Height);

            if (size.Width > availableSize.Width)
            {
                size.Width = availableSize.Width;
            }

            if (size.Height > availableSize.Height)
            {
                size.Height = availableSize.Height;
            }

            size += margin;

            measuredSize = size;

            return measuredSize;
        }

        /// <summary>
        ///     Override this method if you need to customize the layout process. Usually, if you override Measure, you also need
        ///     to override Arrange.
        /// </summary>
        /// <param name="finalSize">Space that the control should fill.</param>
        /// <returns>Space that the control filled.</returns>
        protected virtual Size Arrange(Size finalSize)
        {
            Int32 childrenLeft = padding.Left;
            Int32 childrenTop = padding.Top;
            Int32 childrenRight = padding.Right;
            Int32 childrenBottom = padding.Bottom;

            foreach (ControlBase child in children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                Dock dock = child.Dock;

                if (dock == Dock.None || dock == Dock.Fill)
                {
                    continue;
                }

                Size childSize = child.MeasuredSize;

                Rectangle localBounds =
                    new(
                        childrenLeft,
                        childrenTop,
                        Math.Max(val1: 0, finalSize.Width - (childrenLeft + childrenRight)),
                        Math.Max(val1: 0, finalSize.Height - (childrenTop + childrenBottom)));

                switch (dock)
                {
                    case Dock.Left:
                        childrenLeft += childSize.Width;
                        localBounds.Width = childSize.Width;

                        break;
                    case Dock.Right:
                        childrenRight += childSize.Width;
                        localBounds.X = Math.Max(val1: 0, finalSize.Width - childrenRight);
                        localBounds.Width = childSize.Width;

                        break;
                    case Dock.Top:
                        childrenTop += childSize.Height;
                        localBounds.Height = childSize.Height;

                        break;
                    case Dock.Bottom:
                        childrenBottom += childSize.Height;
                        localBounds.Y = Math.Max(val1: 0, finalSize.Height - childrenBottom);
                        localBounds.Height = childSize.Height;

                        break;
                }

                child.DoArrange(localBounds);
            }

            foreach (ControlBase child in children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                Dock dock = child.Dock;

                if (dock != Dock.Fill)
                {
                    continue;
                }

                Rectangle localBounds =
                    new(
                        childrenLeft,
                        childrenTop,
                        Math.Max(val1: 0, finalSize.Width - (childrenLeft + childrenRight)),
                        Math.Max(val1: 0, finalSize.Height - (childrenTop + childrenBottom)));

                InnerBounds = localBounds;

                child.DoArrange(localBounds);
            }

            foreach (ControlBase child in children)
            {
                if (child.IsCollapsed)
                {
                    continue;
                }

                Dock dock = child.Dock;

                if (dock != Dock.None)
                {
                    continue;
                }

                Rectangle localBounds =
                    new(child.Left, child.Top, finalSize.Width - child.Left, finalSize.Height - child.Top);

                child.DoArrange(localBounds);
            }

            return finalSize;
        }

        /// <summary>
        ///     Call this method for all child controls.
        /// </summary>
        /// <param name="x">Final horizontal location. This includes margins.</param>
        /// <param name="y">Final vertical location. This includes margins.</param>
        /// <param name="width">Final width of the control. This includes margins.</param>
        /// <param name="height">Final height of the control. This includes margins.</param>
        public void DoArrange(Int32 x, Int32 y, Int32 width, Int32 height)
        {
            DoArrange(new Rectangle(x, y, width, height));
        }

        /// <summary>
        ///     Call this method for all child controls.
        /// </summary>
        /// <param name="finalRect">Final location and size of the control. This includes margins.</param>
        public void DoArrange(Rectangle finalRect)
        {
            Size finalSize = finalRect.Size;

            HorizontalAlignment halign = HorizontalAlignment;
            VerticalAlignment valign = VerticalAlignment;

            if (halign != HorizontalAlignment.Stretch)
            {
                finalSize.Width = measuredSize.Width;
            }

            if (valign != VerticalAlignment.Stretch)
            {
                finalSize.Height = measuredSize.Height;
            }

            finalSize -= margin;

            if (!Util.IsIgnore(desiredBounds.Width))
            {
                finalSize.Width = Math.Min(finalRect.Width, desiredBounds.Width);
            }

            if (!Util.IsIgnore(desiredBounds.Height))
            {
                finalSize.Height = Math.Min(finalRect.Height, desiredBounds.Height);
            }

            Size arrangedSize = Arrange(finalSize);

            if (!Util.IsIgnore(desiredBounds.Width))
            {
                arrangedSize.Width = desiredBounds.Width;
            }
            else if (halign == HorizontalAlignment.Stretch)
            {
                arrangedSize.Width = finalSize.Width;
            }

            if (!Util.IsIgnore(desiredBounds.Height))
            {
                arrangedSize.Height = desiredBounds.Height;
            }
            else if (valign == VerticalAlignment.Stretch)
            {
                arrangedSize.Height = finalSize.Height;
            }

            arrangedSize.Width = Util.Clamp(arrangedSize.Width, minimumSize.Width, maximumSize.Width);
            arrangedSize.Height = Util.Clamp(arrangedSize.Height, minimumSize.Height, maximumSize.Height);

            if (arrangedSize.Width > finalSize.Width)
            {
                arrangedSize.Width = finalSize.Width;
            }

            if (arrangedSize.Height > finalSize.Height)
            {
                arrangedSize.Height = finalSize.Height;
            }

            Size areaSize = finalRect.Size;
            areaSize -= margin;

            Point offset = Point.Zero;

            if (halign == HorizontalAlignment.Center)
            {
                offset.X = (areaSize.Width - arrangedSize.Width) / 2;
            }
            else if (halign == HorizontalAlignment.Right)
            {
                offset.X = areaSize.Width - arrangedSize.Width;
            }

            if (valign == VerticalAlignment.Center)
            {
                offset.Y = (areaSize.Height - arrangedSize.Height) / 2;
            }
            else if (valign == VerticalAlignment.Bottom)
            {
                offset.Y = areaSize.Height - arrangedSize.Height;
            }

            SetBounds(
                finalRect.Left + margin.Left + offset.X,
                finalRect.Top + margin.Top + offset.Y,
                arrangedSize.Width,
                arrangedSize.Height);

            NeedsLayout = false;
            LayoutDone = true;
        }

        /// <summary>
        ///     Invoke the layout process for the control and it's children.
        /// </summary>
        public virtual void DoLayout()
        {
            Measure(bounds.Size);
            Arrange(bounds.Size);

            NeedsLayout = false;
            LayoutDone = true;
        }

        /// <summary>
        ///     Recursively check tabs, focus etc.
        /// </summary>
        protected virtual void RecurseControls()
        {
            if (IsHidden || IsCollapsed)
            {
                return;
            }

            foreach (ControlBase child in Children)
            {
                child.RecurseControls();
            }

            if (IsTabable)
            {
                if (GetCanvas().firstTab == null)
                {
                    GetCanvas().firstTab = this;
                }

                if (GetCanvas().nextTab == null)
                {
                    GetCanvas().nextTab = this;
                }
            }

            if (InputHandler.KeyboardFocus == this)
            {
                GetCanvas().nextTab = null;
            }
        }

        /// <summary>
        ///     Checks if the given control is a child of this instance.
        /// </summary>
        /// <param name="child">Control to examine.</param>
        /// <returns>True if the control is our child.</returns>
        public Boolean IsChild(ControlBase child)
        {
            return children.Contains(child);
        }

        /// <summary>
        ///     Converts local coordinates to canvas coordinates.
        /// </summary>
        /// <param name="pnt">Local coordinates.</param>
        /// <returns>Canvas coordinates.</returns>
        public virtual Point LocalPosToCanvas(Point pnt)
        {
            if (actualParent != null)
            {
                Int32 x = pnt.X + ActualLeft;
                Int32 y = pnt.Y + ActualTop;

                return actualParent.LocalPosToCanvas(new Point(x, y));
            }

            return pnt;
        }

        /// <summary>
        ///     Converts canvas coordinates to local coordinates.
        /// </summary>
        /// <param name="pnt">Canvas coordinates.</param>
        /// <returns>Local coordinates.</returns>
        public virtual Point CanvasPosToLocal(Point pnt)
        {
            if (actualParent != null)
            {
                Int32 x = pnt.X - ActualLeft;
                Int32 y = pnt.Y - ActualTop;

                return actualParent.CanvasPosToLocal(new Point(x, y));
            }

            return pnt;
        }

        /// <summary>
        ///     Closes all menus recursively.
        /// </summary>
        public virtual void CloseMenus()
        {
            // todo: not very efficient with the copying and recursive closing, maybe store currently open menus somewhere (canvas)?
            ControlBase[] childrenCopy = children.ToArray();

            foreach (ControlBase child in childrenCopy)
            {
                child.CloseMenus();
            }
        }

        /// <summary>
        ///     Copies Bounds to RenderBounds.
        /// </summary>
        protected virtual void UpdateRenderBounds()
        {
            renderBounds.X = 0;
            renderBounds.Y = 0;

            renderBounds.Width = bounds.Width;
            renderBounds.Height = bounds.Height;
        }

        /// <summary>
        ///     Sets mouse cursor to current cursor.
        /// </summary>
        public virtual void UpdateCursor()
        {
            GwenPlatform.SetCursor(Cursor);
        }

        // giver
        public virtual Package DragAndDrop_GetPackage(Int32 x, Int32 y)
        {
            return dragAndDropPackage;
        }

        // giver
        public virtual Boolean DragAndDrop_Draggable()
        {
            if (dragAndDropPackage == null)
            {
                return false;
            }

            return dragAndDropPackage.IsDraggable;
        }

        // giver
        public virtual void DragAndDrop_SetPackage(Boolean draggable, String name = "", Object userData = null)
        {
            if (dragAndDropPackage == null)
            {
                dragAndDropPackage = new Package { IsDraggable = draggable, Name = name, UserData = userData };
            }
        }

        // giver
        public virtual Boolean DragAndDrop_ShouldStartDrag()
        {
            return true;
        }

        // giver
        public virtual void DragAndDrop_StartDragging(Package package, Int32 x, Int32 y)
        {
            package.HoldOffset = CanvasPosToLocal(new Point(x, y));
            package.DrawControl = this;
        }

        // giver
        public virtual void DragAndDrop_EndDragging(Boolean success, Int32 x, Int32 y) {}

        // receiver
        public virtual Boolean DragAndDrop_HandleDrop(Package p, Int32 x, Int32 y)
        {
            DragAndDrop.SourceControl.Parent = this;

            return true;
        }

        // receiver
        public virtual void DragAndDrop_HoverEnter(Package p, Int32 x, Int32 y) {}

        // receiver
        public virtual void DragAndDrop_HoverLeave(Package p) {}

        // receiver
        public virtual void DragAndDrop_Hover(Package p, Int32 x, Int32 y) {}

        // receiver
        public virtual Boolean DragAndDrop_CanAcceptPackage(Package p)
        {
            return false;
        }

        /// <summary>
        ///     Handles keyboard accelerator.
        /// </summary>
        /// <param name="accelerator">Accelerator text.</param>
        /// <returns>True if handled.</returns>
        internal virtual Boolean HandleAccelerator(String accelerator)
        {
            if (InputHandler.KeyboardFocus == this || !AccelOnlyFocus)
            {
                if (accelerators.ContainsKey(accelerator))
                {
                    accelerators[accelerator].Invoke(this, EventArgs.Empty);

                    return true;
                }
            }

            return children.Any(child => child.HandleAccelerator(accelerator));
        }

        /// <summary>
        ///     Adds keyboard accelerator.
        /// </summary>
        /// <param name="accelerator">Accelerator text.</param>
        /// <param name="handler">Handler.</param>
        public void AddAccelerator(String accelerator, GwenEventHandler<EventArgs> handler)
        {
            accelerator = accelerator.Trim().ToUpperInvariant();
            accelerators[accelerator] = handler;
        }

        /// <summary>
        ///     Adds keyboard accelerator with a default handler.
        /// </summary>
        /// <param name="accelerator">Accelerator text.</param>
        public void AddAccelerator(String accelerator)
        {
            accelerators[accelerator] = DefaultAcceleratorHandler;
        }

        /// <summary>
        ///     Re-renders the control, invalidates cached texture.
        /// </summary>
        public virtual void Redraw()
        {
            UpdateColors();

            if (parent != null)
            {
                parent.Redraw();
            }
        }

        /// <summary>
        ///     Updates control colors.
        /// </summary>
        /// <remarks>
        ///     Used in composite controls like lists to differentiate row colors etc.
        /// </remarks>
        public virtual void UpdateColors() {}

        /// <summary>
        ///     Handler for keyboard events.
        /// </summary>
        /// <param name="key">Key pressed.</param>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyPressed(GwenMappedKey key, Boolean down = true)
        {
            var handled = false;

            switch (key)
            {
                case GwenMappedKey.Tab:
                    handled = OnKeyTab(down);

                    break;
                case GwenMappedKey.Space:
                    handled = OnKeySpace(down);

                    break;
                case GwenMappedKey.Home:
                    handled = OnKeyHome(down);

                    break;
                case GwenMappedKey.End:
                    handled = OnKeyEnd(down);

                    break;
                case GwenMappedKey.Return:
                    handled = OnKeyReturn(down);

                    break;
                case GwenMappedKey.Backspace:
                    handled = OnKeyBackspace(down);

                    break;
                case GwenMappedKey.Delete:
                    handled = OnKeyDelete(down);

                    break;
                case GwenMappedKey.Right:
                    handled = OnKeyRight(down);

                    break;
                case GwenMappedKey.Left:
                    handled = OnKeyLeft(down);

                    break;
                case GwenMappedKey.Up:
                    handled = OnKeyUp(down);

                    break;
                case GwenMappedKey.Down:
                    handled = OnKeyDown(down);

                    break;
                case GwenMappedKey.Escape:
                    handled = OnKeyEscape(down);

                    break;
            }

            if (!handled && Parent != null)
            {
                Parent.OnKeyPressed(key, down);
            }

            return handled;
        }

        /// <summary>
        ///     Invokes key press event (used by input system).
        /// </summary>
        internal Boolean InputKeyPressed(GwenMappedKey key, Boolean down = true)
        {
            return OnKeyPressed(key, down);
        }

        /// <summary>
        ///     Handler for keyboard events.
        /// </summary>
        /// <param name="key">Key pressed.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyReleaseed(GwenMappedKey key)
        {
            return OnKeyPressed(key, down: false);
        }

        /// <summary>
        ///     Handler for Tab keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyTab(Boolean down)
        {
            if (!down)
            {
                return true;
            }

            if (GetCanvas().nextTab != null)
            {
                GetCanvas().nextTab.Focus();
                Redraw();
            }

            return true;
        }

        /// <summary>
        ///     Handler for Space keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeySpace(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Return keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyReturn(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Backspace keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyBackspace(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Delete keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyDelete(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Right Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyRight(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Left Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyLeft(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Home keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyHome(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for End keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyEnd(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Up Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyUp(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Down Arrow keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyDown(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Escape keyboard event.
        /// </summary>
        /// <param name="down">Indicates whether the key was pressed or released.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnKeyEscape(Boolean down)
        {
            return false;
        }

        /// <summary>
        ///     Handler for Paste event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnPaste(ControlBase from, EventArgs args) {}

        /// <summary>
        ///     Handler for Copy event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnCopy(ControlBase from, EventArgs args) {}

        /// <summary>
        ///     Handler for Cut event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnCut(ControlBase from, EventArgs args) {}

        /// <summary>
        ///     Handler for Select All event.
        /// </summary>
        /// <param name="from">Source control.</param>
        /// <param name="args">Event arguments.</param>
        protected virtual void OnSelectAll(ControlBase from, EventArgs args) {}

        internal void InputCopy(ControlBase from)
        {
            OnCopy(from, EventArgs.Empty);
        }

        internal void InputPaste(ControlBase from)
        {
            OnPaste(from, EventArgs.Empty);
        }

        internal void InputCut(ControlBase from)
        {
            OnCut(from, EventArgs.Empty);
        }

        internal void InputSelectAll(ControlBase from)
        {
            OnSelectAll(from, EventArgs.Empty);
        }

        /// <summary>
        ///     Renders the focus overlay.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected virtual void RenderFocus(SkinBase currentSkin)
        {
            if (InputHandler.KeyboardFocus != this)
            {
                return;
            }

            if (!IsTabable)
            {
                return;
            }

            currentSkin.DrawKeyboardHighlight(this, RenderBounds, offset: 3);
        }

        /// <summary>
        ///     Renders under the actual control (shadows etc).
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected virtual void RenderUnder(SkinBase currentSkin) {}

        /// <summary>
        ///     Renders over the actual control (overlays).
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected virtual void RenderOver(SkinBase currentSkin) {}

        /// <summary>
        ///     Called during rendering.
        /// </summary>
        public virtual void Think() {}

        /// <summary>
        ///     Handler for gaining keyboard focus.
        /// </summary>
        protected virtual void OnKeyboardFocus() {}

        /// <summary>
        ///     Handler for losing keyboard focus.
        /// </summary>
        protected virtual void OnLostKeyboardFocus() {}

        /// <summary>
        ///     Handler for character input event.
        /// </summary>
        /// <param name="chr">Character typed.</param>
        /// <returns>True if handled.</returns>
        protected virtual Boolean OnChar(Char chr)
        {
            return false;
        }

        internal Boolean InputChar(Char chr)
        {
            return OnChar(chr);
        }

        private InternalFlags internalFlags;

        private Boolean IsSetInternalFlag(InternalFlags flag)
        {
            return (internalFlags & flag) != 0;
        }

        private InternalFlags GetInternalFlag(InternalFlags mask)
        {
            return internalFlags & mask;
        }

        private void SetInternalFlag(InternalFlags flag, Boolean value)
        {
            if (value)
            {
                internalFlags |= flag;
            }
            else
            {
                internalFlags &= ~flag;
            }
        }

        private Boolean CheckAndChangeInternalFlag(InternalFlags flag, Boolean value)
        {
            Boolean oldValue = (internalFlags & flag) != 0;

            if (oldValue == value)
            {
                return false;
            }

            if (value)
            {
                internalFlags |= flag;
            }
            else
            {
                internalFlags &= ~flag;
            }

            return true;
        }

        private Boolean CheckAndChangeInternalFlag(InternalFlags mask, InternalFlags flag)
        {
            if ((internalFlags & mask) == flag)
            {
                return false;
            }

            internalFlags = (internalFlags & ~mask) | flag;

            return true;
        }

        [Flags]
        internal enum InternalFlags
        {
            // AlignH
            AlignHLeft = 1 << 0,
            AlignHCenter = 1 << 1,
            AlignHRight = 1 << 2,
            AlignHStretch = 1 << 3,
            AlignMaskH = AlignHLeft | AlignHCenter | AlignHRight | AlignHStretch,

            // AlignV
            AlignVTop = 1 << 4,
            AlignVCenter = 1 << 5,
            AlignVBottom = 1 << 6,
            AlignVStretch = 1 << 7,
            AlignMaskV = AlignVTop | AlignVCenter | AlignVBottom | AlignVStretch,

            // Dock
            DockNone = 1 << 8,
            DockLeft = 1 << 9,
            DockTop = 1 << 10,
            DockRight = 1 << 11,
            DockBottom = 1 << 12,
            DockFill = 1 << 13,
            DockMask = DockNone | DockLeft | DockTop | DockRight | DockBottom | DockFill,

            // Flags
            VirtualControl = 1 << 14,
            NeedsLayout = 1 << 15,
            LayoutDone = 1 << 16,
            Disabled = 1 << 17,
            Hidden = 1 << 18,
            Collapsed = 1 << 19,
            DrawDebugOutlines = 1 << 20,
            RestrictToParent = 1 << 21,
            MouseInputEnabled = 1 << 22,
            KeyboardInputEnabled = 1 << 23,
            DrawBackground = 1 << 24,
            Tabable = 1 << 25,
            KeyboardNeeded = 1 << 26
        }
    }
}
