using FluentValidation;

namespace DynamicMappingSystem.Core
{
    /// <summary>
    /// Base class to enforce typed validation.
    /// </summary>
    public abstract class AbstractDMSValidator<T> : AbstractValidator<T>, IDMSValidator
    {
        /// <summary>
        /// Validates any object against the rules of this validator.
        /// Throws an ArgumentException if the type does not match T.
        /// Returns a DMSResult containing validation status and error messages.
        /// </summary>
        /// <param name="data">The object to validate</param>
        /// <returns>DMSResult containing validation status and errors</returns>
        public DMSResult ValidateObject(object data)
        {
            if (!(data is T sourceTyped))
            {
                throw new ArgumentException($"Unsupported source type: {data.GetType().FullName}. Expected type: {typeof(T).FullName}");
            }

            var result = Validate((T)data);
            return new DMSResult(result.IsValid, result.Errors.Select(e => e.ErrorMessage).ToList());
        }
    }
}
