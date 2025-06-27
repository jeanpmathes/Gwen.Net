using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Gwen.Net.Platform
{
    public class GwenPlatform
    {
        private static IPlatform platform;

        public static String CurrentDirectory
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
        /// <param name="initPlatform">Platform.</param>
        public static void Init(IPlatform initPlatform)
        {
            platform = initPlatform;
        }

        /// <summary>
        ///     Gets text from clipboard.
        /// </summary>
        /// <returns>Clipboard text.</returns>
        public static String GetClipboardText()
        {
            Debug.Assert(platform != null);

            return platform.GetClipboardText();
        }

        /// <summary>
        ///     Sets the clipboard text.
        /// </summary>
        /// <param name="text">Text to set.</param>
        /// <returns>True if succeeded.</returns>
        public static Boolean SetClipboardText(String text)
        {
            Debug.Assert(platform != null);

            return platform.SetClipboardText(text);
        }


        /// <summary>
        ///     Gets elapsed time since this class was initalized.
        /// </summary>
        /// <returns>Time interval in seconds.</returns>
        public static Single GetTimeInSeconds()
        {
            Debug.Assert(platform != null);

            return (Single)platform.GetTimeInSeconds();
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

        public static String GetFileName(String path)
        {
            Debug.Assert(platform != null);

            return platform.GetFileName(path);
        }

        public static String GetDirectoryName(String path)
        {
            Debug.Assert(platform != null);

            return platform.GetDirectoryName(path);
        }

        public static Boolean FileExists(String path)
        {
            Debug.Assert(platform != null);

            return platform.FileExists(path);
        }

        public static Boolean DirectoryExists(String path)
        {
            Debug.Assert(platform != null);

            return platform.DirectoryExists(path);
        }

        public static void CreateDirectory(String path)
        {
            Debug.Assert(platform != null);

            platform.CreateDirectory(path);
        }

        public static String Combine(String path1, String path2)
        {
            Debug.Assert(platform != null);

            return platform.Combine(path1, path2);
        }

        public static String Combine(String path1, String path2, String path3)
        {
            Debug.Assert(platform != null);

            return platform.Combine(path1, path2, path3);
        }

        public static String Combine(String path1, String path2, String path3, String path4)
        {
            Debug.Assert(platform != null);

            return platform.Combine(path1, path2, path3, path4);
        }

        public static IEnumerable<IFileSystemDirectoryInfo> GetDirectories(String path)
        {
            Debug.Assert(platform != null);

            return platform.GetDirectories(path);
        }

        public static IEnumerable<IFileSystemFileInfo> GetFiles(String path, String filter)
        {
            Debug.Assert(platform != null);

            return platform.GetFiles(path, filter);
        }

        public static Stream GetFileStream(String path, Boolean isWritable)
        {
            Debug.Assert(platform != null);

            return platform.GetFileStream(path, isWritable);
        }
    }
}
