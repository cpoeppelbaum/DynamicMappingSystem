using DynamicMappingSystem.Core;
using Models;
using BookingDotComModels = BookingDotCom;

namespace DynamicMappingSystem.BookingDotCom.Mappers
{
    internal class FromRoomMapper : AbstractMapper<BookingDotComModels.Room, Room>
    {
        protected override Room Map(BookingDotComModels.Room source)
        {
            return new Room
            {
                Id = source.RoomTypeId.ToString(),
                RoomType = source.RoomTypeName ?? string.Empty,
                PricePerNight = source.BasePrice,
                MaxOccupancy = source.MaxGuests,
                Amenities = source.Amenities?.ToList() ?? new List<string>()
            };
        }
    }
}