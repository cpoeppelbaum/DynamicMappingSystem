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
        public Type ExpectedType { get; }
        public Type ActualType { get; }

        public SourceDataTypeMismatchException(Type expectedType, Type actualType)
            : base($"Data type mismatch. Expected: {expectedType.Name}, Actual: {actualType.Name}")
        {
            ExpectedType = expectedType;
            ActualType = actualType;
        }
    }
}