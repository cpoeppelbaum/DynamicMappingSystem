using BookingDotCom;
using DynamicMappingSystem.Core;
using FluentValidation;

namespace DynamicMappingSystem.BookingDotCom.Validators
{
    internal class BookingValidator : AbstractDMSValidator<Booking>
    {
        public BookingValidator()
        {        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("BookingId must be greater than 0.");

        RuleFor(x => x.ArrivalDate)
            .NotEmpty()
            .WithMessage("Arrival date is required.")
            .Must(BeValidIsoDate)
            .WithMessage("Arrival date must be a valid ISO date.");

        RuleFor(x => x.DepartureDate)
            .NotEmpty()
            .WithMessage("Departure date is required.")
            .Must(BeValidIsoDate)
            .WithMessage("Departure date must be a valid ISO date.");

        RuleFor(x => x)
            .Must(x => BeValidDateRange(x.ArrivalDate, x.DepartureDate))
            .WithMessage("Arrival date must be before departure date.");

        RuleFor(x => x.AdultCount)
            .GreaterThan(0)
            .WithMessage("Adult count must be at least 1.");

        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0)
            .WithMessage("RoomTypeId must be greater than 0.");

        RuleFor(x => x.TotalPrice)
            .GreaterThan(0)
            .WithMessage("Total price must be greater than 0.");

        RuleFor(x => x.GuestDetails)
            .NotNull()
            .WithMessage("Guest details are required.")
            .SetValidator(new GuestValidator());
        }

        private bool BeValidIsoDate(string dateString)
        {
            return DateTime.TryParse(dateString, out _);
        }

        private bool BeValidDateRange(string arrivalDate, string departureDate)
        {
            if (DateTime.TryParse(arrivalDate, out var arrival) &&
                DateTime.TryParse(departureDate, out var departure))
            {
                return arrival < departure;
            }
            return false;
        }
    }
}