using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using Models;
using BookingDotComModels = BookingDotCom;

namespace DynamicMappingSystem.BookingDotCom.Mappers
{
    internal class FromRoomMapper : AbstractMapper<BookingDotComModels.Room, Room>
    {
        protected override Room Map(BookingDotComModels.Room source)
        {
            if (source == null) 
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
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
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Unexpected error during mapping from BookingDotCom.Room to Models.Room", ex);
            }
        }
    }
}