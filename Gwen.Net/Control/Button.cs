using System;
using System.Diagnostics.CodeAnalysis;
using Gwen.Net.Control.Internal;
using Gwen.Net.Skin;

namespace Gwen.Net.Control
{
    /// <summary>
    ///     Image alignment inside the button
    /// </summary>
    [Flags]
    public enum ImageAlign
    {
        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        CenterV = 1 << 4,
        CenterH = 1 << 5,
        Fill = 1 << 6,
        LeftSide = 1 << 7,
        Above = 1 << 8,
        Center = CenterV | CenterH
    }

    /// <summary>
    ///     Button control.
    /// </summary>
    public class Button : ButtonBase
    {
        private Alignment align;
        
        private ImagePanel? image;
        private ImageAlign imageAlign;
        
        private Text? text;
        private Padding textPadding;

        /// <summary>
        ///     Control constructor.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public Button(ControlBase parent)
            : base(parent)
        {
            Alignment = Alignment.Center;
            TextPadding = new Padding(left: 3, top: 3, right: 3, bottom: 3);
        }
        
        /// <summary>
        ///     Text.
        /// </summary>
        public virtual string? Text
        {
            get => text?.String;
            set
            {
                if (value == null)
                {
                    RemoveText();
                }
                else
                {
                    EnsureText();
                    text.String = value;
                }
            }
        }

        /// <summary>
        ///     Font.
        /// </summary>
        public Font? Font
        {
            get => text?.Font;
            set
            {
                if (value == null)
                {
                    RemoveText();
                }
                else
                {
                    EnsureText();
                    text.Font = value;
                }
            }
        }

        /// <summary>
        ///     Text color.
        /// </summary>
        public Color? TextColor
        {
            get => text?.TextColor;
            set
            {
                if (value == null)
                {
                    RemoveText();
                }
                else
                {
                    EnsureText();
                    text.TextColor = value.Value;
                }
            }
        }

