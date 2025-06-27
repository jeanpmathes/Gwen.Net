using System;

namespace Gwen.Net.Platform
{
    public interface IFileSystemItemInfo
    {
        String Name { get; }
        String FullName { get; }
        String FormattedLastWriteTime { get; }
    }
}