namespace DynamicMappingSystem.Core
{
    /// <summary>
    /// Main interface for conversion between different data models
    /// </summary>
    public interface IMapHandler
    {
        /// <summary>
        /// Converts a data object from source type to target type
        /// </summary>
        object Map(object data, string sourceType, string targetType);
    }
}