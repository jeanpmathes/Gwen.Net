﻿using System;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Static text label.
    /// </summary>
    public class Label : ControlBase
    {
        protected readonly Text text;
        private Alignment align;
        private Padding textPadding;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="Label" /> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Label(ControlBase parent) : base(parent)
        {
            text = new Text(this);

            MouseInputEnabled = false;
            Alignment = Alignment.Left | Alignment.Top;
        }

        /// <summary>
        ///     Text alignment.
        /// </summary>
        public Alignment Alignment
        {
            get => align;
            set
            {
                align = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Text.
        /// </summary>
        public virtual String Text
        {
            get => text.String;
            set => text.String = value;
        }

        /// <summary>
        ///     Font.
        /// </summary>
        public Font Font
        {
            get => text.Font;
            set
            {
                text.Font = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        public Color TextColor
        {
            get => text.TextColor;
            set => text.TextColor = value;
        }

        /// <summary>
        ///     Override text color (used by tooltips).
        /// </summary>
        public Color TextColorOverride
        {
            get => text.TextColorOverride;
            set => text.TextColorOverride = value;
        }

        /// <summary>
        ///     Text override - used to display different string.
        /// </summary>
        public String TextOverride
        {
            get => text.TextOverride;
            set => text.TextOverride = value;
        }

        /// <summary>
        ///     Determines if the control should autosize to its text.
        /// </summary>
        public Boolean AutoSizeToContents
        {
            get => text.AutoSizeToContents;
            set
            {
                text.AutoSizeToContents = value;
                IsVirtualControl = !value;

                if (value)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///     Text padding.
        /// </summary>
        public Padding TextPadding
        {
            get => textPadding;
            set
            {
                textPadding = value;
                Invalidate();
            }
        }

        public override event GwenEventHandler<ClickedEventArgs> Clicked
        {
            add
            {
                base.Clicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.Clicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        public override event GwenEventHandler<ClickedEventArgs> DoubleClicked
        {
            add
            {
                base.DoubleClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.DoubleClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        public override event GwenEventHandler<ClickedEventArgs> RightClicked
        {
            add
            {
                base.RightClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.RightClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        public override event GwenEventHandler<ClickedEventArgs> DoubleRightClicked
        {
            add
            {
                base.DoubleRightClicked += value;
                MouseInputEnabled = ClickEventAssigned;
            }
            remove
            {
                base.DoubleRightClicked -= value;
                MouseInputEnabled = ClickEventAssigned;
            }
        }

        /// <summary>
        ///     Returns index of the character closest to specified point (in canvas coordinates).
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected virtual Point GetClosestCharacter(Int32 x, Int32 y)
        {
            return new Point(text.GetClosestCharacter(text.CanvasPosToLocal(new Point(x, y))), y: 0);
        }

        /// <summary>
        ///     Handler for text changed event.
        /// </summary>
        protected virtual void OnTextChanged() {}

        protected override Size Measure(Size availableSize)
        {
            return text.DoMeasure(availableSize) + textPadding + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            Size innerSize = finalSize - textPadding - Padding;
            Rectangle rect = new(Point.Zero, Size.Min(text.MeasuredSize, innerSize));

            if ((align & Alignment.CenterH) != 0)
            {
                rect.X = (innerSize.Width - text.MeasuredSize.Width) / 2;
            }
            else if ((align & Alignment.Right) != 0)
            {
                rect.X = innerSize.Width - text.MeasuredSize.Width;
            }

            if ((align & Alignment.CenterV) != 0)
            {
                rect.Y = (innerSize.Height - text.MeasuredSize.Height) / 2;
            }
            else if ((align & Alignment.Bottom) != 0)
            {
                rect.Y = innerSize.Height - text.MeasuredSize.Height;
            }

            rect.Offset(textPadding + Padding);

            text.DoArrange(rect);

            return finalSize;
        }

        /// <summary>
        ///     Gets the coordinates of specified character.
        /// </summary>
        /// <param name="index">Character index.</param>
        /// <returns>Character coordinates (local).</returns>
        public virtual Point GetCharacterPosition(Int32 index)
        {
            Point p = text.GetCharacterPosition(index);

            return new Point(p.X + text.ActualLeft, p.Y + text.ActualTop);
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            // The text element will render itself.
        }
    }
}
