using DynamicMappingSystem;
using Moq;

namespace DynamicMappingSystemTest
{
    public class MapHandlerTests
    {
        private readonly IMapHandler _mapHandler;

        public MapHandlerTests()
        {
            _mapHandler = new Mock<IMapHandler>().Object;
        }

        [Fact]
        public void Map_InternalReservationToGoogle_ShouldMapCorrectly()
        {
            // Arrange
            var internalReservation = new DynamicMappingSystem.Models.Reservation
            {
                Id = "INT-123",
                CheckInDate = new DateTime(2024, 6, 15),
                CheckOutDate = new DateTime(2024, 6, 18),
                GuestName = "John Doe",
                NumberOfGuests = 2,
                RoomId = "ROOM-456",
                TotalAmount = 450.00m
            };

            // Act
            var result = _mapHandler.Map(internalReservation, "Models.Reservation", "Google.Reservation");

            // Assert
            Assert.NotNull(result);
            var googleReservation = Assert.IsType<Google.Reservation>(result);
            Assert.Equal("INT-123", googleReservation.ReservationId);
            Assert.Equal("John Doe", googleReservation.GuestFullName);
            Assert.Equal(2, googleReservation.PartySize);
            Assert.Equal("ROOM-456", googleReservation.AccommodationId);
            Assert.Equal(450.00, googleReservation.TotalCost);
            
            // Check timestamp conversion (DateTime to Unix timestamp)
            var expectedCheckIn = ((DateTimeOffset)new DateTime(2024, 6, 15)).ToUnixTimeSeconds();
            var expectedCheckOut = ((DateTimeOffset)new DateTime(2024, 6, 18)).ToUnixTimeSeconds();
            Assert.Equal(expectedCheckIn, googleReservation.CheckInTimestamp);
            Assert.Equal(expectedCheckOut, googleReservation.CheckOutTimestamp);
        }

        [Fact]
        public void Map_GoogleReservationToInternal_ShouldMapCorrectly()
        {
            // Arrange
            var googleReservation = new Google.Reservation
            {
                ReservationId = "GOOG-789",
                CheckInTimestamp = ((DateTimeOffset)new DateTime(2024, 7, 10)).ToUnixTimeSeconds(),
                CheckOutTimestamp = ((DateTimeOffset)new DateTime(2024, 7, 13)).ToUnixTimeSeconds(),
                GuestFullName = "Jane Smith",
                PartySize = 3,
                AccommodationId = "GOOG-ROOM-123",
                TotalCost = 675.50
            };

            // Act
            var result = _mapHandler.Map(googleReservation, "Google.Reservation", "Models.Reservation");

            // Assert
            Assert.NotNull(result);
            var internalReservation = Assert.IsType<DynamicMappingSystem.Models.Reservation>(result);
            Assert.Equal("GOOG-789", internalReservation.Id);
            Assert.Equal(new DateTime(2024, 7, 10), internalReservation.CheckInDate);
            Assert.Equal(new DateTime(2024, 7, 13), internalReservation.CheckOutDate);
            Assert.Equal("Jane Smith", internalReservation.GuestName);
            Assert.Equal(3, internalReservation.NumberOfGuests);
            Assert.Equal("GOOG-ROOM-123", internalReservation.RoomId);
            Assert.Equal(675.50m, internalReservation.TotalAmount);
        }

        [Fact]
        public void Map_BookingDotComToInternal_WithComposedGuest_ShouldMapCorrectly()
        {
            // Arrange
            var bookingDotComBooking = new BookingDotCom.Booking
            {
                BookingId = 12345,
                ArrivalDate = "2024-08-20",
                DepartureDate = "2024-08-23",
                GuestDetails = new BookingDotCom.Guest
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    EmailAddress = "alice.johnson@email.com"
                },
                AdultCount = 2,
                RoomTypeId = 789,
                TotalPrice = 520.75m
            };

            // Act
            var result = _mapHandler.Map(bookingDotComBooking, "BookingDotCom.Booking", "Models.Reservation");

            // Assert
            Assert.NotNull(result);
            var internalReservation = Assert.IsType<DynamicMappingSystem.Models.Reservation>(result);
            Assert.Equal("12345", internalReservation.Id);
            Assert.Equal(new DateTime(2024, 8, 20), internalReservation.CheckInDate);
            Assert.Equal(new DateTime(2024, 8, 23), internalReservation.CheckOutDate);
            Assert.Equal("Alice Johnson", internalReservation.GuestName); // Composed from FirstName + LastName
            Assert.Equal(2, internalReservation.NumberOfGuests);
            Assert.Equal("789", internalReservation.RoomId);
            Assert.Equal(520.75m, internalReservation.TotalAmount);
        }

