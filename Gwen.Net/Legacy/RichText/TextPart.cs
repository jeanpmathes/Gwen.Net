using System;
using System.Collections.Generic;

namespace Gwen.Net.Legacy.RichText
{
    public class TextPart : Part
    {
        private static readonly Char[] separator =
        {
            ' ',
            '\n',
            '\r'
        };

        public TextPart(String text)
        {
            Text = text;
            Color = null;
        }

        public TextPart(String text, Color color)
        {
            Text = text;
            Color = color;
        }

        public String Text { get; }

        public Color? Color { get; }

        public Font Font { get; protected set; }

        public override String[] Split(ref Font splitFont)
        {
            Font = splitFont;

            return StringSplit(Text);
        }

        protected static String[] StringSplit(String str)
        {
            List<String> strings = new();
            Int32 len = str.Length;
            var index = 0;
            Int32 i;

            while (index < len)
            {
                i = str.IndexOfAny(separator, index);

                if (i == index)
                {
                    if (str[i] == ' ')
                    {
                        strings.Add(" ");

                        while (index < len && str[index] == ' ')
                        {
                            index++;
                        }
                    }
                    else
                    {
                        strings.Add("\n");
                        index++;

                        if (index < len && str[index - 1] == '\r' && str[index] == '\n')
                        {
                            index++;
                        }
                    }
                }
                else if (i != -1)
                {
                    if (str[i] == ' ')
                    {
                        strings.Add(str.Substring(index, i - index + 1));
                        index = i + 1;
                    }
                    else
                    {
                        strings.Add(str.Substring(index, i - index));
                        index = i;
                    }
                }
                else
                {
                    strings.Add(str.Substring(index));

                    break;
                }
            }

            return strings.ToArray();
        }
    }
}
