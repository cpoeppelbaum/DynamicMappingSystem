using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using DynamicMappingSystem.Validators;
using System.Reflection;

namespace DynamicMappingSystem
{
    /// <summary>
    /// Main implementation of the data mapper
    /// </summary>
    public class MapHandler : IMapHandler
    {
        private readonly Dictionary<Tuple<string, string>, IMapper> _mapperRegistry = new Dictionary<Tuple<string, string>, IMapper>();
        private readonly Dictionary<string, Type> _typeRegistry = new Dictionary<string, Type>();
        private readonly Dictionary<string, IDMSValidator> _validatorRegistry = new Dictionary<string, IDMSValidator>();

        private MapHandler() { }

        public static IMapHandler Create()
        {
            var mapHandler = new MapHandler();
            mapHandler.RegisterValidator(new ReservationValidator());
            mapHandler.RegisterValidator(new RoomValidator());
            return mapHandler;
        }

        /// <summary>
        /// Registers a mapper instance
        /// </summary>
        public IMapHandler RegisterMapper<TSource, TTarget>(AbstractMapper<TSource, TTarget> mapper)
        {
            if (mapper == null) 
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            
            var sourceType = typeof(TSource).FullName;
            var targetType = typeof(TTarget).FullName;
            
            if (string.IsNullOrWhiteSpace(sourceType) || string.IsNullOrWhiteSpace(targetType))
            {
                throw new ArgumentException("Source and target types must have valid names.");
            }
            
            var key = Tuple.Create(sourceType, targetType);
            _mapperRegistry[key] = mapper;
            
            // Cache the types for faster lookup
            _typeRegistry[sourceType] = typeof(TSource);
            _typeRegistry[targetType] = typeof(TTarget);

            return this;
        }

        public IMapHandler RegisterValidator<T>(AbstractDMSValidator<T> validator)
        {
            if (validator == null) 
            {
                throw new ArgumentNullException(nameof(validator));
            }
            
            var typeName = typeof(T).FullName;
            
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentException("Type must have a valid name.");
            }
            
            _validatorRegistry[typeName] = validator;

            return this;
        }

        public IMapHandler RegisterInternalValidators<T>()
        {
            this.RegisterValidator(new ReservationValidator());
            this.RegisterValidator(new RoomValidator());

            return this;
        }

        public object Map(object data, string sourceType, string targetType)
        {
            #region // Check input
            if (data == null) 
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (string.IsNullOrWhiteSpace(sourceType)) 
            {
                throw new ArgumentException("Source type cannot be empty", nameof(sourceType));
            }
            if (string.IsNullOrWhiteSpace(targetType)) 
            {
                throw new ArgumentException("Target type cannot be empty", nameof(targetType));
            }

            var sourceModelType = GetTypeFromRegistry(sourceType);

            // Check if the input data object is of the correct type
            if (sourceModelType == null || !sourceModelType.IsInstanceOfType(data))
            {
                throw new SourceDataTypeMismatchException(sourceType, data?.GetType()?.FullName ?? "null");
            }
            #endregion

            // Validate source object before mapping
            ValidateObject(data, sourceType);

            // Execute the mapping
            var result = MapObject(data, sourceType, targetType);

            // Validate source object after mapping
            ValidateObject(result, targetType);

            return result;
        }

        /// <summary>
        /// Executes the mapping operation using the registered mapper
        /// </summary>
        /// <param name="data">The source data object to map</param>
        /// <param name="sourceType">The fully qualified name of the source type</param>
        /// <param name="targetType">The fully qualified name of the target type</param>
        /// <returns>The mapped object</returns>
        /// <exception cref="UnsupportedMappingException">Thrown when no mapper is found for the source and target type combination</exception>
        /// <exception cref="MappingException">Thrown when an error occurs during the mapping process</exception>
        private object MapObject(object data, string sourceType, string targetType)
        {
            // input data has been checked before

            // Try to get mapper from registry first
            var key = Tuple.Create(sourceType, targetType);
            if (_mapperRegistry.TryGetValue(key, out var registeredMapper))
            {
                try
                {
                    return registeredMapper.Map(data);
                }
                catch (Exception ex)
                {
                    throw new MappingException($"Error mapping from {sourceType} to {targetType}", ex);
                }
            }
            else
            {
                throw new UnsupportedMappingException(sourceType, targetType);
            }
        }

        /// <summary>
        /// Validates an object using the appropriate validator if one exists and throws an exception if validation fails or no validator was found.
        /// </summary>
        private void ValidateObject(object obj, string typeName)
        {
            // input data has been checked before

            if (_validatorRegistry.TryGetValue(typeName, out var registeredValidator))
            {
                var validated = registeredValidator.ValidateObject(obj);
                if (validated.Errors.Any())
                {
                    throw new Core.Exceptions.ValidationException(typeName, validated.Errors);
                }
            }
            else
            {
                throw new MappingException($"Validator for type '{typeName}' not found. If no validation is needed, create an empty validator for this type.");
            }
        }

        /// <summary>
        /// Gets type from cache first, then resolves if not cached
        /// </summary>
        private Type? GetTypeFromRegistry(string typeName)
        {
            if (_typeRegistry.TryGetValue(typeName, out var registeredType))
            {
                return registeredType;
            }
            else return null;
        }
    }
}