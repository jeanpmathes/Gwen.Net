using System;

namespace Gwen.Net.New.Texts;

/// <summary>
/// Combines text layout options that are provided at formatted-text creation time.
/// </summary>
public record struct TextOptions(TextWrapping Wrapping, TextAlignment Alignment, TextTrimming Trimming, Single LineHeight);
