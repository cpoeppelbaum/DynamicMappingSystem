using Google;
using FluentValidation.TestHelper;
using DynamicMappingSystem.Google.Validators;

namespace DynamicMappingSystemTest
{
    public class GoogleValidatingTests
    {
        private readonly ReservationValidator _reservationValidator;

        public GoogleValidatingTests()
        {
            _reservationValidator = new ReservationValidator();
        }

        [Fact]
        public void Map_ValidReservation_ShouldPassValidation()
        {
            // Arrange
            var validReservation = new Reservation
            {
                ReservationId = "GOOG-789",
                CheckInTimestamp = new DateTimeOffset(2024, 7, 10, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                CheckOutTimestamp = new DateTimeOffset(2024, 7, 13, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                GuestFullName = "Jane Smith",
                PartySize = 3,
                AccommodationId = "GOOG-ROOM-123",
                TotalCost = 675.50
            };

            // Act & Assert
            var result = _reservationValidator.TestValidate(validReservation);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Map_ReservationWithEmptyId_ShouldFailValidation()
        {
            // Arrange
            var invalidReservation = new Reservation
            {
                ReservationId = "", // Invalid
                CheckInTimestamp = new DateTimeOffset(2024, 7, 10, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                CheckOutTimestamp = new DateTimeOffset(2024, 7, 13, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                GuestFullName = "Jane Smith",
                PartySize = 3,
                AccommodationId = "GOOG-ROOM-123",
                TotalCost = 675.50
            };

            // Act & Assert
            var result = _reservationValidator.TestValidate(invalidReservation);
            result.ShouldHaveValidationErrorFor(x => x.ReservationId)
                  .WithErrorMessage("ReservationId is required.");
        }

        [Fact]
        public void Map_ReservationWithInvalidTimestamps_ShouldFailValidation()
        {
            // Arrange
            var invalidReservation = new Reservation
            {
                ReservationId = "GOOG-789",
                CheckInTimestamp = 1000, // Later than checkout
                CheckOutTimestamp = 500,
                GuestFullName = "Jane Smith",
                PartySize = 3,
                AccommodationId = "GOOG-ROOM-123",
                TotalCost = 675.50
            };

            // Act & Assert
            var result = _reservationValidator.TestValidate(invalidReservation);
            result.ShouldHaveValidationErrorFor(x => x.CheckOutTimestamp)
                  .WithErrorMessage("CheckOutTimestamp must be greater than CheckInTimestamp.");
        }

        [Fact]
        public void Map_ReservationWithInvalidPartySize_ShouldFailValidation()
        {
            // Arrange
            var invalidReservation = new Reservation
            {
                ReservationId = "GOOG-789",
                CheckInTimestamp = new DateTimeOffset(2024, 7, 10, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                CheckOutTimestamp = new DateTimeOffset(2024, 7, 13, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                GuestFullName = "Jane Smith",
                PartySize = 0, // Invalid
                AccommodationId = "GOOG-ROOM-123",
                TotalCost = 675.50
            };

            // Act & Assert
            var result = _reservationValidator.TestValidate(invalidReservation);
            result.ShouldHaveValidationErrorFor(x => x.PartySize)
                  .WithErrorMessage("PartySize must be at least 1.");
        }

        [Fact]
        public void Map_ReservationWithLongGuestName_ShouldFailValidation()
        {
            // Arrange
            var invalidReservation = new Reservation
            {
                ReservationId = "GOOG-789",
                CheckInTimestamp = new DateTimeOffset(2024, 7, 10, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                CheckOutTimestamp = new DateTimeOffset(2024, 7, 13, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                GuestFullName = new string('A', 201), // Too long
                PartySize = 3,
                AccommodationId = "GOOG-ROOM-123",
                TotalCost = 675.50
            };

            // Act & Assert
            var result = _reservationValidator.TestValidate(invalidReservation);
            result.ShouldHaveValidationErrorFor(x => x.GuestFullName)
                  .WithErrorMessage("GuestFullName must not exceed 200 characters.");
        }
    }
}
