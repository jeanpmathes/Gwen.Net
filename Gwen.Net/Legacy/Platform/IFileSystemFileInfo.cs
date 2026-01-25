using System;

namespace Gwen.Net.Legacy.Platform
{
    public interface IFileSystemFileInfo : IFileSystemItemInfo
    {
        String FormattedFileLength { get; }
    }
}