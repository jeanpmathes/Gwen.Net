using System;

namespace Gwen.Net.New.Texts;

/// <summary>
/// Describes a font.
/// </summary>
public record struct Font(String Family, Single Size, Weight Weight, Style Style, Stretch Stretch);
