using System;

namespace Gwen.Net.Legacy.Platform
{
    public interface ISpecialFolder
    {
        String Name { get; }
        String Category { get; }
        String Path { get; }
    }
}