namespace DynamicMappingSystem.Core.Exceptions
{
    public class MappingException : Exception
    {
        public MappingException(string message) : base(message)
        {
        }

        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class UnsupportedMappingException : MappingException
    {
        public string SourceType { get; }
        public string TargetType { get; }

        public UnsupportedMappingException(string sourceType, string targetType) 
            : base($"Mapping from '{sourceType}' to '{targetType}' is not supported")
        {
            SourceType = sourceType;
            TargetType = targetType;
        }
    }

    public class SourceDataTypeMismatchException : MappingException
    {
        public string ExpectedType { get; }
        public string ActualType { get; }

        public SourceDataTypeMismatchException(string expectedType, string actualType)
            : base($"Data type mismatch. Expected: {expectedType}, Actual: {actualType}")
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }
    }

    public class ValidationException : MappingException
    {
        public IReadOnlyList<string> ValidationErrors { get; }
        public string ValidatedType { get; }

        public ValidationException(string validatedType, IList<string> validationErrors)
            : base($"Validation failed for {validatedType}: {string.Join("; ", validationErrors)}")
        {
            ValidatedType = validatedType;
            ValidationErrors = validationErrors.ToList().AsReadOnly();
        }
    }
}