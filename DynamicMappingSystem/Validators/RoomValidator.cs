using FluentValidation;
using Models;

namespace DynamicMappingSystem.Validators
{
    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.")
                .MaximumLength(50)
                .WithMessage("Id must not exceed 50 characters.");

            RuleFor(x => x.RoomType)
                .NotEmpty()
                .WithMessage("RoomType is required.")
                .MaximumLength(200)
                .WithMessage("RoomType must not exceed 200 characters.");

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0)
                .WithMessage("PricePerNight must be greater than 0.");

            RuleFor(x => x.MaxOccupancy)
                .GreaterThan(0)
                .WithMessage("MaxOccupancy must be at least 1.");

            RuleFor(x => x.Amenities)
                .NotNull()
                .WithMessage("Amenities cannot be null.");

            RuleForEach(x => x.Amenities)
                .NotEmpty()
                .WithMessage("Amenities must not contain empty values.")
                .MaximumLength(100)
                .WithMessage("Each amenity must not exceed 100 characters.");
        }
    }
}