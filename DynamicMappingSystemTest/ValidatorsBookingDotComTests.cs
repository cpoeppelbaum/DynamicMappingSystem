using BookingDotCom;
using DynamicMappingSystem.BookingDotCom;
using DynamicMappingSystem.Core;
using FluentValidation.TestHelper;

namespace DynamicMappingSystemTest
{
    public class BookingDotComValidatingTests
    {
        private readonly AbstractDMSValidator<Booking> _bookingValidator;
        private readonly AbstractDMSValidator<Guest> _guestValidator;
        private readonly AbstractDMSValidator<Room> _roomValidator;

        public BookingDotComValidatingTests()
        {
            var mapHandlerStub = new MapHandlerStub();
            mapHandlerStub.AddBookingDotComMappers();

            _bookingValidator = mapHandlerStub.GetValidator<Booking>();
            _guestValidator = mapHandlerStub.GetValidator<Guest>();
            _roomValidator = mapHandlerStub.GetValidator<Room>();

            // Assert that all three validators have been registered
            mapHandlerStub.ValidateAllValidatorsHaveBeenAskedFor();
            Assert.NotNull(_bookingValidator);
            Assert.NotNull(_guestValidator);
            Assert.NotNull(_roomValidator);

        }

        [Fact]
        public void Map_ValidBooking_ShouldPassValidation()
        {
            // Arrange
            var validBooking = new Booking
            {
                BookingId = 12345,
                ArrivalDate = "2024-08-20",
                DepartureDate = "2024-08-23",
                GuestDetails = new Guest
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    EmailAddress = "alice.johnson@email.com"
                },
                AdultCount = 2,
                RoomTypeId = 789,
                TotalPrice = 520.75m
            };

            // Act & Assert
            var result = _bookingValidator.TestValidate(validBooking);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Map_BookingWithInvalidId_ShouldFailValidation()
        {
            // Arrange
            var invalidBooking = new Booking
            {
                BookingId = 0, // Invalid
                ArrivalDate = "2024-08-20",
                DepartureDate = "2024-08-23",
                GuestDetails = new Guest
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    EmailAddress = "alice.johnson@email.com"
                },
                AdultCount = 2,
                RoomTypeId = 789,
                TotalPrice = 520.75m
            };

            // Act & Assert
            var result = _bookingValidator.TestValidate(invalidBooking);
            result.ShouldHaveValidationErrorFor(x => x.BookingId)
                  .WithErrorMessage("BookingId must be greater than 0.");
        }

        [Fact]
        public void Map_BookingWithInvalidDateRange_ShouldFailValidation()
        {
            // Arrange
            var invalidBooking = new Booking
            {
                BookingId = 12345,
                ArrivalDate = "2024-08-25", // After departure date
                DepartureDate = "2024-08-23",
                GuestDetails = new Guest
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    EmailAddress = "alice.johnson@email.com"
                },
                AdultCount = 2,
                RoomTypeId = 789,
                TotalPrice = 520.75m
            };

            // Act & Assert
            var result = _bookingValidator.TestValidate(invalidBooking);
            result.ShouldHaveValidationErrorFor(x => x)
                  .WithErrorMessage("Arrival date must be before departure date.");
        }

        [Fact]
        public void Map_BookingWithInvalidDates_ShouldFailValidation()
        {
            // Arrange
            var invalidBooking = new Booking
            {
                BookingId = 12345,
                ArrivalDate = "invalid-date",
                DepartureDate = "2024-08-23",
                GuestDetails = new Guest
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    EmailAddress = "alice.johnson@email.com"
                },
                AdultCount = 2,
                RoomTypeId = 789,
                TotalPrice = 520.75m
            };

            // Act & Assert
            var result = _bookingValidator.TestValidate(invalidBooking);
            result.ShouldHaveValidationErrorFor(x => x.ArrivalDate)
                  .WithErrorMessage("Arrival date must be a valid ISO date.");
        }

        [Fact]
        public void Map_ValidGuest_ShouldPassValidation()
        {
            // Arrange
            var validGuest = new Guest
            {
                FirstName = "Alice",
                LastName = "Johnson",
                EmailAddress = "alice.johnson@email.com"
            };

            // Act & Assert
            var result = _guestValidator.TestValidate(validGuest);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Map_GuestWithEmptyFirstName_ShouldFailValidation()
        {
            // Arrange
            var invalidGuest = new Guest
            {
                FirstName = "", // Invalid
                LastName = "Johnson",
                EmailAddress = "alice.johnson@email.com"
            };

            // Act & Assert
            var result = _guestValidator.TestValidate(invalidGuest);
            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                  .WithErrorMessage("First name is required.");
        }

        [Fact]
        public void Map_GuestWithInvalidEmail_ShouldFailValidation()
        {
            // Arrange
            var invalidGuest = new Guest
            {
                FirstName = "Alice",
                LastName = "Johnson",
                EmailAddress = "invalid-email" // Invalid
            };

            // Act & Assert
            var result = _guestValidator.TestValidate(invalidGuest);
            result.ShouldHaveValidationErrorFor(x => x.EmailAddress)
                  .WithErrorMessage("Email address must be valid.");
        }

        [Fact]
        public void Map_ValidRoom_ShouldPassValidation()
        {
            // Arrange
            var validRoom = new Room
            {
                RoomTypeId = 555,
                RoomTypeName = "Standard Double",
                MaxGuests = 2,
                BasePrice = 89.99m,
                Amenities = new List<string> { "WiFi", "TV" }
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(validRoom);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Map_RoomWithInvalidPrice_ShouldFailValidation()
        {
            // Arrange
            var invalidRoom = new Room
            {
                RoomTypeId = 555,
                RoomTypeName = "Standard Double",
                MaxGuests = 2,
                BasePrice = 0, // Invalid
                Amenities = new List<string> { "WiFi", "TV" }
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(invalidRoom);
            result.ShouldHaveValidationErrorFor(x => x.BasePrice)
                  .WithErrorMessage("BasePrice must be greater than 0.");
        }

        [Fact]
        public void Map_RoomWithEmptyAmenity_ShouldFailValidation()
        {
            // Arrange
            var invalidRoom = new Room
            {
                RoomTypeId = 555,
                RoomTypeName = "Standard Double",
                MaxGuests = 2,
                BasePrice = 89.99m,
                Amenities = new List<string> { "WiFi", "" } // Empty amenity
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(invalidRoom);
            result.ShouldHaveValidationErrorFor("Amenities[1]")
                  .WithErrorMessage("Amenities must not be empty.");
        }
    }
}
