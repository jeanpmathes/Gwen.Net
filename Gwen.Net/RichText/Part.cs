using System;

namespace Gwen.Net.RichText
{
    public abstract class Part
    {
        public abstract String[] Split(ref Font splitFont);
    }
}
