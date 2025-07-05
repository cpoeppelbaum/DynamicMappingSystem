using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using Models;
using BookingDotComModels = BookingDotCom;

namespace DynamicMappingSystem.BookingDotCom.Mappers
{
    internal class FromBookingMapper : IMapper<BookingDotComModels.Booking, Reservation>
    {
        public Reservation Convert(BookingDotComModels.Booking source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                return new Reservation
                {
                    Id = source.BookingId.ToString(),
                    CheckInDate = DateTime.Parse(source.ArrivalDate),
                    CheckOutDate = DateTime.Parse(source.DepartureDate),
                    GuestName = $"{source.GuestDetails?.FirstName?.Trim()} {source.GuestDetails?.LastName?.Trim()}".Trim(),
                    NumberOfGuests = source.AdultCount,
                    RoomId = source.RoomTypeId.ToString(),
                    TotalAmount = source.TotalPrice
                };
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Unexpected error during conversion from BookingDotCom.Booking to Models.Reservation", ex);
            }
        }
    }
}