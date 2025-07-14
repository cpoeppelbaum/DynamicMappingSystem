using DynamicMappingSystem.Core;
using DynamicMappingSystem.BookingDotCom.Mappers;
using BookingDotComModels = BookingDotCom;
using DynamicMappingSystem.BookingDotCom.Validators;

namespace DynamicMappingSystem.BookingDotCom
{
    public static class MapHandlerBookingDotComExtension
    {
        /// <summary>
        /// Registers all BookingDotCom-specific mappers and validators
        /// </summary>
        public static IMapHandler AddBookingDotComMappers(this IMapHandler mapHandler)
        {
            // Register validators
            mapHandler.RegisterValidator<BookingDotComModels.Booking>(new BookingValidator());
            mapHandler.RegisterValidator<BookingDotComModels.Guest>(new GuestValidator());
            mapHandler.RegisterValidator<BookingDotComModels.Room>(new RoomValidator());

            // Register mappers
            mapHandler.RegisterMapper<BookingDotComModels.Booking, Models.Reservation>(new FromBookingMapper());
            mapHandler.RegisterMapper<Models.Reservation, BookingDotComModels.Booking>(new ToBookingMapper());
            mapHandler.RegisterMapper<BookingDotComModels.Room, Models.Room>(new FromRoomMapper());
            
            return mapHandler;
        }
    }
}