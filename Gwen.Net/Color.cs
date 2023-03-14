using System.Collections.Generic;

namespace Gwen.Net
{
    /// <summary>
    ///     Represent ARGB color.
    /// </summary>
    public struct Color
    {
        /// <summary>
        ///     Red value.
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        ///     Green value.
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        ///     Blue value.
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        ///     Alpha value.
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        ///     Initializes new color. Alpha value is 255.
        /// </summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        public Color(int r, int g, int b)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = 255;
        }

        /// <summary>
        ///     Initializes new color.
        /// </summary>
        /// <param name="a">Alpha value.</param>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        public Color(int a, int r, int g, int b)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)a;
        }

        /// <summary>
        ///     Initializes new color.
        /// </summary>
        /// <param name="value">32 bit color value as 0xAARRGGBB. Alpha value 0 is treated as 255.</param>
        public Color(uint value)
        {
            unchecked
            {
                R = (byte)((value >> 16) & 0xff);
                G = (byte)((value >> 8) & 0xff);
                B = (byte)(value & 0xff);
                A = (byte)((value >> 24) & 0xff);

                if (A == 0)
                {
                    A = 255;
                }
            }
        }

        /// <summary>
        ///     Get color by name.
        /// </summary>
        /// <param name="name">HTML color name supported by browsers.</param>
        /// <returns>Color if named value exists, color black otherwise.</returns>
        public static Color FromName(string name)
        {
            Color color;

            if (namedColors.TryGetValue(name, out color))
            {
                return color;
            }

            return Black;
        }

        public override string ToString()
        {
            return $"R = {R} G = {G} B = {B} A = {A}";
        }

        public static readonly Color AliceBlue = new(value: 0xF0F8FF);
        public static readonly Color AntiqueWhite = new(value: 0xFAEBD7);
        public static readonly Color Aqua = new(value: 0x00FFFF);
        public static readonly Color Aquamarine = new(value: 0x7FFFD4);
        public static readonly Color Azure = new(value: 0xF0FFFF);
        public static readonly Color Beige = new(value: 0xF5F5DC);
        public static readonly Color Bisque = new(value: 0xFFE4C4);
        public static readonly Color Black = new(value: 0x000000);
        public static readonly Color BlanchedAlmond = new(value: 0xFFEBCD);
        public static readonly Color Blue = new(value: 0x0000FF);
        public static readonly Color BlueViolet = new(value: 0x8A2BE2);
        public static readonly Color Brown = new(value: 0xA52A2A);
        public static readonly Color BurlyWood = new(value: 0xDEB887);
        public static readonly Color CadetBlue = new(value: 0x5F9EA0);
        public static readonly Color Chartreuse = new(value: 0x7FFF00);
        public static readonly Color Chocolate = new(value: 0xD2691E);
        public static readonly Color Coral = new(value: 0xFF7F50);
        public static readonly Color CornflowerBlue = new(value: 0x6495ED);
        public static readonly Color Cornsilk = new(value: 0xFFF8DC);
        public static readonly Color Crimson = new(value: 0xDC143C);
        public static readonly Color Cyan = new(value: 0x00FFFF);
        public static readonly Color DarkBlue = new(value: 0x00008B);
        public static readonly Color DarkCyan = new(value: 0x008B8B);
        public static readonly Color DarkGoldenRod = new(value: 0xB8860B);
        public static readonly Color DarkGray = new(value: 0xA9A9A9);
        public static readonly Color DarkGrey = new(value: 0xA9A9A9);
        public static readonly Color DarkGreen = new(value: 0x006400);
        public static readonly Color DarkKhaki = new(value: 0xBDB76B);
        public static readonly Color DarkMagenta = new(value: 0x8B008B);
        public static readonly Color DarkOliveGreen = new(value: 0x556B2F);
        public static readonly Color DarkOrange = new(value: 0xFF8C00);
        public static readonly Color DarkOrchid = new(value: 0x9932CC);
        public static readonly Color DarkRed = new(value: 0x8B0000);
        public static readonly Color DarkSalmon = new(value: 0xE9967A);
        public static readonly Color DarkSeaGreen = new(value: 0x8FBC8F);
        public static readonly Color DarkSlateBlue = new(value: 0x483D8B);
        public static readonly Color DarkSlateGray = new(value: 0x2F4F4F);
        public static readonly Color DarkSlateGrey = new(value: 0x2F4F4F);
        public static readonly Color DarkTurquoise = new(value: 0x00CED1);
        public static readonly Color DarkViolet = new(value: 0x9400D3);
        public static readonly Color DeepPink = new(value: 0xFF1493);
        public static readonly Color DeepSkyBlue = new(value: 0x00BFFF);
        public static readonly Color DimGray = new(value: 0x696969);
        public static readonly Color DimGrey = new(value: 0x696969);
        public static readonly Color DodgerBlue = new(value: 0x1E90FF);
        public static readonly Color FireBrick = new(value: 0xB22222);
        public static readonly Color FloralWhite = new(value: 0xFFFAF0);
        public static readonly Color ForestGreen = new(value: 0x228B22);
        public static readonly Color Fuchsia = new(value: 0xFF00FF);
        public static readonly Color Gainsboro = new(value: 0xDCDCDC);
        public static readonly Color GhostWhite = new(value: 0xF8F8FF);
        public static readonly Color Gold = new(value: 0xFFD700);
        public static readonly Color GoldenRod = new(value: 0xDAA520);
        public static readonly Color Gray = new(value: 0x808080);
        public static readonly Color Grey = new(value: 0x808080);
        public static readonly Color Green = new(value: 0x008000);
        public static readonly Color GreenYellow = new(value: 0xADFF2F);
        public static readonly Color HoneyDew = new(value: 0xF0FFF0);
        public static readonly Color HotPink = new(value: 0xFF69B4);
        public static readonly Color IndianRed = new(value: 0xCD5C5C);
        public static readonly Color Indigo = new(value: 0x4B0082);
        public static readonly Color Ivory = new(value: 0xFFFFF0);
        public static readonly Color Khaki = new(value: 0xF0E68C);
        public static readonly Color Lavender = new(value: 0xE6E6FA);
        public static readonly Color LavenderBlush = new(value: 0xFFF0F5);
        public static readonly Color LawnGreen = new(value: 0x7CFC00);
        public static readonly Color LemonChiffon = new(value: 0xFFFACD);
        public static readonly Color LightBlue = new(value: 0xADD8E6);
        public static readonly Color LightCoral = new(value: 0xF08080);
        public static readonly Color LightCyan = new(value: 0xE0FFFF);
        public static readonly Color LightGoldenRodYellow = new(value: 0xFAFAD2);
        public static readonly Color LightGray = new(value: 0xD3D3D3);
        public static readonly Color LightGrey = new(value: 0xD3D3D3);
        public static readonly Color LightGreen = new(value: 0x90EE90);
        public static readonly Color LightPink = new(value: 0xFFB6C1);
        public static readonly Color LightSalmon = new(value: 0xFFA07A);
        public static readonly Color LightSeaGreen = new(value: 0x20B2AA);
        public static readonly Color LightSkyBlue = new(value: 0x87CEFA);
        public static readonly Color LightSlateGray = new(value: 0x778899);
        public static readonly Color LightSlateGrey = new(value: 0x778899);
        public static readonly Color LightSteelBlue = new(value: 0xB0C4DE);
        public static readonly Color LightYellow = new(value: 0xFFFFE0);
        public static readonly Color Lime = new(value: 0x00FF00);
        public static readonly Color LimeGreen = new(value: 0x32CD32);
        public static readonly Color Linen = new(value: 0xFAF0E6);
        public static readonly Color Magenta = new(value: 0xFF00FF);
        public static readonly Color Maroon = new(value: 0x800000);
        public static readonly Color MediumAquaMarine = new(value: 0x66CDAA);
        public static readonly Color MediumBlue = new(value: 0x0000CD);
        public static readonly Color MediumOrchid = new(value: 0xBA55D3);
        public static readonly Color MediumPurple = new(value: 0x9370DB);
        public static readonly Color MediumSeaGreen = new(value: 0x3CB371);
        public static readonly Color MediumSlateBlue = new(value: 0x7B68EE);
        public static readonly Color MediumSpringGreen = new(value: 0x00FA9A);
        public static readonly Color MediumTurquoise = new(value: 0x48D1CC);
        public static readonly Color MediumVioletRed = new(value: 0xC71585);
        public static readonly Color MidnightBlue = new(value: 0x191970);
        public static readonly Color MintCream = new(value: 0xF5FFFA);
        public static readonly Color MistyRose = new(value: 0xFFE4E1);
        public static readonly Color Moccasin = new(value: 0xFFE4B5);
        public static readonly Color NavajoWhite = new(value: 0xFFDEAD);
        public static readonly Color Navy = new(value: 0x000080);
        public static readonly Color OldLace = new(value: 0xFDF5E6);
        public static readonly Color Olive = new(value: 0x808000);
        public static readonly Color OliveDrab = new(value: 0x6B8E23);
        public static readonly Color Orange = new(value: 0xFFA500);
        public static readonly Color OrangeRed = new(value: 0xFF4500);
        public static readonly Color Orchid = new(value: 0xDA70D6);
        public static readonly Color PaleGoldenRod = new(value: 0xEEE8AA);
        public static readonly Color PaleGreen = new(value: 0x98FB98);
        public static readonly Color PaleTurquoise = new(value: 0xAFEEEE);
        public static readonly Color PaleVioletRed = new(value: 0xDB7093);
        public static readonly Color PapayaWhip = new(value: 0xFFEFD5);
        public static readonly Color PeachPuff = new(value: 0xFFDAB9);
        public static readonly Color Peru = new(value: 0xCD853F);
        public static readonly Color Pink = new(value: 0xFFC0CB);
        public static readonly Color Plum = new(value: 0xDDA0DD);
        public static readonly Color PowderBlue = new(value: 0xB0E0E6);
        public static readonly Color Purple = new(value: 0x800080);
        public static readonly Color RebeccaPurple = new(value: 0x663399);
        public static readonly Color Red = new(value: 0xFF0000);
        public static readonly Color RosyBrown = new(value: 0xBC8F8F);
        public static readonly Color RoyalBlue = new(value: 0x4169E1);
        public static readonly Color SaddleBrown = new(value: 0x8B4513);
        public static readonly Color Salmon = new(value: 0xFA8072);
        public static readonly Color SandyBrown = new(value: 0xF4A460);
        public static readonly Color SeaGreen = new(value: 0x2E8B57);
        public static readonly Color SeaShell = new(value: 0xFFF5EE);
        public static readonly Color Sienna = new(value: 0xA0522D);
        public static readonly Color Silver = new(value: 0xC0C0C0);
        public static readonly Color SkyBlue = new(value: 0x87CEEB);
        public static readonly Color SlateBlue = new(value: 0x6A5ACD);
        public static readonly Color SlateGray = new(value: 0x708090);
        public static readonly Color SlateGrey = new(value: 0x708090);
        public static readonly Color Snow = new(value: 0xFFFAFA);
        public static readonly Color SpringGreen = new(value: 0x00FF7F);
        public static readonly Color SteelBlue = new(value: 0x4682B4);
        public static readonly Color Tan = new(value: 0xD2B48C);
        public static readonly Color Teal = new(value: 0x008080);
        public static readonly Color Thistle = new(value: 0xD8BFD8);
        public static readonly Color Tomato = new(value: 0xFF6347);
        public static readonly Color Turquoise = new(value: 0x40E0D0);
        public static readonly Color Violet = new(value: 0xEE82EE);
        public static readonly Color Wheat = new(value: 0xF5DEB3);
        public static readonly Color White = new(value: 0xFFFFFF);
        public static readonly Color WhiteSmoke = new(value: 0xF5F5F5);
        public static readonly Color Yellow = new(value: 0xFFFF00);
        public static readonly Color YellowGreen = new(value: 0x9ACD32);
        public static readonly Color GwenPink = new(r: 255, g: 65, b: 199);
        public static readonly Color Transparent = new(a: 0, r: 255, g: 255, b: 255);

        private static readonly Dictionary<string, Color> namedColors = new()
        {
            {"AliceBlue", new Color(value: 0xF0F8FF)},
            {"AntiqueWhite", new Color(value: 0xFAEBD7)},
            {"Aqua", new Color(value: 0x00FFFF)},
            {"Aquamarine", new Color(value: 0x7FFFD4)},
            {"Azure", new Color(value: 0xF0FFFF)},
            {"Beige", new Color(value: 0xF5F5DC)},
            {"Bisque", new Color(value: 0xFFE4C4)},
            {"Black", new Color(value: 0x000000)},
            {"BlanchedAlmond", new Color(value: 0xFFEBCD)},
            {"Blue", new Color(value: 0x0000FF)},
            {"BlueViolet", new Color(value: 0x8A2BE2)},
            {"Brown", new Color(value: 0xA52A2A)},
            {"BurlyWood", new Color(value: 0xDEB887)},
            {"CadetBlue", new Color(value: 0x5F9EA0)},
            {"Chartreuse", new Color(value: 0x7FFF00)},
            {"Chocolate", new Color(value: 0xD2691E)},
            {"Coral", new Color(value: 0xFF7F50)},
            {"CornflowerBlue", new Color(value: 0x6495ED)},
            {"Cornsilk", new Color(value: 0xFFF8DC)},
            {"Crimson", new Color(value: 0xDC143C)},
            {"Cyan", new Color(value: 0x00FFFF)},
            {"DarkBlue", new Color(value: 0x00008B)},
            {"DarkCyan", new Color(value: 0x008B8B)},
            {"DarkGoldenRod", new Color(value: 0xB8860B)},
            {"DarkGray", new Color(value: 0xA9A9A9)},
            {"DarkGrey", new Color(value: 0xA9A9A9)},
            {"DarkGreen", new Color(value: 0x006400)},
            {"DarkKhaki", new Color(value: 0xBDB76B)},
            {"DarkMagenta", new Color(value: 0x8B008B)},
            {"DarkOliveGreen", new Color(value: 0x556B2F)},
            {"DarkOrange", new Color(value: 0xFF8C00)},
            {"DarkOrchid", new Color(value: 0x9932CC)},
            {"DarkRed", new Color(value: 0x8B0000)},
            {"DarkSalmon", new Color(value: 0xE9967A)},
            {"DarkSeaGreen", new Color(value: 0x8FBC8F)},
            {"DarkSlateBlue", new Color(value: 0x483D8B)},
            {"DarkSlateGray", new Color(value: 0x2F4F4F)},
            {"DarkSlateGrey", new Color(value: 0x2F4F4F)},
            {"DarkTurquoise", new Color(value: 0x00CED1)},
            {"DarkViolet", new Color(value: 0x9400D3)},
            {"DeepPink", new Color(value: 0xFF1493)},
            {"DeepSkyBlue", new Color(value: 0x00BFFF)},
            {"DimGray", new Color(value: 0x696969)},
            {"DimGrey", new Color(value: 0x696969)},
            {"DodgerBlue", new Color(value: 0x1E90FF)},
            {"FireBrick", new Color(value: 0xB22222)},
            {"FloralWhite", new Color(value: 0xFFFAF0)},
            {"ForestGreen", new Color(value: 0x228B22)},
            {"Fuchsia", new Color(value: 0xFF00FF)},
            {"Gainsboro", new Color(value: 0xDCDCDC)},
            {"GhostWhite", new Color(value: 0xF8F8FF)},
            {"Gold", new Color(value: 0xFFD700)},
            {"GoldenRod", new Color(value: 0xDAA520)},
            {"Gray", new Color(value: 0x808080)},
            {"Grey", new Color(value: 0x808080)},
            {"Green", new Color(value: 0x008000)},
            {"GreenYellow", new Color(value: 0xADFF2F)},
            {"HoneyDew", new Color(value: 0xF0FFF0)},
            {"HotPink", new Color(value: 0xFF69B4)},
            {"IndianRed", new Color(value: 0xCD5C5C)},
            {"Indigo", new Color(value: 0x4B0082)},
            {"Ivory", new Color(value: 0xFFFFF0)},
            {"Khaki", new Color(value: 0xF0E68C)},
            {"Lavender", new Color(value: 0xE6E6FA)},
            {"LavenderBlush", new Color(value: 0xFFF0F5)},
            {"LawnGreen", new Color(value: 0x7CFC00)},
            {"LemonChiffon", new Color(value: 0xFFFACD)},
            {"LightBlue", new Color(value: 0xADD8E6)},
            {"LightCoral", new Color(value: 0xF08080)},
            {"LightCyan", new Color(value: 0xE0FFFF)},
            {"LightGoldenRodYellow", new Color(value: 0xFAFAD2)},
            {"LightGray", new Color(value: 0xD3D3D3)},
            {"LightGrey", new Color(value: 0xD3D3D3)},
            {"LightGreen", new Color(value: 0x90EE90)},
            {"LightPink", new Color(value: 0xFFB6C1)},
            {"LightSalmon", new Color(value: 0xFFA07A)},
            {"LightSeaGreen", new Color(value: 0x20B2AA)},
            {"LightSkyBlue", new Color(value: 0x87CEFA)},
            {"LightSlateGray", new Color(value: 0x778899)},
            {"LightSlateGrey", new Color(value: 0x778899)},
            {"LightSteelBlue", new Color(value: 0xB0C4DE)},
            {"LightYellow", new Color(value: 0xFFFFE0)},
            {"Lime", new Color(value: 0x00FF00)},
            {"LimeGreen", new Color(value: 0x32CD32)},
            {"Linen", new Color(value: 0xFAF0E6)},
            {"Magenta", new Color(value: 0xFF00FF)},
            {"Maroon", new Color(value: 0x800000)},
            {"MediumAquaMarine", new Color(value: 0x66CDAA)},
            {"MediumBlue", new Color(value: 0x0000CD)},
            {"MediumOrchid", new Color(value: 0xBA55D3)},
            {"MediumPurple", new Color(value: 0x9370DB)},
            {"MediumSeaGreen", new Color(value: 0x3CB371)},
            {"MediumSlateBlue", new Color(value: 0x7B68EE)},
            {"MediumSpringGreen", new Color(value: 0x00FA9A)},
            {"MediumTurquoise", new Color(value: 0x48D1CC)},
            {"MediumVioletRed", new Color(value: 0xC71585)},
            {"MidnightBlue", new Color(value: 0x191970)},
            {"MintCream", new Color(value: 0xF5FFFA)},
            {"MistyRose", new Color(value: 0xFFE4E1)},
            {"Moccasin", new Color(value: 0xFFE4B5)},
            {"NavajoWhite", new Color(value: 0xFFDEAD)},
            {"Navy", new Color(value: 0x000080)},
            {"OldLace", new Color(value: 0xFDF5E6)},
            {"Olive", new Color(value: 0x808000)},
            {"OliveDrab", new Color(value: 0x6B8E23)},
            {"Orange", new Color(value: 0xFFA500)},
            {"OrangeRed", new Color(value: 0xFF4500)},
            {"Orchid", new Color(value: 0xDA70D6)},
            {"PaleGoldenRod", new Color(value: 0xEEE8AA)},
            {"PaleGreen", new Color(value: 0x98FB98)},
            {"PaleTurquoise", new Color(value: 0xAFEEEE)},
            {"PaleVioletRed", new Color(value: 0xDB7093)},
            {"PapayaWhip", new Color(value: 0xFFEFD5)},
            {"PeachPuff", new Color(value: 0xFFDAB9)},
            {"Peru", new Color(value: 0xCD853F)},
            {"Pink", new Color(value: 0xFFC0CB)},
            {"Plum", new Color(value: 0xDDA0DD)},
            {"PowderBlue", new Color(value: 0xB0E0E6)},
            {"Purple", new Color(value: 0x800080)},
            {"RebeccaPurple", new Color(value: 0x663399)},
            {"Red", new Color(value: 0xFF0000)},
            {"RosyBrown", new Color(value: 0xBC8F8F)},
            {"RoyalBlue", new Color(value: 0x4169E1)},
            {"SaddleBrown", new Color(value: 0x8B4513)},
            {"Salmon", new Color(value: 0xFA8072)},
            {"SandyBrown", new Color(value: 0xF4A460)},
            {"SeaGreen", new Color(value: 0x2E8B57)},
            {"SeaShell", new Color(value: 0xFFF5EE)},
            {"Sienna", new Color(value: 0xA0522D)},
            {"Silver", new Color(value: 0xC0C0C0)},
            {"SkyBlue", new Color(value: 0x87CEEB)},
            {"SlateBlue", new Color(value: 0x6A5ACD)},
            {"SlateGray", new Color(value: 0x708090)},
            {"SlateGrey", new Color(value: 0x708090)},
            {"Snow", new Color(value: 0xFFFAFA)},
            {"SpringGreen", new Color(value: 0x00FF7F)},
            {"SteelBlue", new Color(value: 0x4682B4)},
            {"Tan", new Color(value: 0xD2B48C)},
            {"Teal", new Color(value: 0x008080)},
            {"Thistle", new Color(value: 0xD8BFD8)},
            {"Tomato", new Color(value: 0xFF6347)},
            {"Turquoise", new Color(value: 0x40E0D0)},
            {"Violet", new Color(value: 0xEE82EE)},
            {"Wheat", new Color(value: 0xF5DEB3)},
            {"White", new Color(value: 0xFFFFFF)},
            {"WhiteSmoke", new Color(value: 0xF5F5F5)},
            {"Yellow", new Color(value: 0xFFFF00)},
            {"YellowGreen", new Color(value: 0x9ACD32)},
            {"GwenPink", new Color(r: 255, g: 65, b: 199)},
            {"Transparent", new Color(a: 0, r: 255, g: 255, b: 255)}
        };
    }
}
