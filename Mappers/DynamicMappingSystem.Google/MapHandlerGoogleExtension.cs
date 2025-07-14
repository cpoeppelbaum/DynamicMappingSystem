using DynamicMappingSystem.Core;
using DynamicMappingSystem.Google.Mappers;
using DynamicMappingSystem.Google.Validators;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google
{
    public static class MapHandlerGoogleExtension
    {
        /// <summary>
        /// Registers all Google-specific mappers and validators
        /// </summary>
        public static IMapHandler AddGoogleMappers(this IMapHandler mapHandler)
        {
            // Register validators
            mapHandler.RegisterValidator(new ReservationValidator());
            mapHandler.RegisterValidator(new RoomValidator());

            // Register mappers
            mapHandler.RegisterMapper<Models.Reservation, GoogleModels.Reservation>(new ToReservationMapper());
            mapHandler.RegisterMapper<GoogleModels.Reservation, Models.Reservation>(new FromReservationMapper());
            mapHandler.RegisterMapper<Models.Room, GoogleModels.Room>(new ToRoomMapper());
            mapHandler.RegisterMapper<GoogleModels.Room, Models.Room>(new FromRoomMapper());
            
            return mapHandler;
        }
    }
}