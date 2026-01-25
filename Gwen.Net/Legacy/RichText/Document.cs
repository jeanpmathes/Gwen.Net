using System;
using System.Collections.Generic;

namespace Gwen.Net.Legacy.RichText
{
    public class Document
    {
        public Document() {}

        public Document(String text)
        {
            Paragraph paragraph = new();
            paragraph.Text(text);
            Paragraphs.Add(paragraph);
        }

        public List<Paragraph> Paragraphs { get; } = new();

        public Paragraph Paragraph(Margin margin = new(), Int32 firstIndent = 0, Int32 remainingIndent = 0)
        {
            Paragraph paragraph = new(margin, firstIndent, remainingIndent);

            Paragraphs.Add(paragraph);

            return paragraph;
        }

        public ImageParagraph Image(String imageName, Size? imageSize = null, Rectangle? textureRect = null,
            Color? imageColor = null, Margin margin = new(), Int32 indent = 0)
        {
            ImageParagraph paragraph = new(margin, indent);
            paragraph.Image(imageName, imageSize, textureRect, imageColor);

            Paragraphs.Add(paragraph);

            return paragraph;
        }

        public ImageParagraph Image(String imageName, Margin margin, Int32 indent)
        {
            ImageParagraph paragraph = new(margin, indent);
            paragraph.Image(imageName);

            Paragraphs.Add(paragraph);

            return paragraph;
        }

        public ImageParagraph Image(String imageName, Size imageSize, Margin margin = new(), Int32 indent = 0)
        {
            ImageParagraph paragraph = new(margin, indent);
            paragraph.Image(imageName, imageSize);

            Paragraphs.Add(paragraph);

            return paragraph;
        }
    }
}