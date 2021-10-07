using System;

namespace Gwen.Net.OpenTk.Exceptions
{
    public class ResourceLoaderNotFoundException : Exception
    {
        public ResourceLoaderNotFoundException(string resourceName)
            : base(string.Format(StringResources.ResourceLoaderNotFoundFormat, resourceName))
        {
            ResourceName = resourceName;
        }

        public string ResourceName { get; }
    }
}