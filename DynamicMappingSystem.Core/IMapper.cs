namespace DynamicMappingSystem.Core
{
    public interface IMapper
    {
        /// <summary>
        /// Maps a source object into a target object
        /// </summary>
        /// <typeparam name="TSource">Type of the source object</typeparam>
        /// <typeparam name="TTarget">Type of the target object</typeparam>
        /// <param name="source">Source object to be mapped. It is guaranteed that source is not null.</param>
        /// <returns>Mapped target object</returns>
        object Map(object source);
    }
}