        [Fact]
        public void Map_InternalRoomToGoogle_ShouldMapCorrectly()
        {
            // Arrange
            var internalRoom = new DynamicMappingSystem.Models.Room
            {
                Id = "ROOM-001",
                RoomType = "Deluxe Suite",
                PricePerNight = 199.99m,
                MaxOccupancy = 4,
                Amenities = new List<string> { "WiFi", "TV", "Minibar" }
            };

            // Act
            var result = _mapHandler.Map(internalRoom, "Models.Room", "Google.Room");

            // Assert
            Assert.NotNull(result);
            var googleRoom = Assert.IsType<Google.Room>(result);
            Assert.Equal("ROOM-001", googleRoom.AccommodationId);
            Assert.Equal("Deluxe Suite", googleRoom.Category);
            Assert.Equal(199.99, googleRoom.NightlyRate);
            Assert.Equal(4, googleRoom.GuestCapacity);
            Assert.Equal(new[] { "WiFi", "TV", "Minibar" }, googleRoom.Features);
        }

        [Fact]
        public void Map_GoogleRoomToInternal_ShouldMapCorrectly()
        {
            // Arrange
            var googleRoom = new Google.Room
            {
                AccommodationId = "GOOG-ROOM-456",
                Category = "Premium Suite",
                NightlyRate = 299.50,
                GuestCapacity = 6,
                Features = new[] { "WiFi", "Balcony", "Kitchen", "TV" }
            };

            // Act
            var result = _mapHandler.Map(googleRoom, "Google.Room", "Models.Room");

            // Assert
            Assert.NotNull(result);
            var internalRoom = Assert.IsType<DynamicMappingSystem.Models.Room>(result);
            Assert.Equal("GOOG-ROOM-456", internalRoom.Id);
            Assert.Equal("Premium Suite", internalRoom.RoomType);
            Assert.Equal(299.50m, internalRoom.PricePerNight);
            Assert.Equal(6, internalRoom.MaxOccupancy);
            Assert.Equal(new List<string> { "WiFi", "Balcony", "Kitchen", "TV" }, internalRoom.Amenities);
        }

        [Fact]
        public void Map_BookingDotComRoomToInternal_ShouldMapCorrectly()
        {
            // Arrange
            var bookingRoom = new BookingDotCom.Room
            {
                RoomTypeId = 555,
                RoomTypeName = "Standard Double",
                MaxGuests = 2,
                BasePrice = 89.99m,
                Amenities = new List<string> { "WiFi", "TV" }
            };

            // Act
            var result = _mapHandler.Map(bookingRoom, "BookingDotCom.Room", "Models.Room");

            // Assert
            Assert.NotNull(result);
            var internalRoom = Assert.IsType<DynamicMappingSystem.Models.Room>(result);
            Assert.Equal("555", internalRoom.Id);
            Assert.Equal("Standard Double", internalRoom.RoomType);
            Assert.Equal(89.99m, internalRoom.PricePerNight);
            Assert.Equal(2, internalRoom.MaxOccupancy);
            Assert.Equal(new List<string> { "WiFi", "TV" }, internalRoom.Amenities);
        }

        [Fact]
        public void Map_InternalToBookingDotCom_ShouldMapCorrectly()
        {
            // Arrange
            var internalReservation = new DynamicMappingSystem.Models.Reservation
            {
                Id = "999",
                CheckInDate = new DateTime(2024, 9, 15),
                CheckOutDate = new DateTime(2024, 9, 18),
                GuestName = "Max Mustermann",
                NumberOfGuests = 3,
                RoomId = "333",
                TotalAmount = 420.00m
            };

            // Act
            var result = _mapHandler.Map(internalReservation, "Models.Reservation", "BookingDotCom.Booking");

            // Assert
            Assert.NotNull(result);
            var bookingReservation = Assert.IsType<BookingDotCom.Booking>(result);
            Assert.Equal(999, bookingReservation.BookingId); // Convert string to int
            Assert.Equal("2024-09-15", bookingReservation.ArrivalDate);
            Assert.Equal("2024-09-18", bookingReservation.DepartureDate);
            Assert.Equal("Max", bookingReservation.GuestDetails.FirstName); // Split name
            Assert.Equal("Mustermann", bookingReservation.GuestDetails.LastName);
            Assert.Equal(3, bookingReservation.AdultCount);
            Assert.Equal(333, bookingReservation.RoomTypeId);
            Assert.Equal(420.00m, bookingReservation.TotalPrice);
        }

