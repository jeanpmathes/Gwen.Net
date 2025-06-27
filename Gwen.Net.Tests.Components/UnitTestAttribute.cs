using System;

namespace Gwen.Net.Tests.Components
{
    public class UnitTestAttribute : Attribute
    {
        public UnitTestAttribute()
        {
            Order = Int32.MaxValue;
        }

        public String Category { get; set; }
        public Int32 Order { get; set; }
        public String Name { get; set; }
    }
}
