using DynamicMappingSystem;
using DynamicMappingSystem.BookingDotCom;
using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using DynamicMappingSystem.Google;
using Microsoft.Extensions.DependencyInjection;

namespace DynamicMappingSystemTest
{
    public class MapHandlerTests
    {
        private readonly IMapHandler _mapHandler;

        public MapHandlerTests()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IMapHandler, MapHandler>();
            services.AddGoogleMappers();
            services.AddBookingDotComMappers();
            var serviceProvider = services.BuildServiceProvider();

            _mapHandler = new MapHandler(serviceProvider);
        }

        [Fact]
        public void Map_EmptyOrNullParameters_ShouldThrowException()
        {
            // Arrange
            var reservation = new Models.Reservation();

            // Act & Assert - Empty source type
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "", "Any.Destination"));

            // Act & Assert - Empty target type  
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "Any.Source", ""));

            // Act & Assert - Null source type
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, null!, "Any.Destination"));

            // Act & Assert - Null target type
            Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "Any.Source", null!));
        }

        [Fact]
        public void Map_InvalidSourceType_ShouldThrowException()
        {
            // Arrange
            var reservation = new Models.Reservation();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                _mapHandler.Map(reservation, "Invalid.Type", "Google.Reservation"));
            
            Assert.Contains("Unsupported source type", exception.Message);
        }

        [Fact]
        public void Map_InvalidTargetType_ShouldThrowException()
        {
            // Arrange
            var reservation = new Models.Reservation();

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
            var exception = Assert.Throws<SourceDataTypeMismatchException>(() =>
                _mapHandler.Map(wrongDataType, "Models.Reservation", "Google.Reservation"));
            
            Assert.Contains("Data type mismatch", exception.Message);
        }

        [Fact]
        public void Map_BookingDotComToInternal_ShouldMapCorrectly()
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
            var internalReservation = Assert.IsType<Models.Reservation>(result);
            Assert.Equal("12345", internalReservation.Id);
            Assert.Equal(new DateTime(2024, 8, 20), internalReservation.CheckInDate);
            Assert.Equal(new DateTime(2024, 8, 23), internalReservation.CheckOutDate);
            Assert.Equal("Alice Johnson", internalReservation.GuestName);
            Assert.Equal(2, internalReservation.NumberOfGuests);
            Assert.Equal("789", internalReservation.RoomId);
            Assert.Equal(520.75m, internalReservation.TotalAmount);
        }

        [Fact]
        public void Map_InternalToGoogle_ShouldMapCorrectly()
        {
            // Arrange
            var internalReservation = new Models.Reservation
            {
                Id = "INT-123",
                CheckInDate = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                CheckOutDate = new DateTime(2024, 6, 18, 0, 0, 0, DateTimeKind.Utc),
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
        }
    }
}