        /// <summary>
        ///     Override text color (used by tooltips).
        /// </summary>
        public Color? TextColorOverride
        {
            get => text?.TextColorOverride;
            set
            {
                if (value == null)
                {
                    RemoveText();
                }
                else
                {
                    EnsureText();
                    text.TextColorOverride = value.Value;
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
                if (value == textPadding)
                {
                    return;
                }

                textPadding = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Text alignment.
        /// </summary>
        public Alignment Alignment
        {
            get => align;
            set
            {
                if (value == align)
                {
                    return;
                }

                align = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Determines how the image is aligned inside the button.
        /// </summary>
        public ImageAlign ImageAlign
        {
            get => imageAlign;
            set
            {
                if (imageAlign == value)
                {
                    return;
                }

                imageAlign = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Returns the current image name (or null if no image set) or set a new image.
        /// </summary>
        public string? ImageName
        {
            get => image?.ImageName;
            set
            {
                if (image != null && image.ImageName == value) 
                    return;

                SetImage(value, imageAlign);
            }
        }

        /// <summary>
        ///     Gets or sets the size of the image.
        /// </summary>
        public Size ImageSize
        {
            get
            {
                if (image != null)
                {
                    return image.ImageSize;
                }

                return Size.Zero;
            }
            set
            {
                if (image == null)
                {
                    return;
                }

                image.ImageSize = value;
            }
        }

        /// <summary>
        ///     Gets or sets the texture coordinates of the image in pixels.
        /// </summary>
        public Rectangle ImageTextureRect
        {
            get
            {
                if (image != null)
                {
                    return image.TextureRect;
                }

                return Rectangle.Empty;
            }
            set
            {
                if (image == null)
                {
                    return;
                }

                image.TextureRect = value;
            }
        }

        /// <summary>
        ///     Gets or sets the color of the image.
        /// </summary>
        public Color ImageColor
        {
            get
            {
                if (image != null)
                {
                    return image.ImageColor;
                }

                return Color.White;
            }
            set
            {
                if (image == null)
                {
                    return;
                }

                image.ImageColor = value;
            }
        }

        [MemberNotNull(nameof(text))]
        private void EnsureText()
        {
            text ??= new Text(this);
        }
        
        private void RemoveText()
        {
            if (text == null) return;

            RemoveChild(text, dispose: true);
                
            text = null;
        }

        /// <summary>
        ///     Renders the control using specified skin.
        /// </summary>
        /// <param name="currentSkin">Skin to use.</param>
        protected override void Render(SkinBase currentSkin)
        {
            base.Render(currentSkin);

            if (ShouldDrawBackground)
            {
                bool drawDepressed = IsDepressed && IsHovered;

                if (IsToggle)
                {
                    drawDepressed = drawDepressed || ToggleState;
                }

                bool bDrawHovered = IsHovered && ShouldDrawHover;

                currentSkin.DrawButton(this, drawDepressed, bDrawHovered, IsDisabled);
            }
        }

        /// <summary>
        ///     Sets the button's image.
        /// </summary>
        /// <param name="textureName">Texture name. Null to remove.</param>
        /// <param name="newImageAlign">Determines how the image should be aligned.</param>
        public virtual void SetImage(string? textureName, ImageAlign newImageAlign = ImageAlign.LeftSide)
        {
            if (string.IsNullOrEmpty(textureName))
            {
                image?.Dispose();
                image = null;

                return;
            }

            image ??= new ImagePanel(this);

            image.ImageName = textureName;
            image.MouseInputEnabled = false;
            imageAlign = newImageAlign;
            image.SendToBack();

            Invalidate();
        }

        protected override Size Measure(Size availableSize)
        {
            if (image == null)
            {
                Size size = Size.Zero;

                if (text != null)
                {
                    size = text.DoMeasure(availableSize);
                }

                size += textPadding + Padding;

                return size;
            }

            Size imageSize = image.DoMeasure(availableSize);
            Size textSize = text != null ? text.DoMeasure(availableSize) + textPadding : Size.Zero;

            Size totalSize;

            switch (imageAlign)
            {
                case ImageAlign.LeftSide:
                    totalSize = new Size(textSize.Width + imageSize.Width, Math.Max(imageSize.Height, textSize.Height));

                    break;
                case ImageAlign.Above:
                    totalSize = new Size(Math.Max(imageSize.Width, textSize.Width), textSize.Height + imageSize.Height);

                    break;
                default:
                    totalSize = Size.Max(imageSize, textSize);

                    break;
            }

            totalSize += Padding;

            return totalSize;
        }

        protected override Size Arrange(Size finalSize)
        {
            if (image == null)
            {
                if (text == null) return finalSize;

                Size innerSize = finalSize - Padding;
                Size textSize = text.MeasuredSize + textPadding;
                Rectangle rect = new(Point.Zero, textSize);

                if ((align & Alignment.CenterH) != 0)
                {
                    rect.X = (innerSize.Width - rect.Width) / 2;
                }
                else if ((align & Alignment.Right) != 0)
                {
                    rect.X = innerSize.Width - rect.Width;
                }

                if ((align & Alignment.CenterV) != 0)
                {
                    rect.Y = (innerSize.Height - rect.Height) / 2;
                }
                else if ((align & Alignment.Bottom) != 0)
                {
                    rect.Y = innerSize.Height - rect.Height;
                }

                rect.Offset(textPadding + Padding);

                text.DoArrange(rect);
            }
            else
            {
                Size innerSize = finalSize - Padding;

                Size imageSize = image.MeasuredSize;
                Size textSize = text != null ? text.MeasuredSize + textPadding : Size.Zero;

                Rectangle rect;

                switch (imageAlign)
                {
                    case ImageAlign.LeftSide:
                        rect = new Rectangle(
                            Point.Zero,
                            textSize.Width + imageSize.Width,
                            Math.Max(imageSize.Height, textSize.Height));

                        break;
                    case ImageAlign.Above:
                        rect = new Rectangle(
                            Point.Zero,
                            Math.Max(imageSize.Width, textSize.Width),
                            textSize.Height + imageSize.Height);

                        break;
                    default:
                        rect = new Rectangle(Point.Zero, textSize);

                        break;
                }

                if ((align & Alignment.Right) != 0)
                {
                    rect.X = innerSize.Width - rect.Width;
                }
                else if ((align & Alignment.CenterH) != 0)
                {
                    rect.X = (innerSize.Width - rect.Width) / 2;
                }

                if ((align & Alignment.Bottom) != 0)
                {
                    rect.Y = innerSize.Height - rect.Height;
                }
                else if ((align & Alignment.CenterV) != 0)
                {
                    rect.Y = (innerSize.Height - rect.Height) / 2;
                }

                Rectangle imageRect = new(Point.Zero, imageSize);
                Rectangle textRect = new(rect.Location, text?.MeasuredSize ?? Size.Zero);

                switch (imageAlign)
                {
                    case ImageAlign.LeftSide:
                        imageRect.Location = new Point(rect.X, rect.Y + ((rect.Height - imageSize.Height) / 2));

                        textRect.Location = new Point(
                            rect.X + imageSize.Width,
                            rect.Y + ((rect.Height - textSize.Height) / 2));

                        break;
                    case ImageAlign.Above:
                        imageRect.Location = new Point(rect.X + ((rect.Width - imageSize.Width) / 2), rect.Y);

                        textRect.Location = new Point(
                            rect.X + ((rect.Width - textSize.Width) / 2),
                            rect.Y + imageSize.Height);

                        break;
                    case ImageAlign.Fill:
                        imageRect.Size = innerSize;

                        break;
                    default:
                        if ((imageAlign & ImageAlign.Right) != 0)
                        {
                            imageRect.X = innerSize.Width - imageRect.Width;
                        }
                        else if ((imageAlign & ImageAlign.CenterH) != 0)
                        {
                            imageRect.X = (innerSize.Width - imageRect.Width) / 2;
                        }

                        if ((imageAlign & ImageAlign.Bottom) != 0)
                        {
                            imageRect.Y = innerSize.Height - imageRect.Height;
                        }
                        else if ((imageAlign & ImageAlign.CenterV) != 0)
                        {
                            imageRect.Y = (innerSize.Height - imageRect.Height) / 2;
                        }

                        break;
                }

                imageRect.Offset(Padding);
                image.DoArrange(imageRect);

                if (text != null)
                {
                    textRect.Offset(Padding + textPadding);
                    text.DoArrange(textRect);
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Whether to consider the toggle state when coloring the button.
        /// </summary>
        public bool UseToggleStateForColor { get; set; } = true;
        
        /// <summary>
        ///     Updates control colors.
        /// </summary>
        public override void UpdateColors()
        {
            if (text == null)
            {
                return;
            }

            if (IsDisabled)
            {
                TextColor = Skin.colors.buttonColors.disabled;

                return;
            }

            if (IsDepressed || (UseToggleStateForColor && IsToggle && ToggleState))
            {
                TextColor = Skin.colors.buttonColors.down;

                return;
            }

            if (IsHovered)
            {
                TextColor = Skin.colors.buttonColors.hover;

                return;
            }

            TextColor = Skin.colors.buttonColors.normal;
        }
    }
}
