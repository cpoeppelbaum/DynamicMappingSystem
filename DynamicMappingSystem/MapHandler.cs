using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using DynamicMappingSystem.Validators;
using FluentValidation;
using System.Reflection;

namespace DynamicMappingSystem
{
    /// <summary>
    /// Main implementation of the data converter
    /// </summary>
    public class MapHandler : IMapHandler
    {
        private readonly Dictionary<Tuple<string, string>, IMapper> _converterRegistry = new Dictionary<Tuple<string, string>, IMapper>();
        private readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
        private readonly Dictionary<string, IDMSValidator> _validatorRegistry = new Dictionary<string, IDMSValidator>();

        private MapHandler() { }

        public static IMapHandler Create()
        {
            return new MapHandler();
        }

        /// <summary>
        /// Registers a converter instance
        /// </summary>
        public IMapHandler RegisterMapper<TSource, TTarget>(IMapper<TSource, TTarget> converter)
        {
            if (converter == null) 
            {
                throw new ArgumentNullException(nameof(converter));
            }
            
            var sourceType = typeof(TSource).FullName;
            var targetType = typeof(TTarget).FullName;
            
            if (string.IsNullOrWhiteSpace(sourceType) || string.IsNullOrWhiteSpace(targetType))
            {
                throw new ArgumentException("Source and target types must have valid names.");
            }
            
            var key = Tuple.Create(sourceType, targetType);
            _converterRegistry[key] = converter;
            
            // Cache the types for faster lookup
            _typeCache[sourceType] = typeof(TSource);
            _typeCache[targetType] = typeof(TTarget);

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
            
            // Cache the type for faster lookup
            _typeCache[typeName] = typeof(T);

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
            // Validation
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

            var sourceModelType = GetTypeFromCacheOrResolve(sourceType);

            // Check if the input data object is of the correct type
            if (sourceModelType == null || !sourceModelType.IsInstanceOfType(data))
            {
                throw new SourceDataTypeMismatchException(sourceType, data?.GetType()?.FullName ?? "null");
            }

            // Validate source object before conversion
            var sourceValidationResult = ValidateObject(data, sourceType);
            if (sourceValidationResult.Any())
            {
                throw new Core.Exceptions.ValidationException(sourceType, sourceValidationResult);
            }

            // Try to get converter from registry first
            var key = Tuple.Create(sourceType, targetType);
            IMapper? mapper = null;
            
            if (_converterRegistry.TryGetValue(key, out var registeredConverter))
            {
                mapper = registeredConverter;
            }
            else
            {
                throw new UnsupportedMappingException(sourceType, targetType);
            }

            object result;
            try
            {
                result = mapper.Map(data);
            }
            catch (TargetInvocationException ex)
            {
                if(ex.InnerException == null)
                {
                    throw new MappingException($"Error converting from {sourceType} to {targetType}", ex);
                }

                // Re-throw specific MappingExceptions directly
                if (ex.InnerException is MappingException mappingEx)
                {
                    throw mappingEx;
                }
                    
                throw new MappingException($"Error converting from {sourceType} to {targetType}", ex.InnerException);
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Error converting from {sourceType} to {targetType}", ex);
            }

            var convertedValidationResult = ValidateObject(result, targetType);
            if (convertedValidationResult.Any())
            {
                throw new Core.Exceptions.ValidationException(targetType, convertedValidationResult);
            }

            return result;
        }

        /// <summary>
        /// Validates an object using the appropriate validator if one exists
        /// </summary>
        private List<string> ValidateObject(object obj, string typeName)
        {
            if (obj == null) { throw new ArgumentNullException(nameof(obj)); }

            // Try to get validator from registry first (fastest)
            if (_validatorRegistry.TryGetValue(typeName, out var registeredValidator))
            {
                return registeredValidator.ValidateObject(obj).Errors;
            }

            throw new MappingException($"Validator for type '{typeName}' not found. If no validation is needed, create an empty validator for this type.");
        }

        /// <summary>
        /// Resolves a type name to a Type object by searching through all loaded assemblies
        /// </summary>
        private static Type? ResolveType(string typeName)
        {
            // First try to load the type directly
            var type = Type.GetType(typeName, false);
            if (type != null)
            {
                return type;
            }

            // If not found, search through all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName, false);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets type from cache first, then resolves if not cached
        /// </summary>
        private Type? GetTypeFromCacheOrResolve(string typeName)
        {
            if (_typeCache.TryGetValue(typeName, out var cachedType))
            {
                return cachedType;
            }

            var resolvedType = ResolveType(typeName);
            if (resolvedType != null)
            {
                _typeCache[typeName] = resolvedType;
            }

            return resolvedType;
        }
    }
}