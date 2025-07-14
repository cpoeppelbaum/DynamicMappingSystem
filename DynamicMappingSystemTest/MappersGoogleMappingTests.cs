using DynamicMappingSystem.Core;
using DynamicMappingSystem.Google;

namespace DynamicMappingSystemTest
{
    public class MapHandlersGoogleTests
    {
        private readonly IMapper<Models.Reservation, Google.Reservation> _toGoogleReservationMapper;
        private readonly IMapper<Google.Reservation, Models.Reservation> _fromGoogleReservationMapper;
        private readonly IMapper<Models.Room, Google.Room> _toGoogleRoomMapper;
        private readonly IMapper<Google.Room, Models.Room> _fromGoogleRoomMapper;

        public MapHandlersGoogleTests()
        {
            var mapHandlerStub = new MapHandlerStub();
            mapHandlerStub.AddGoogleMappers();

            _toGoogleReservationMapper = mapHandlerStub.GetMapper<Models.Reservation, Google.Reservation>();
            _fromGoogleReservationMapper = mapHandlerStub.GetMapper <Google.Reservation, Models.Reservation>();
            _toGoogleRoomMapper = mapHandlerStub.GetMapper <Models.Room, Google.Room>();
            _fromGoogleRoomMapper = mapHandlerStub.GetMapper <Google.Room, Models.Room>();

            // Assert that all three mappers have been registered
            mapHandlerStub.ValidateAllMappersHaveBeenAskedFor();
            Assert.NotNull(_toGoogleReservationMapper);
            Assert.NotNull(_fromGoogleReservationMapper);
            Assert.NotNull(_toGoogleRoomMapper);
            Assert.NotNull(_fromGoogleRoomMapper);
        }

        [Fact]
        public void Map_InternalReservationToGoogle_ShouldMapCorrectly()
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
            var result = _toGoogleReservationMapper.Map(internalReservation);

            // Assert
            Assert.NotNull(result);
            var googleReservation = Assert.IsType<Google.Reservation>(result);
            Assert.Equal("INT-123", googleReservation.ReservationId);
            Assert.Equal("John Doe", googleReservation.GuestFullName);
            Assert.Equal(2, googleReservation.PartySize);
            Assert.Equal("ROOM-456", googleReservation.AccommodationId);
            Assert.Equal(450.00, googleReservation.TotalCost);

            // Check timestamp conversion (DateTime to Unix timestamp) - using UTC
            var expectedCheckIn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();
            var expectedCheckOut = new DateTimeOffset(2024, 6, 18, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds();
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
                CheckInTimestamp = new DateTimeOffset(2024, 7, 10, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                CheckOutTimestamp = new DateTimeOffset(2024, 7, 13, 0, 0, 0, TimeSpan.Zero).ToUnixTimeSeconds(),
                GuestFullName = "Jane Smith",
                PartySize = 3,
                AccommodationId = "GOOG-ROOM-123",
                TotalCost = 675.50
            };

            // Act
            var result = _fromGoogleReservationMapper.Map(googleReservation);

            // Assert
            Assert.NotNull(result);
            var internalReservation = Assert.IsType<Models.Reservation>(result);
            Assert.Equal("GOOG-789", internalReservation.Id);
            Assert.Equal(new DateTime(2024, 7, 10, 0, 0, 0, DateTimeKind.Utc), internalReservation.CheckInDate);
            Assert.Equal(new DateTime(2024, 7, 13, 0, 0, 0, DateTimeKind.Utc), internalReservation.CheckOutDate);
            Assert.Equal("Jane Smith", internalReservation.GuestName);
            Assert.Equal(3, internalReservation.NumberOfGuests);
            Assert.Equal("GOOG-ROOM-123", internalReservation.RoomId);
            Assert.Equal(675.50m, internalReservation.TotalAmount);
        }

        [Fact]
        public void Map_InternalRoomToGoogle_ShouldMapCorrectly()
        {
            // Arrange
            var internalRoom = new Models.Room
            {
                Id = "ROOM-001",
                RoomType = "Deluxe Suite",
                PricePerNight = 199.99m,
                MaxOccupancy = 4,
                Amenities = new List<string> { "WiFi", "TV", "Minibar" }
            };

            // Act
            var result = _toGoogleRoomMapper.Map(internalRoom);

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
            var result = _fromGoogleRoomMapper.Map(googleRoom);

            // Assert
            Assert.NotNull(result);
            var internalRoom = Assert.IsType<Models.Room>(result);
            Assert.Equal("GOOG-ROOM-456", internalRoom.Id);
            Assert.Equal("Premium Suite", internalRoom.RoomType);
            Assert.Equal(299.50m, internalRoom.PricePerNight);
            Assert.Equal(6, internalRoom.MaxOccupancy);
            Assert.Equal(new List<string> { "WiFi", "Balcony", "Kitchen", "TV" }, internalRoom.Amenities);
        }
    }
}