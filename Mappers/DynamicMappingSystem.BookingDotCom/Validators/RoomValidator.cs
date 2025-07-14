using BookingDotCom;
using DynamicMappingSystem.Core;
using FluentValidation;

namespace DynamicMappingSystem.BookingDotCom.Validators
{
    public class RoomValidator : AbstractDMSValidator<Room>
    {
        public RoomValidator()
        {        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0)
            .WithMessage("RoomTypeId must be greater than 0.");
        RuleFor(x => x.RoomTypeName)
            .NotEmpty()
            .WithMessage("RoomTypeName is required.")
            .MaximumLength(200)
            .WithMessage("RoomTypeName must not exceed 200 characters.");
        RuleFor(x => x.MaxGuests)
            .GreaterThan(0)
            .WithMessage("MaxGuests must be at least 1.");
        RuleFor(x => x.BasePrice)
            .GreaterThan(0)
            .WithMessage("BasePrice must be greater than 0.");
        RuleForEach(x => x.Amenities)
            .NotEmpty()
            .WithMessage("Amenities must not be empty.");
        }
    }
}