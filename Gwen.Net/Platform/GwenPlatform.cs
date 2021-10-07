using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Gwen.Net.Platform
{
    public class GwenPlatform
    {
        private static IPlatform platform;

        public static string CurrentDirectory
        {
            get
            {
                Debug.Assert(platform != null);

                return platform.CurrentDirectory;
            }
        }

        /// <summary>
        ///     Set the current platform.
        /// </summary>
        /// <param name="platform">Platform.</param>
        public static void Init(IPlatform platform)
        {
            GwenPlatform.platform = platform;
        }

        /// <summary>
        ///     Gets text from clipboard.
        /// </summary>
        /// <returns>Clipboard text.</returns>
        public static string GetClipboardText()
        {
            Debug.Assert(platform != null);

            return platform.GetClipboardText();
        }

        /// <summary>
        ///     Sets the clipboard text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        /// <returns>True if succeeded.</returns>
        public static bool SetClipboardText(string text)
        {
            Debug.Assert(platform != null);

            return platform.SetClipboardText(text);
        }


        /// <summary>
        ///     Gets elapsed time since this class was initalized.
        /// </summary>
        /// <returns>Time interval in seconds.</returns>
        public static float GetTimeInSeconds()
        {
            Debug.Assert(platform != null);

            return (float)platform.GetTimeInSeconds();
        }

        /// <summary>
        ///     Changes the mouse cursor.
        /// </summary>
        /// <param name="cursor">Cursor type.</param>
        public static void SetCursor(Cursor cursor)
        {
            Debug.Assert(platform != null);

            platform.SetCursor(cursor);
        }

        /// <summary>
        ///     Get special folders of the system.
        /// </summary>
        /// <returns>List of folders.</returns>
        public static IEnumerable<ISpecialFolder> GetSpecialFolders()
        {
            Debug.Assert(platform != null);

            return platform.GetSpecialFolders();
        }

        public static string GetFileName(string path)
        {
            Debug.Assert(platform != null);

            return platform.GetFileName(path);
        }

        public static string GetDirectoryName(string path)
        {
            Debug.Assert(platform != null);

            return platform.GetDirectoryName(path);
        }

        public static bool FileExists(string path)
        {
            Debug.Assert(platform != null);

            return platform.FileExists(path);
        }

        public static bool DirectoryExists(string path)
        {
            Debug.Assert(platform != null);

            return platform.DirectoryExists(path);
        }

        public static void CreateDirectory(string path)
        {
            Debug.Assert(platform != null);

            platform.CreateDirectory(path);
        }

        public static string Combine(string path1, string path2)
        {
            Debug.Assert(platform != null);

            return platform.Combine(path1, path2);
        }

        public static string Combine(string path1, string path2, string path3)
        {
            Debug.Assert(platform != null);

            return platform.Combine(path1, path2, path3);
        }

        public static string Combine(string path1, string path2, string path3, string path4)
        {
            Debug.Assert(platform != null);

            return platform.Combine(path1, path2, path3, path4);
        }

        public static IEnumerable<IFileSystemDirectoryInfo> GetDirectories(string path)
        {
            Debug.Assert(platform != null);

            return platform.GetDirectories(path);
        }

        public static IEnumerable<IFileSystemFileInfo> GetFiles(string path, string filter)
        {
            Debug.Assert(platform != null);

            return platform.GetFiles(path, filter);
        }

        public static Stream GetFileStream(string path, bool isWritable)
        {
            Debug.Assert(platform != null);

            return platform.GetFileStream(path, isWritable);
        }
    }
}