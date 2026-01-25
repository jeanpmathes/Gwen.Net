using System;

namespace Gwen.Net.Legacy
{
    /// <summary>
    ///     Font metrics.
    /// </summary>
    public struct FontMetrics
    {
        public Single EmHeightPixels { get; internal set; }
        public Single AscentPixels { get; internal set; }
        public Single DescentPixels { get; internal set; }
        public Single CellHeightPixels { get; internal set; }
        public Single InternalLeadingPixels { get; internal set; }
        public Single LineSpacingPixels { get; internal set; }
        public Single ExternalLeadingPixels { get; internal set; }

        public Single Top => InternalLeadingPixels;
        public Single Baseline => AscentPixels;
        public Single Bottom => CellHeightPixels;

        public FontMetrics(Font font)
        {
            EmHeightPixels = font.RealSize;
            InternalLeadingPixels = 0.0f;
            ExternalLeadingPixels = 0.0f;
            DescentPixels = 0.0f;
            AscentPixels = EmHeightPixels;
            CellHeightPixels = EmHeightPixels;
            LineSpacingPixels = EmHeightPixels;
        }

        public FontMetrics
        (
            Single emHeightPixels,
            Single ascentPixels,
            Single descentPixels,
            Single cellHeightPixels,
            Single internalLeadingPixels,
            Single lineSpacingPixels,
            Single externalLeadingPixels
        )
        {
            EmHeightPixels = emHeightPixels;
            AscentPixels = ascentPixels;
            DescentPixels = descentPixels;
            CellHeightPixels = cellHeightPixels;
            InternalLeadingPixels = internalLeadingPixels;
            LineSpacingPixels = lineSpacingPixels;
            ExternalLeadingPixels = externalLeadingPixels;
        }
    }
}