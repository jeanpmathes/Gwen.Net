using System;

namespace Gwen.Net.Legacy.Platform
{
    public interface IFileSystemItemInfo
    {
        String Name { get; }
        String FullName { get; }
        String FormattedLastWriteTime { get; }
    }
}