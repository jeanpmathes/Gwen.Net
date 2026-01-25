using System;

namespace Gwen.Net.OpenTk.Legacy.Exceptions
{
    public class ResourceLoaderNotFoundException : Exception
    {
        public ResourceLoaderNotFoundException(String resourceName)
            : base(String.Format(StringResources.ResourceLoaderNotFoundFormat, resourceName))
        {
            ResourceName = resourceName;
        }

        public String ResourceName { get; }
    }
}