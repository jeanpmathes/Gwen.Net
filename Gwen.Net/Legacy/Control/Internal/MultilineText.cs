using System;
using System.Collections.Generic;
using System.Linq;

namespace Gwen.Net.Legacy.Control.Internal
{
    /// <summary>
    ///     Multi line text.
    /// </summary>
    public class MultilineText : ControlBase
    {
        private readonly List<Text> textLines = new();
        private Font font;

        private Int32 lineHeight;

        public MultilineText(ControlBase parent)
            : base(parent) {}

        /// <summary>
        ///     Get or set text line.
        /// </summary>
        /// <param name="index">Line index.</param>
        /// <returns>Text.</returns>
        public String this[Int32 index]
        {
            get
            {
                if (index < 0 && index >= textLines.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return textLines[index].String;
            }
            set
            {
                if (index < 0 && index >= textLines.Count)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                textLines[index].String = value;

                Invalidate();
            }
        }

        /// <summary>
        ///     Returns the number of lines that are in the Multiline Text Box.
        /// </summary>
        public Int32 TotalLines => textLines.Count;

        /// <summary>
        ///     Height of the text line in pixels.
        /// </summary>
        public Int32 LineHeight
        {
            get
            {
                if (lineHeight == 0)
                {
                    lineHeight = Util.Ceil(Font.FontMetrics.LineSpacingPixels);
                }

                return lineHeight;
            }
        }

        /// <summary>
        ///     Gets and sets the text to display to the user. Each line is separated by
        ///     an Environment.NetLine character.
        /// </summary>
        public String Text
        {
            get => String.Join(Environment.NewLine, textLines.Select(t => t.String));
            set => SetText(value);
        }

        /// <summary>
        ///     Font.
        /// </summary>
        public Font Font
        {
            get => font;
            set
            {
                font = value;

                foreach (Text textCtrl in textLines)
                {
                    textCtrl.Font = value;
                }

                lineHeight = 0;
                Invalidate();
            }
        }

        /// <summary>
        ///     Sets the text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        public void SetText(String text)
        {
            String[] lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split(separator: '\n');
            Int32 index;

            for (index = 0; index < lines.Length; index++)
            {
                if (textLines.Count > index)
                {
                    textLines[index].String = lines[index];
                }
                else
                {
                    InsertLine(index, lines[index]);
                }
            }

            for (; index < textLines.Count; index++)
            {
                RemoveLine(lines.Length);
            }

            Invalidate();
        }

        /// <summary>
        ///     Inserts text at a position.
        /// </summary>
        /// <param name="text">Text to insert.</param>
        /// <param name="position">Position where to insert.</param>
        public Point InsertText(String text, Point position)
        {
            if (position.Y < 0 || position.Y >= textLines.Count)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if (position.X < 0 || position.X > textLines[position.Y].String.Length)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if (text.Contains("\r") || text.Contains("\n"))
            {
                String[] newLines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split(separator: '\n');

                String oldLineStart = textLines[position.Y].String.Substring(startIndex: 0, position.X);
                String oldLineEnd = textLines[position.Y].String.Substring(position.X);

                textLines[position.Y].String = oldLineStart + newLines[0]; // First line

                for (var i = 1; i < newLines.Length - 1; i++)
                {
                    InsertLine(position.Y + i, newLines[i]); // Middle lines
                }

                InsertLine(position.Y + newLines.Length - 1, newLines[newLines.Length - 1] + oldLineEnd); // Last line

                Invalidate();

                return new Point(newLines[newLines.Length - 1].Length, position.Y + newLines.Length - 1);
            }

            String str = textLines[position.Y].String;
            str = str.Insert(position.X, text);
            textLines[position.Y].String = str;

            Invalidate();

            return new Point(position.X + text.Length, position.Y);
        }

        /// <summary>
        ///     Add line to the end.
        /// </summary>
        /// <param name="text">Text to add.</param>
        public void AddLine(String text)
        {
            InsertLine(textLines.Count, text);
        }

        /// <summary>
        ///     Insert a new line.
        /// </summary>
        /// <param name="index">Index where to insert.</param>
        /// <param name="text">Text to insert.</param>
        public void InsertLine(Int32 index, String text)
        {
            if (index < 0 || index > textLines.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            Text textCtrl = new(this);
            textCtrl.Font = font;
            textCtrl.AutoSizeToContents = false;
            textCtrl.TextColor = Skin.colors.textBoxColors.text;
            textCtrl.String = text;

            textLines.Insert(index, textCtrl);
            Invalidate();
        }

        /// <summary>
        ///     Replace text line.
        /// </summary>
        /// <param name="index">Index what to replace.</param>
        /// <param name="text">New text.</param>
        public void ReplaceLine(Int32 index, String text)
        {
            if (index < 0 || index >= textLines.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            textLines[index].String = text;

            Invalidate();
        }

        /// <summary>
        ///     Remove the line at the index.
        /// </summary>
        /// <param name="index">Index to remove.</param>
        public void RemoveLine(Int32 index)
        {
            if (index < 0 || index >= textLines.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            RemoveChild(textLines[index], dispose: true);
            textLines.RemoveAt(index);

            Invalidate();
        }

        /// <summary>
        ///     Remove all text.
        /// </summary>
        public void Clear()
        {
            foreach (Text textCtrl in textLines)
            {
                RemoveChild(textCtrl, dispose: true);
            }

            textLines.Clear();

            Invalidate();
        }

        /// <summary>
        ///     Gets the coordinates of specified character position in the text.
        /// </summary>
        /// <param name="position">Character position.</param>
        /// <returns>Character position in local coordinates.</returns>
        public Point GetCharacterPosition(Point position)
        {
            if (position.Y < 0 || position.Y >= textLines.Count)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            if (position.X < 0 || position.X > textLines[position.Y].String.Length)
            {
                throw new ArgumentOutOfRangeException("position");
            }

            String currLine = textLines[position.Y].String.Substring(
                startIndex: 0,
                Math.Min(position.X, textLines[position.Y].Length));

            Point p = new(Skin.Renderer.MeasureText(Font, currLine).Width, position.Y * LineHeight);

            return new Point(p.X + Padding.Left, p.Y + Padding.Top);
        }

        /// <summary>
        ///     Returns position of the character closest to specified point.
        /// </summary>
        /// <param name="p">Point in local coordinates.</param>
        /// <returns>Character position.</returns>
        public Point GetClosestCharacter(Point p)
        {
            p.X -= Padding.Left;
            p.Y -= Padding.Top;

            Point best = new(x: 0, y: 0);

            /* Find the appropriate Y (always pick a row whichever the mouse currently is on) */
            best.Y = Util.Clamp(p.Y / LineHeight, min: 0, textLines.Count - 1);

            /* Find the best X, closest char */
            best.X = textLines[best.Y].GetClosestCharacter(p);

            return best;
        }

        protected override Size Measure(Size availableSize)
        {
            availableSize -= Padding;

            var width = 0;
            var height = 0;
            Int32 currentLineHeight = LineHeight;

            foreach (Text line in textLines)
            {
                Size size = line.DoMeasure(availableSize);
                availableSize.Height -= currentLineHeight;

                if (size.Width > width)
                {
                    width = size.Width;
                }

                height += currentLineHeight;
            }

            return new Size(width + 2, height) + Padding;
        }

        protected override Size Arrange(Size finalSize)
        {
            finalSize -= Padding;

            Int32 width = finalSize.Width;
            Int32 y = Padding.Top;
            Int32 currentLineHeight = LineHeight;

            foreach (Text line in textLines)
            {
                line.DoArrange(new Rectangle(Padding.Left, y, width, line.MeasuredSize.Height));
                y += currentLineHeight;
            }

            y += Padding.Bottom;

            return new Size(finalSize.Width + 2 + Padding.Left + Padding.Right, y);
        }
    }
}
