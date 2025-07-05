using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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

            // Validate source object before conversion
            var sourceValidationResult = ValidateObject(data, sourceModelType, sourceType);
            if (sourceValidationResult.Any())
            {
                throw new Core.Exceptions.ValidationException(sourceType, sourceValidationResult);
            }

            // Create generic converter type
            var converterType = typeof(IMapper<,>).MakeGenericType(sourceModelType, targetModelType);
            
            // Get converter from registry
            var converter = _serviceProvider.GetService(converterType);
            
            if (converter == null)
            {
                throw new UnsupportedMappingException(sourceType, targetType);
            }

            var convertMethod = converterType.GetMethod("Map");

            object result;
            try
            {
                // Now actually invoke conversion method
                result = convertMethod!.Invoke(converter, new[] { data })!;
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

            // Validate target object after conversion
            var convertedValidationResult = ValidateObject(result, targetModelType, targetType);
            if (convertedValidationResult.Any())
            {
                throw new Core.Exceptions.ValidationException(targetType, convertedValidationResult);
            }

            return result;
        }

        /// <summary>
        /// Validates an object using the appropriate validator if one exists
        /// </summary>
        private List<string> ValidateObject(object obj, Type objectType, string typeName)
        {
            if (obj == null) { throw new ArgumentNullException("obj"); }

            // Create validator type for the object
            var validatorType = typeof(FluentValidation.IValidator<>).MakeGenericType(objectType);
            
            // Try to get validator from service provider
            var validator = _serviceProvider.GetService(validatorType);
            
            if (validator != null)
            {
                // Get the Validate method
                var validateMethod = validatorType.GetMethod("Validate", new[] { objectType });
                
                if (validateMethod != null)
                {
                    // Invoke validation
                    var validationResult = validateMethod.Invoke(validator, new[] { obj });
                        
                    // Check if it's a FluentValidation ValidationResult
                    var isValidProperty = validationResult?.GetType().GetProperty("IsValid");
                    var errorsProperty = validationResult?.GetType().GetProperty("Errors");
                        
                    if (isValidProperty != null && errorsProperty != null)
                    {
                        var isValid = (bool)(isValidProperty.GetValue(validationResult) ?? true);
                            
                        if (!isValid)
                        {
                            var errors = errorsProperty.GetValue(validationResult);
                            var errorMessages = new List<string>();
                                
                            // Extract error messages from FluentValidation errors
                            if (errors is System.Collections.IEnumerable errorCollection)
                            {
                                foreach (var error in errorCollection)
                                {
                                    var errorMessageProperty = error?.GetType().GetProperty("ErrorMessage");
                                    if (errorMessageProperty != null)
                                    {
                                        var errorMessage = errorMessageProperty.GetValue(error)?.ToString();
                                        if (!string.IsNullOrEmpty(errorMessage))
                                        {
                                            errorMessages.Add(errorMessage);
                                        }
                                    }
                                }
                            }
                            // Validation completed, return error messages
                            return errorMessages;
                        }
                        // Validation completed, no errors found
                        return new List<string>();
                    }
                    throw new MappingException($"Validator for type '{typeName}' does not have a valid 'IsValid' or 'Errors' property.");
                }
                throw new MappingException($"Validator for type '{typeName}' does not have a valid 'Validate' method.");
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
    }
}