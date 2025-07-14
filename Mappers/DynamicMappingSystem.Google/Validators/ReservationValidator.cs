using DynamicMappingSystem.Core;
using FluentValidation;
using Google;

namespace DynamicMappingSystem.Google.Validators
{
    public class ReservationValidator : AbstractDMSValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(x => x.ReservationId)
                .NotEmpty()
                .WithMessage("ReservationId is required.");
            RuleFor(x => x.CheckInTimestamp)
                .GreaterThan(0)
                .WithMessage("CheckInTimestamp must be greater than 0.");
            RuleFor(x => x.CheckOutTimestamp)
                .GreaterThan(x => x.CheckInTimestamp)
                .WithMessage("CheckOutTimestamp must be greater than CheckInTimestamp.");
            RuleFor(x => x.GuestFullName)
                .NotEmpty()
                .WithMessage("GuestFullName is required.")
                .MaximumLength(200)
                .WithMessage("GuestFullName must not exceed 200 characters.");
            RuleFor(x => x.PartySize)
                .GreaterThan(0)
                .WithMessage("PartySize must be at least 1.");
            RuleFor(x => x.AccommodationId)
                .NotEmpty()
                .WithMessage("AccommodationId is required.");
            RuleFor(x => x.TotalCost)
                .GreaterThan(0)
                .WithMessage("TotalCost must be greater than 0.");
        }
    }
}