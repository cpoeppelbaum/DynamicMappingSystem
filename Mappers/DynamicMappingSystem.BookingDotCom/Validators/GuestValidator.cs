using BookingDotCom;
using DynamicMappingSystem.Core;
using FluentValidation;

namespace DynamicMappingSystem.BookingDotCom.Validators
{
    internal class GuestValidator : AbstractDMSValidator<Guest>
    {
        public GuestValidator()
        {        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(100)
            .WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(100)
            .WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address is required.")
            .EmailAddress()
            .WithMessage("Email address must be valid.");
        }
    }
}