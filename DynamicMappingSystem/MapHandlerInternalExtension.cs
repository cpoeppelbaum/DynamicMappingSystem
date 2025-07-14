using DynamicMappingSystem.Core;
using DynamicMappingSystem.Validators;

namespace DynamicMappingSystem
{
    public static class MapHandlerInternalExtension
    {
        /// <summary>
        /// Registers internal model validators
        /// </summary>
        public static IMapHandler AddInternalValidators(this IMapHandler mapHandler)
        {
            mapHandler.RegisterValidator(new ReservationValidator());
            mapHandler.RegisterValidator(new RoomValidator());

            return mapHandler;
        }
    }
}