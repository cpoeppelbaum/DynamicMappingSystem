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
    }
}