using FluentValidation;

namespace DynamicMappingSystem.Core
{
    public abstract class AbstractDMSValidator<T> : AbstractValidator<T>, IDMSValidator
    {
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
