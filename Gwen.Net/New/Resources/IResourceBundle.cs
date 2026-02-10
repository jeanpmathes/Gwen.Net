namespace Gwen.Net.New.Resources;

/// <summary>
/// A bundle of resources that can be added to a <see cref="Context"/>.
/// </summary>
/// <typeparam name="TSelf">The type of the resource bundle itself.</typeparam>
public interface IResourceBundle<out TSelf> where TSelf : IResourceBundle<TSelf>
{
    /// <summary>
    /// Loads the resource bundle.
    /// </summary>
    /// <param name="registry">The registry to load the resources into.</param>
    /// <returns>The loaded resource bundle.</returns>
    public static abstract TSelf Load(ResourceRegistry registry);
}
