using System;
using System.Collections.Generic;

namespace Gwen.Net
{
    /// <summary>
    ///     Misc utility functions.
    /// </summary>
    public static class Util
    {
        public const Int32 Ignore = -1;

        public const Int32 Infinity = 0xfffffff;

        public static Int32 Ceil(Single x)
        {
            return (Int32) Math.Ceiling(x);
        }

        public static Rectangle FloatRect(Single x, Single y, Single w, Single h)
        {
            return new((Int32) x, (Int32) y, (Int32) w, (Int32) h);
        }

        public static Int32 Clamp(Int32 x, Int32 min, Int32 max)
        {
            if (x < min)
            {
                return min;
            }

            if (x > max)
            {
                return max;
            }

            return x;
        }

        public static Single Clamp(Single x, Single min, Single max)
        {
            if (x < min)
            {
                return min;
            }

            if (x > max)
            {
                return max;
            }

            return x;
        }

        public static Rectangle ClampRectToRect(Rectangle inside, Rectangle outside, Boolean clampSize = false)
        {
            if (inside.X < outside.X)
            {
                inside.X = outside.X;
            }

            if (inside.Y < outside.Y)
            {
                inside.Y = outside.Y;
            }

            if (inside.Right > outside.Right)
            {
                if (clampSize)
                {
                    inside.Width = outside.Width;
                }
                else
                {
                    inside.X = outside.Right - inside.Width;
                }
            }

            if (inside.Bottom > outside.Bottom)
            {
                if (clampSize)
                {
                    inside.Height = outside.Height;
                }
                else
                {
                    inside.Y = outside.Bottom - inside.Height;
                }
            }

            return inside;
        }

        public static HSV ToHSV(this Color color)
        {
            HSV hsv = new();

            Single r = color.R / 255.0f;
            Single g = color.G / 255.0f;
            Single b = color.B / 255.0f;

            Single max = Math.Max(r, Math.Max(g, b));
            Single min = Math.Min(r, Math.Min(g, b));

            hsv.V = max;

            Single delta = max - min;

            if (max != 0)
            {
                hsv.S = delta / max;
            }
            else
            {
                hsv.S = 0;
            }

            if (delta != 0)
            {
                if (r == max)
                {
                    hsv.H = (g - b) / delta;
                }
                else if (g == max)
                {
                    hsv.H = 2 + ((b - r) / delta);
                }
                else
                {
                    hsv.H = 4 + ((r - g) / delta);
                }

                hsv.H *= 60;

                if (hsv.H < 0)
                {
                    hsv.H += 360;
                }
            }
            else
            {
                hsv.H = 0;
            }

            return hsv;
        }

        public static Color HSVToColor(Single h, Single s, Single v)
        {
            Int32 hi = Convert.ToInt32(Math.Floor(h / 60)) % 6;
            Single f = (h / 60) - (Single) Math.Floor(h / 60);

            v = v * 255;
            var va = Convert.ToInt32(v);
            var p = Convert.ToInt32(v * (1 - s));
            var q = Convert.ToInt32(v * (1 - f * s));
            var t = Convert.ToInt32(v * (1 - (1 - f) * s));

            if (hi == 0)
            {
                return new Color(a: 255, va, t, p);
            }

            if (hi == 1)
            {
                return new Color(a: 255, q, va, p);
            }

            if (hi == 2)
            {
                return new Color(a: 255, p, va, t);
            }

            if (hi == 3)
            {
                return new Color(a: 255, p, q, va);
            }

            if (hi == 4)
            {
                return new Color(a: 255, t, p, va);
            }

            return new Color(a: 255, va, p, q);
        }

        // can't create extension operators
        public static Color Subtract(this Color color, Color other)
        {
            return new(color.A - other.A, color.R - other.R, color.G - other.G, color.B - other.B);
        }

        public static Color Add(this Color color, Color other)
        {
            return new(color.A + other.A, color.R + other.R, color.G + other.G, color.B + other.B);
        }

        public static Color Multiply(this Color color, Single amount)
        {
            return new(color.A, (Int32) (color.R * amount), (Int32) (color.G * amount), (Int32) (color.B * amount));
        }

        public static Rectangle Add(this Rectangle r, Rectangle other)
        {
            return new(r.X + other.X, r.Y + other.Y, r.Width + other.Width, r.Height + other.Height);
        }

        /// <summary>
        ///     Splits a string but keeps the separators intact.
        /// </summary>
        /// <param name="text">String to split.</param>
        /// <param name="separators">Separator characters.</param>
        /// <returns>Split strings.</returns>
        public static String[] SplitAndKeep(String text, String separators)
        {
            List<String> strings = new();
            var offset = 0;
            Int32 length = text.Length;
            Int32 sepLen = separators.Length;
            Int32 i = text.IndexOf(separators, StringComparison.Ordinal);
            String word;

            while (i != -1)
            {
                word = text.Substring(offset, i - offset);

                if (!String.IsNullOrWhiteSpace(word))
                {
                    strings.Add(word);
                }

                offset = i + sepLen;
                i = text.IndexOf(separators, offset, StringComparison.Ordinal);
                offset -= sepLen;
            }

            strings.Add(text.Substring(offset, length - offset));

            return strings.ToArray();
        }

        public static Boolean IsIgnore(Int32 value)
        {
            return value == Ignore;
        }

        public static Boolean IsInfinity(Int32 value)
        {
            return value > 0xffffff;
        }
    }
}
