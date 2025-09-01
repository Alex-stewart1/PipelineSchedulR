using PipelineSchedulR.Interfaces;

namespace PipelineSchedulR.Common.Helpers;

internal static class KeyedServiceHelper
{
    /// <summary>
    /// Guid to be used as a prefix for the key
    /// </summary>
    private static readonly string _guid = Guid.NewGuid().ToString();

    /// <summary>
    /// Get a unique key for a type that implements <see cref="IExecutable"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>
    /// A unique key per executable type
    /// </returns>
    /// <exception cref="InvalidOperationException"/>
    internal static string GetExecutableKey(Type type)
    {
        if (!typeof(IExecutable).IsAssignableFrom(type))
        {
            throw new InvalidOperationException($"Type {type.Name} does not implement {typeof(IExecutable).FullName}");
        }

        return $"{_guid}:{type.FullName}";
    }
}
