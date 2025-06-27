using System;

namespace Gwen.Net.RichText
{
    public class ImageParagraph : Paragraph
    {
        public ImageParagraph(Margin margin = new(), Int32 indent = 0)
            : base(margin, indent, indent) {}

        public String ImageName { get; private set; }

        public Size? ImageSize { get; private set; }

        public Rectangle? TextureRect { get; private set; }

        public Color? ImageColor { get; private set; }

        public ImageParagraph Image(String imageName, Size? imageSize = null, Rectangle? textureRect = null,
            Color? imageColor = null)
        {
            ImageName = imageName;

            if (imageSize != null)
            {
                ImageSize = imageSize;
            }

            if (textureRect != null)
            {
                TextureRect = textureRect;
            }

            if (imageColor != null)
            {
                ImageColor = imageColor;
            }

            return this;
        }
    }
}