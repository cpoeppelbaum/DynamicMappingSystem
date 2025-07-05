namespace DynamicMappingSystem.Core
{
    /// <summary>
    /// Specific mapper for transformation between two particular types
    /// </summary>
    public interface IMapper<TSource, TTarget>
    {
        /// <summary>
        /// Maps a source object into a target object
        /// </summary>
        TTarget Map(TSource source);
    }
}