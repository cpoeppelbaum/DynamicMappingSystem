using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using DynamicMappingSystem.BookingDotCom;

namespace DynamicMappingSystemTest
{
    public class MapHandlersBookingDotComTests
    {
        private readonly IMapper<BookingDotCom.Booking, Models.Reservation> _fromBookingMapper;
        private readonly IMapper<BookingDotCom.Room, Models.Room> _fromRoomMapper;
        private readonly IMapper<Models.Reservation, BookingDotCom.Booking> _toBookingMapper;

        public MapHandlersBookingDotComTests()
        {
            var mapHandlerStub = new MapHandlerStub();
            mapHandlerStub.AddBookingDotComMappers();

            _fromBookingMapper = mapHandlerStub.GetMapper<BookingDotCom.Booking, Models.Reservation>();
            _fromRoomMapper = mapHandlerStub.GetMapper<BookingDotCom.Room, Models.Room>();
            _toBookingMapper = mapHandlerStub.GetMapper<Models.Reservation, BookingDotCom.Booking>();

            // Assert that all three mappers have been registered
            mapHandlerStub.ValidateAllMappersHaveBeenAskedFor();
            Assert.NotNull(_fromBookingMapper);
            Assert.NotNull(_fromRoomMapper);
            Assert.NotNull(_toBookingMapper);
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
            var result = _fromBookingMapper.Map(bookingDotComBooking);

            // Assert
            Assert.NotNull(result);
            var internalReservation = Assert.IsType<Models.Reservation>(result);
            Assert.Equal("12345", internalReservation.Id);
            Assert.Equal(new DateTime(2024, 8, 20), internalReservation.CheckInDate);
            Assert.Equal(new DateTime(2024, 8, 23), internalReservation.CheckOutDate);
            Assert.Equal("Alice Johnson", internalReservation.GuestName); // Composed from FirstName + LastName
            Assert.Equal(2, internalReservation.NumberOfGuests);
            Assert.Equal("789", internalReservation.RoomId);
            Assert.Equal(520.75m, internalReservation.TotalAmount);
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
            var result = _fromRoomMapper.Map(bookingRoom);

            // Assert
            Assert.NotNull(result);
            var internalRoom = Assert.IsType<Models.Room>(result);
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
            var internalReservation = new Models.Reservation
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
            var result = _toBookingMapper.Map(internalReservation);

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
            var exception = Assert.Throws<MappingException>(() =>
                _fromBookingMapper.Map(booking));
        }
    }
}