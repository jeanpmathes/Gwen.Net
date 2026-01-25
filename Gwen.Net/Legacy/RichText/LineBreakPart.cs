using System;

namespace Gwen.Net.Legacy.RichText
{
    public class LineBreakPart : Part
    {
        public override String[] Split(ref Font splitFont)
        {
            return new[]
            {
                "\n"
            };
        }
    }
}
