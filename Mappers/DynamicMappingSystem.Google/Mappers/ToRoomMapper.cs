using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class ToRoomMapper : IMapper<Room, GoogleModels.Room>
    {
        public override GoogleModels.Room Map(Room source)
        {
            if (source == null) 
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                return new GoogleModels.Room
                {
                    AccommodationId = source.Id ?? string.Empty,
                    Category = source.RoomType ?? string.Empty,
                    NightlyRate = (double)source.PricePerNight,
                    GuestCapacity = source.MaxOccupancy,
                    Features = source.Amenities?.ToArray() ?? Array.Empty<string>()
                };
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Unexpected error during conversion from Models.Room to Google.Room", ex);
            }
        }
    }
}