using DynamicMappingSystem.Core;
using FluentValidation;
using Google;

namespace DynamicMappingSystem.Google.Validators
{
    internal class RoomValidator : AbstractDMSValidator<Room>
    {
        public RoomValidator()
        {
            RuleFor(x => x.AccommodationId)
                .NotEmpty()
                .WithMessage("AccommodationId is required.")
                .MaximumLength(100)
                .WithMessage("AccommodationId must not exceed 100 characters.");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required.")
                .MaximumLength(200)
                .WithMessage("Category must not exceed 200 characters.");

            RuleFor(x => x.NightlyRate)
                .GreaterThan(0)
                .WithMessage("NightlyRate must be greater than 0.");

            RuleFor(x => x.GuestCapacity)
                .GreaterThan(0)
                .WithMessage("GuestCapacity must be at least 1.");

            RuleFor(x => x.Features)
                .NotNull()
                .WithMessage("Features cannot be null.");

            RuleForEach(x => x.Features)
                .NotEmpty()
                .WithMessage("Features must not contain empty values.")
                .MaximumLength(100)
                .WithMessage("Each feature must not exceed 100 characters.");
        }
    }
}