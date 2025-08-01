using DynamicMappingSystem.Core;
using DynamicMappingSystem.Google;
using FluentValidation.TestHelper;
using Google;

namespace DynamicMappingSystemTest
{
    public class GoogleValidatingTests
    {
        private readonly AbstractDMSValidator<Reservation> _reservationValidator;
        private readonly AbstractDMSValidator<Room> _roomValidator;

        public GoogleValidatingTests()
        {
            var mapHandlerStub = new MapHandlerStub();
            mapHandlerStub.AddGoogleMappers();

            _reservationValidator = mapHandlerStub.GetValidator<Reservation>();
            _roomValidator = mapHandlerStub.GetValidator<Room>();

            // Assert that the validator have been registered
            mapHandlerStub.ValidateAllValidatorsHaveBeenAskedFor();
            Assert.NotNull(_reservationValidator);

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

        [Fact]
        public void Map_ValidRoom_ShouldPassValidation()
        {
            // Arrange
            var validRoom = new Room
            {
                AccommodationId = "GOOG-ROOM-123",
                Category = "Deluxe Suite",
                NightlyRate = 199.99,
                GuestCapacity = 4,
                Features = new[] { "WiFi", "TV", "Balcony" }
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(validRoom);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Map_RoomWithEmptyAccommodationId_ShouldFailValidation()
        {
            // Arrange
            var invalidRoom = new Room
            {
                AccommodationId = "",
                Category = "Deluxe Suite",
                NightlyRate = 199.99,
                GuestCapacity = 4,
                Features = new[] { "WiFi", "TV" }
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(invalidRoom);
            result.ShouldHaveValidationErrorFor(x => x.AccommodationId)
                  .WithErrorMessage("AccommodationId is required.");
        }

        [Fact]
        public void Map_RoomWithNegativeNightlyRate_ShouldFailValidation()
        {
            // Arrange
            var invalidRoom = new Room
            {
                AccommodationId = "GOOG-ROOM-123",
                Category = "Deluxe Suite",
                NightlyRate = -10.0,
                GuestCapacity = 2,
                Features = new[] { "WiFi" }
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(invalidRoom);
            result.ShouldHaveValidationErrorFor(x => x.NightlyRate)
                  .WithErrorMessage("NightlyRate must be greater than 0.");
        }

        [Fact]
        public void Map_RoomWithZeroGuestCapacity_ShouldFailValidation()
        {
            // Arrange
            var invalidRoom = new Room
            {
                AccommodationId = "GOOG-ROOM-123",
                Category = "Deluxe Suite",
                NightlyRate = 100.0,
                GuestCapacity = 0,
                Features = new[] { "WiFi" }
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(invalidRoom);
            result.ShouldHaveValidationErrorFor(x => x.GuestCapacity)
                  .WithErrorMessage("GuestCapacity must be at least 1.");
        }

        [Fact]
        public void Map_RoomWithLongCategory_ShouldFailValidation()
        {
            // Arrange
            var invalidRoom = new Room
            {
                AccommodationId = "GOOG-ROOM-123",
                Category = new string('X', 201), // Too long for 200 max
                NightlyRate = 100.0,
                GuestCapacity = 2,
                Features = new[] { "WiFi" }
            };

            // Act & Assert
            var result = _roomValidator.TestValidate(invalidRoom);
            result.ShouldHaveValidationErrorFor(x => x.Category)
                  .WithErrorMessage("Category must not exceed 200 characters.");
        }
    }
}
