using System;

namespace Gwen.Net.Tests.Components
{
    public class UnitTestAttribute : Attribute
    {
        public UnitTestAttribute()
        {
            Order = int.MaxValue;
        }

        public string Category { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }
}
