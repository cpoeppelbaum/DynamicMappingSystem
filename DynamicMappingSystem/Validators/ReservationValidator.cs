using DynamicMappingSystem.Core;
using FluentValidation;
using Models;

namespace DynamicMappingSystem.Validators
{
    internal class ReservationValidator : AbstractDMSValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.")
                .MaximumLength(50)
                .WithMessage("Id must not exceed 50 characters.");

            RuleFor(x => x.CheckInDate)
                .NotEmpty()
                .WithMessage("CheckInDate is required.")
                .Must(BeValidDate)
                .WithMessage("CheckInDate must be a valid date.");

            RuleFor(x => x.CheckOutDate)
                .NotEmpty()
                .WithMessage("CheckOutDate is required.")
                .Must(BeValidDate)
                .WithMessage("CheckOutDate must be a valid date.");

            RuleFor(x => x)
                .Must(x => BeValidDateRange(x.CheckInDate, x.CheckOutDate))
                .WithMessage("CheckInDate must be before CheckOutDate.");

            RuleFor(x => x.GuestName)
                .NotEmpty()
                .WithMessage("GuestName is required.")
                .MaximumLength(200)
                .WithMessage("GuestName must not exceed 200 characters.");

            RuleFor(x => x.NumberOfGuests)
                .GreaterThan(0)
                .WithMessage("NumberOfGuests must be at least 1.");

            RuleFor(x => x.RoomId)
                .NotEmpty()
                .WithMessage("RoomId is required.")
                .MaximumLength(50)
                .WithMessage("RoomId must not exceed 50 characters.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("TotalAmount must be greater than 0.");
        }

        private bool BeValidDate(DateTime date)
        {
            return date != DateTime.MinValue && date != DateTime.MaxValue;
        }

        private bool BeValidDateRange(DateTime checkInDate, DateTime checkOutDate)
        {
            return checkInDate < checkOutDate;
        }
    }
}