using DynamicMappingSystem.Core;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class FromRoomMapper : AbstractMapper<GoogleModels.Room, Room>
    {
        protected override Room Map(GoogleModels.Room source)
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
    }
}