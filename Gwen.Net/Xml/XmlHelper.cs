using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Gwen.Net.CommonDialog;
using Gwen.Net.Control;
using Gwen.Net.Control.Layout;

namespace Gwen.Net.Xml
{
    public class XmlHelper
    {
        private static readonly Dictionary<Type, EventHandlerConverter> m_EventHandlerConverters = new()
        {
            {
                typeof(EventArgs), (attribute, value) =>
                {
                    return new ControlBase.GwenEventHandler<EventArgs>(
                        new XmlEventHandler<EventArgs>(value, attribute).OnEvent);
                }
            },
            {
                typeof(ClickedEventArgs), (attribute, value) =>
                {
                    return new ControlBase.GwenEventHandler<ClickedEventArgs>(
                        new XmlEventHandler<ClickedEventArgs>(value, attribute).OnEvent);
                }
            },
            {
                typeof(ItemSelectedEventArgs), (attribute, value) =>
                {
                    return new ControlBase.GwenEventHandler<ItemSelectedEventArgs>(
                        new XmlEventHandler<ItemSelectedEventArgs>(value, attribute).OnEvent);
                }
            },
            {
                typeof(LinkClickedEventArgs), (attribute, value) =>
                {
                    return new ControlBase.GwenEventHandler<LinkClickedEventArgs>(
                        new XmlEventHandler<LinkClickedEventArgs>(value, attribute).OnEvent);
                }
            }
        };

