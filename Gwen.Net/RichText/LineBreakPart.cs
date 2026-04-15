using System;

namespace Gwen.Net.RichText
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
