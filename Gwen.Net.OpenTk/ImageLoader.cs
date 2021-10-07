using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Gwen.Net.OpenTk.Exceptions;

namespace Gwen.Net.OpenTk
{
    public static class ImageLoader
    {
        public delegate Bitmap Loader(string filename);

        public static readonly Dictionary<string, Loader> loaders = new()
        {
            {"jpeg", StandardLoader},
            {"jpe", StandardLoader},
            {"jfif", StandardLoader},
            {"jpg", StandardLoader},
            {"bmp", StandardLoader},
            {"dib", StandardLoader},
            {"rle", StandardLoader},
            {"png", StandardLoader},
            {"gif", StandardLoader},
            {"tif", StandardLoader},
            {"exif", StandardLoader},
            {"wmf", StandardLoader},
            {"emf", StandardLoader}
        };

        public static Bitmap StandardLoader(string s)
        {
            return new(s);
        }

        public static Bitmap Load(string filename)
        {
            string resourceType = filename.ToLower().Split(separator: '.').Last();

            if (loaders.TryGetValue(resourceType, out Loader loader))
            {
                return loader.Invoke(filename);
            }

            throw new ResourceLoaderNotFoundException(resourceType);
        }
    }
}