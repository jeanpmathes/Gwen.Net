using System.Collections.Generic;

namespace Gwen.Net.RichText
{
    public class Document
    {
        public Document() {}

        public Document(string text)
        {
            Paragraph paragraph = new();
            paragraph.Text(text);
            Paragraphs.Add(paragraph);
        }

        public List<Paragraph> Paragraphs { get; } = new();

        public Paragraph Paragraph(Margin margin = new(), int firstIndent = 0, int remainingIndent = 0)
        {
            Paragraph paragraph = new(margin, firstIndent, remainingIndent);

            Paragraphs.Add(paragraph);

            return paragraph;
        }

        public ImageParagraph Image(string imageName, Size? imageSize = null, Rectangle? textureRect = null,
            Color? imageColor = null, Margin margin = new(), int indent = 0)
        {
            ImageParagraph paragraph = new(margin, indent);
            paragraph.Image(imageName, imageSize, textureRect, imageColor);

            Paragraphs.Add(paragraph);

            return paragraph;
        }

        public ImageParagraph Image(string imageName, Margin margin, int indent)
        {
            ImageParagraph paragraph = new(margin, indent);
            paragraph.Image(imageName);

            Paragraphs.Add(paragraph);

            return paragraph;
        }

        public ImageParagraph Image(string imageName, Size imageSize, Margin margin = new(), int indent = 0)
        {
            ImageParagraph paragraph = new(margin, indent);
            paragraph.Image(imageName, imageSize);

            Paragraphs.Add(paragraph);

            return paragraph;
        }
    }
}