        [Fact]
        public void Map_EmptyStringValues_ShouldHandleGracefully()
        {
            // Arrange
            var reservation = new DynamicMappingSystem.Models.Reservation
            {
                Id = "",
                GuestName = "",
                RoomId = "",
                CheckInDate = DateTime.MinValue,
                CheckOutDate = DateTime.MinValue,
                NumberOfGuests = 0,
                TotalAmount = 0m
            };

            // Act
            var result = _mapHandler.Map(reservation, "Models.Reservation", "Google.Reservation");

            // Assert
            Assert.NotNull(result);
            var googleReservation = Assert.IsType<Google.Reservation>(result);
            Assert.Equal("", googleReservation.ReservationId);
            Assert.Equal("", googleReservation.GuestFullName);
            Assert.Equal("", googleReservation.AccommodationId);
            Assert.Equal(0, googleReservation.PartySize);
            Assert.Equal(0.0, googleReservation.TotalCost);
        }

        [Fact]
        public void Map_EmptyAmenities_ShouldHandleCorrectly()
        {
            // Arrange
            var room = new DynamicMappingSystem.Models.Room
            {
                Id = "ROOM-EMPTY",
                RoomType = "Basic",
                PricePerNight = 50m,
                MaxOccupancy = 1,
                Amenities = new List<string>() // Empty list
            };

            // Act
            var result = _mapHandler.Map(room, "Models.Room", "Google.Room");

            // Assert
            Assert.NotNull(result);
            var googleRoom = Assert.IsType<Google.Room>(result);
            Assert.Empty(googleRoom.Features);
        }

        [Fact]
        public void Map_SingleNameToBookingDotCom_ShouldHandleGracefully()
        {
            // Arrange
            var reservation = new DynamicMappingSystem.Models.Reservation
            {
                Id = "123",
                CheckInDate = new DateTime(2024, 12, 25),
                CheckOutDate = new DateTime(2024, 12, 27),
                GuestName = "Madonna", // Single name, no space
                NumberOfGuests = 1,
                RoomId = "456",
                TotalAmount = 200m
            };

            // Act
            var result = _mapHandler.Map(reservation, "Models.Reservation", "BookingDotCom.Booking");

            // Assert
            Assert.NotNull(result);
            var bookingReservation = Assert.IsType<BookingDotCom.Booking>(result);
            Assert.Equal("Madonna", bookingReservation.GuestDetails.FirstName);
            Assert.Equal("", bookingReservation.GuestDetails.LastName); // Empty last name
        }

        [Fact]
        public void Map_InvalidDateString_BookingDotCom_ShouldThrowException()
        {
            // Arrange
            var booking = new BookingDotCom.Booking
            {
                BookingId = 123,
                ArrivalDate = "invalid-date",
                DepartureDate = "2024-12-31",
                GuestDetails = new BookingDotCom.Guest { FirstName = "Test", LastName = "User" },
                AdultCount = 1,
                RoomTypeId = 1,
                TotalPrice = 100m
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(booking, "BookingDotCom.Booking", "Models.Reservation"));
            
            Assert.Contains("Invalid date format", exception.Message);
        }

        [Fact]
        public void Map_NullGuestDetails_BookingDotCom_ShouldThrowException()
        {
            // Arrange
            var booking = new BookingDotCom.Booking
            {
                BookingId = 123,
                ArrivalDate = "2024-12-25",
                DepartureDate = "2024-12-31",
                GuestDetails = null!, // Null guest
                AdultCount = 1,
                RoomTypeId = 1,
                TotalPrice = 100m
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(booking, "BookingDotCom.Booking", "Models.Reservation"));
            
            Assert.Contains("Guest details cannot be null", exception.Message);
        }

        [Fact]
        public void Map_EmptyOrNullParameters_ShouldThrowException()
        {
            // Arrange
            var reservation = new DynamicMappingSystem.Models.Reservation();

            // Act & Assert - Empty source type
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "", "Google.Reservation"));

            // Act & Assert - Empty target type  
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "Models.Reservation", ""));

            // Act & Assert - Null source type
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, null!, "Google.Reservation"));

            // Act & Assert - Null target type
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "Models.Reservation", null!));
        }

        [Fact]
        public void Map_InvalidSourceType_ShouldThrowException()
        {
            // Arrange
            var reservation = new DynamicMappingSystem.Models.Reservation();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "Invalid.Type", "Google.Reservation"));
            
            Assert.Contains("Unsupported source type", exception.Message);
        }

        [Fact]
        public void Map_InvalidTargetType_ShouldThrowException()
        {
            // Arrange
            var reservation = new DynamicMappingSystem.Models.Reservation();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "Models.Reservation", "Invalid.Type"));
            
            Assert.Contains("Unsupported target type", exception.Message);
        }

        [Fact]
        public void Map_NullData_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                _mapHandler.Map(null!, "Models.Reservation", "Google.Reservation"));
            
            Assert.Equal("data", exception.ParamName);
        }

        [Fact]
        public void Map_DataTypeMismatch_ShouldThrowException()
        {
            // Arrange
            var wrongDataType = "This is a string, not a reservation";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(wrongDataType, "Models.Reservation", "Google.Reservation"));
            
            Assert.Contains("Data type mismatch", exception.Message);
        }
    }
}