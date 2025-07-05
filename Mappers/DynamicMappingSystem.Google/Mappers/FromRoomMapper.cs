using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class FromRoomMapper : IMapper<GoogleModels.Room, Room>
    {
        public Room Convert(GoogleModels.Room source)
        {
            if (source == null) 
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                return new Room
                {
                    Id = source.AccommodationId ?? string.Empty,
                    RoomType = source.Category ?? string.Empty,
                    PricePerNight = (decimal)source.NightlyRate,
                    MaxOccupancy = source.GuestCapacity,
                    Amenities = source.Features?.ToList() ?? new List<string>()
                };
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Unexpected error during conversion from Google.Room to Models.Room", ex);
            }
        }
    }
}