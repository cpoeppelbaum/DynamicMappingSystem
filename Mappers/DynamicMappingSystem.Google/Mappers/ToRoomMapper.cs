using DynamicMappingSystem.Core;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class ToRoomMapper : AbstractMapper<Room, GoogleModels.Room>
    {
        protected override GoogleModels.Room Map(Room source)
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
    }
}