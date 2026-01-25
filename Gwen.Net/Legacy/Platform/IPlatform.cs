using System;
using System.Collections.Generic;
using System.IO;

namespace Gwen.Net.Legacy.Platform
{
    public interface IPlatform
    {
        String CurrentDirectory { get; }

        /// <summary>
        ///     Gets text from clipboard.
        /// </summary>
        /// <returns>Clipboard text.</returns>
        String GetClipboardText();

        /// <summary>
        ///     Sets the clipboard text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        /// <returns>True if succeeded.</returns>
        Boolean SetClipboardText(String text);

        /// <summary>
        ///     Gets elapsed time. Initialization time is platform specific.
        /// </summary>
        /// <returns>Time interval in seconds.</returns>
        Double GetTimeInSeconds();

        /// <summary>
        ///     Changes the mouse cursor.
        /// </summary>
        /// <param name="cursor">Cursor type.</param>
        void SetCursor(Cursor cursor);

        /// <summary>
        ///     Get special folders of the system.
        /// </summary>
        /// <returns>List of folders.</returns>
        IEnumerable<ISpecialFolder> GetSpecialFolders();

        String GetFileName(String path);
        String GetDirectoryName(String path);

        Boolean FileExists(String path);
        Boolean DirectoryExists(String path);

        void CreateDirectory(String path);

        String Combine(String path1, String path2);
        String Combine(String path1, String path2, String path3);
        String Combine(String path1, String path2, String path3, String path4);

        IEnumerable<IFileSystemDirectoryInfo> GetDirectories(String path);
        IEnumerable<IFileSystemFileInfo> GetFiles(String path, String filter);

        Stream GetFileStream(String path, Boolean isWritable);
    }
}