        private static readonly Dictionary<Type, AttributeValueConverter> m_AttributeValueConverters = new()
        {
            {
                typeof(bool), (element, value) =>
                {
                    bool result;

                    if (bool.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as boolean failed.");
                }
            },
            {
                typeof(int), (element, value) =>
                {
                    int result;

                    if (Int32.TryParse(value, NumberStyles.Integer, Parser.NumberFormatInfo, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as integer failed.");
                }
            },
            {
                typeof(float), (element, value) =>
                {
                    float result;

                    if (Single.TryParse(value, NumberStyles.Float, Parser.NumberFormatInfo, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as single failed.");
                }
            },
            {
                typeof(string), (element, value) =>
                {
                    return value;
                }
            },
            {
                typeof(char), (element, value) =>
                {
                    if (value.Length == 1)
                    {
                        return value[index: 0];
                    }

                    throw new XmlException("Attribute value error. Parsing the value as char failed.");
                }
            },
            {
                typeof(object), (element, value) =>
                {
                    if (value.IndexOf(Parser.NumberFormatInfo.NumberDecimalSeparator) >= 0)
                    {
                        float valueFloat;

                        if (Single.TryParse(value, NumberStyles.Float, Parser.NumberFormatInfo, out valueFloat))
                        {
                            return valueFloat;
                        }
                    }

                    int valueInt32;

                    if (Int32.TryParse(value, NumberStyles.Integer, Parser.NumberFormatInfo, out valueInt32))
                    {
                        return valueInt32;
                    }

                    return value;
                }
            },
            {
                typeof(Size), (element, value) =>
                {
                    int[] values = ParseArrayInt32(value);

                    if (values != null)
                    {
                        if (values.Length == 2)
                        {
                            return new Size(values[0], values[1]);
                        }
                    }

                    throw new XmlException("Attribute value error. Parsing the value as size failed.");
                }
            },
            {
                typeof(Point), (element, value) =>
                {
                    int[] values = ParseArrayInt32(value);

                    if (values != null)
                    {
                        if (values.Length == 2)
                        {
                            return new Point(values[0], values[1]);
                        }
                    }

                    throw new XmlException("Attribute value error. Parsing the value as point failed.");
                }
            },
            {
                typeof(Margin), (element, value) =>
                {
                    int[] values = ParseArrayInt32(value);

                    if (values != null)
                    {
                        if (values.Length == 1)
                        {
                            return new Margin(values[0], values[0], values[0], values[0]);
                        }

                        if (values.Length == 2)
                        {
                            return new Margin(values[0], values[1], values[0], values[1]);
                        }

                        if (values.Length == 4)
                        {
                            return new Margin(values[0], values[1], values[2], values[3]);
                        }
                    }

                    throw new XmlException("Attribute value error. Parsing the value as margin failed.");
                }
            },
            {
                typeof(Padding), (element, value) =>
                {
                    int[] values = ParseArrayInt32(value);

                    if (values != null)
                    {
                        if (values.Length == 1)
                        {
                            return new Padding(values[0], values[0], values[0], values[0]);
                        }

                        if (values.Length == 2)
                        {
                            return new Padding(values[0], values[1], values[0], values[1]);
                        }

                        if (values.Length == 4)
                        {
                            return new Padding(values[0], values[1], values[2], values[3]);
                        }
                    }

                    throw new XmlException("Attribute value error. Parsing the value as padding failed.");
                }
            },
            {
                typeof(Anchor), (element, value) =>
                {
                    int[] values = ParseArrayInt32(value);

                    if (values != null)
                    {
                        if (values.Length == 2)
                        {
                            return new Anchor((byte)values[0], (byte)values[1], (byte)values[0], (byte)values[1]);
                        }

                        if (values.Length == 4)
                        {
                            return new Anchor((byte)values[0], (byte)values[1], (byte)values[2], (byte)values[3]);
                        }
                    }

                    throw new XmlException("Attribute value error. Parsing the value as anchor failed.");
                }
            },
            {
                typeof(Rectangle), (element, value) =>
                {
                    int[] values = ParseArrayInt32(value);

                    if (values != null)
                    {
                        if (values.Length == 4)
                        {
                            return new Rectangle(values[0], values[1], values[2], values[3]);
                        }
                    }

                    throw new XmlException("Attribute value error. Parsing the value as rectangle failed.");
                }
            },
            {
                typeof(Color), (element, value) =>
                {
                    int[] values = ParseArrayInt32(value);

                    if (values != null)
                    {
                        if (values.Length == 4)
                        {
                            return new Color(values[0], values[1], values[2], values[3]);
                        }

                        if (values.Length == 3)
                        {
                            return new Color(values[0], values[1], values[2]);
                        }

                        throw new XmlException("Attribute value error. Parsing the value as color failed.");
                    }

                    value = value.Trim();

                    string hex = null;

                    if (value.StartsWith("0x") || value.StartsWith("0X"))
                    {
                        hex = value.Substring(startIndex: 2);
                    }
                    else if (value.StartsWith("#"))
                    {
                        hex = value.Substring(startIndex: 1);
                    }

                    if (hex != null)
                    {
                        uint color;

                        if (UInt32.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out color))
                        {
                            return new Color(color);
                        }

                        throw new XmlException("Attribute value error. Parsing the value as color failed.");
                    }

                    return Color.FromName(value);
                }
            },
            {
                typeof(Alignment), (element, value) =>
                {
                    Alignment result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as alignment failed.");
                }
            },
            {
                typeof(HorizontalAlignment), (element, value) =>
                {
                    HorizontalAlignment result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as horizontal alignment failed.");
                }
            },
            {
                typeof(VerticalAlignment), (element, value) =>
                {
                    VerticalAlignment result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as vertical alignment failed.");
                }
            },
            {
                typeof(Dock), (element, value) =>
                {
                    Dock result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as dock failed.");
                }
            },
            {
                typeof(ImageAlign), (element, value) =>
                {
                    ImageAlign result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as image alignment failed.");
                }
            },
            {
                typeof(BorderType), (element, value) =>
                {
                    BorderType result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as border type failed.");
                }
            },
            {
                typeof(StartPosition), (element, value) =>
                {
                    StartPosition result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as start position failed.");
                }
            },
            {
                typeof(Resizing), (element, value) =>
                {
                    Resizing result;

                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }

                    throw new XmlException("Attribute value error. Parsing the value as resizing failed.");
                }
            },
            {
                typeof(Font), (element, value) =>
                {
                    string name;
                    int size = 10;
                    FontStyle style = FontStyle.Normal;

                    string[] fontValues = value.Split(m_fontValueSeparator);

                    if (fontValues.Length == 0)
                    {
                        throw new XmlException("Attribute value error. No font face name specified.");
                    }

                    name = fontValues[0].Trim();

                    if (fontValues.Length >= 2)
                    {
                        if (!Int32.TryParse(fontValues[1], out size))
                        {
                            throw new XmlException("Attribute value error. Font size parsing failed.");
                        }

                        for (int i = 2; i < fontValues.Length; i++)
                        {
                            switch (fontValues[i].Trim().ToLower())
                            {
                                case "bold":
                                    style |= FontStyle.Bold;

                                    break;
                                case "italic":
                                    style |= FontStyle.Italic;

                                    break;
                                case "underline":
                                    style |= FontStyle.Underline;

                                    break;
                                case "strikeout":
                                    style |= FontStyle.Strikeout;

                                    break;
                                default:
                                    throw new XmlException("Attribute value error. Unknown font style.");
                            }
                        }
                    }

                    if (element is ControlBase)
                    {
                        return Font.Create(((ControlBase)element).Skin.Renderer, name, size, style);
                    }

                    if (element is Component)
                    {
                        return Font.Create(((Component)element).View.Skin.Renderer, name, size, style);
                    }

                    throw new Exception("Can't create a font. The renderer is unknown.");
                }
            },
            {
                typeof(GridCellSizes), (element, value) =>
                {
                    string[] values = value.Split(Parser.ArraySeparator);
                    float[] sizes = new float[values.Length];
                    int index = 0;

                    foreach (string val in values)
                    {
                        string cellSize = val.Trim();

                        if (cellSize.ToLower() == "auto")
                        {
                            sizes[index++] = GridLayout.AutoSize;
                        }
                        else if (cellSize.IndexOf(value: '%') > 0)
                        {
                            float v;

                            if (Single.TryParse(cellSize.Substring(startIndex: 0, cellSize.IndexOf(value: '%')), out v))
                            {
                                sizes[index++] = v / 100.0f;
                            }
                            else
                            {
                                throw new XmlException("Attribute value error. Parsing the value as cell size failed.");
                            }
                        }
                        else
                        {
                            float v;

                            if (Single.TryParse(cellSize, out v))
                            {
                                sizes[index++] = v;
                            }
                            else
                            {
                                throw new XmlException("Attribute value error. Parsing the value as cell size failed.");
                            }
                        }
                    }

                    return new GridCellSizes(sizes);
                }
            }
        };

        private static readonly char[] m_fontValueSeparator =
        {
            ';'
        };

        internal static void RegisterDefaultHandlers()
        {
            foreach (KeyValuePair<Type, AttributeValueConverter> converter in m_AttributeValueConverters)
            {
                Parser.RegisterAttributeValueConverter(converter.Key, converter.Value);
            }

            foreach (KeyValuePair<Type, EventHandlerConverter> converter in m_EventHandlerConverters)
            {
                Parser.RegisterEventHandlerConverter(converter.Key, converter.Value);
            }

            Component.Register<FileDialog>();
            Component.Register<OpenFileDialog>();
            Component.Register<SaveFileDialog>();
            Component.Register<FolderBrowserDialog>();
        }

        public static Int32[] ParseArrayInt32(string valueStr)
        {
            string[] values = valueStr.Split(Parser.ArraySeparator);
            Int32[] newValues = new Int32[values.Length];
            int index = 0;

            foreach (string value in values)
            {
                if (!Int32.TryParse(value, NumberStyles.Integer, Parser.NumberFormatInfo, out newValues[index++]))
                {
                    return null;
                }
            }

            return newValues;
        }
    }
}