using System;

namespace Gwen.Net.Legacy.RichText
{
    public abstract class Part
    {
        public abstract String[] Split(ref Font splitFont);
    }
}
