namespace DynamicMappingSystem.Core
{
    public class DMSResult
    {
        public DMSResult(bool isValid = true, List<string>? errors = null)
        {
            IsValid = isValid;
            Errors = errors ?? new List<string>();
        }

        /// <summary>
        /// Indicates whether the validation was successful.
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the validation errors, if any.
        /// </summary>
        public List<string> Errors { get; }
    }
}
