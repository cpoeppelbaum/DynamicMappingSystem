namespace DynamicMappingSystem.Core
{
    /// <summary>
    /// Specific converter for transformation between two particular types
    /// </summary>
    public interface IMapper<TSource, TTarget>
    {
        /// <summary>
        /// Converts a source object into a target object
        /// </summary>
        TTarget Convert(TSource source);
    }
}