namespace DynamicMappingSystem.Core
{
    /// <summary>
    /// Specific mapper for transformation between two particular types
    /// </summary>
    public abstract class IMapper<TSource, TTarget> : IMapper
    {
        /// <summary>
        /// Maps a source object into a target object
        /// </summary>
        public abstract TTarget Map(TSource source);

        public object Map(object source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source), "Source object cannot be null.");
            }

            if (!(source is TSource sourceTyped))
            {
                throw new ArgumentException($"Unsupported source type: {source.GetType().FullName}. Expected type: {typeof(TSource).FullName}");
            }

            var result = Map((TSource)source);
            if (result is null)
            {
                throw new InvalidOperationException("Mapping resulted in a null target object.");
            }

            return result;
        }
    }
}
