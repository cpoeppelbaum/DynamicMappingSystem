using FluentValidation;

namespace DynamicMappingSystem.Core
{
    /// <summary>
    /// Main interface for conversion between different data models
    /// </summary>
    public interface IMapHandler
    {
        /// <summary>
        /// Maps a data object from source type to target type
        /// </summary>
        object Map(object data, string sourceType, string targetType);

        /// <summary>
        /// Registers a mapper for direct registry lookup, improving performance
        /// </summary>
        IMapHandler RegisterMapper<TSource, TTarget>(AbstractMapper<TSource, TTarget> mapper);

        /// <summary>
        /// Registers a validator for direct registry lookup, improving validation performance
        /// </summary>
        IMapHandler RegisterValidator<T>(AbstractDMSValidator<T> validator);
    }
}