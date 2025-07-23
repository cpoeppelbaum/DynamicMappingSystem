using DynamicMappingSystem.Core;
using Models;
using BookingDotComModels = BookingDotCom;

namespace DynamicMappingSystem.BookingDotCom.Mappers
{
    internal class FromBookingMapper : AbstractMapper<BookingDotComModels.Booking, Reservation>
    {
        protected override Reservation Map(BookingDotComModels.Booking source)
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
    }
}