using System;

namespace Gwen.Net.Platform
{
    public interface IFileSystemFileInfo : IFileSystemItemInfo
    {
        String FormattedFileLength { get; }
    }
}