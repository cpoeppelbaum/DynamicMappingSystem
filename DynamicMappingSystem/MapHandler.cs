using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using System.Reflection;

namespace DynamicMappingSystem
{
    /// <summary>
    /// Main implementation of the data converter
    /// </summary>
    public class MapHandler : IMapHandler
    {
        private readonly IServiceProvider _serviceProvider;
        
        public MapHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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

            var sourceModelType = ResolveType(sourceType);
            var targetModelType = ResolveType(targetType);

            if (sourceModelType == null)
            {
                throw new ArgumentException($"Unsupported source type: {sourceType}", nameof(sourceType));
            }
            if (targetModelType == null)
            {
                throw new ArgumentException($"Unsupported target type: {targetType}", nameof(targetType));
            }
                
            // Check if the input data object is of the correct type
            if (!sourceModelType.IsInstanceOfType(data))
            {
                throw new SourceDataTypeMismatchException(sourceModelType, data.GetType());
            }
                
            // Create generic converter type
            var converterType = typeof(IMapper<,>).MakeGenericType(sourceModelType, targetModelType);
            
            // Get converter from registry
            var converter = _serviceProvider.GetService(converterType);
            
            if (converter == null)
            {
                throw new UnsupportedMappingException(sourceType, targetType);
            }

            var convertMethod = converterType.GetMethod("Convert");

            try
            {
                // Now actually invoke conversion method
                return convertMethod!.Invoke(converter, new[] { data })!;
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
    }
}