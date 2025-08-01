using DynamicMappingSystem.Core;
using Models;
using BookingDotComModels = BookingDotCom;

namespace DynamicMappingSystem.BookingDotCom.Mappers
{
    internal class ToBookingMapper : AbstractMapper<Reservation, BookingDotComModels.Booking>
    {
        protected override BookingDotComModels.Booking Map(Reservation source)
        {
            // Split guest name (simple logic)
            var nameParts = source.GuestName?.Split(' ', 2) ?? Array.Empty<string>();
            var firstName = nameParts.Length > 0 ? nameParts[0] : string.Empty;
            var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                
            return new BookingDotComModels.Booking
            {
                BookingId = int.TryParse(source.Id, out var bookingId) ? bookingId : 0,
                ArrivalDate = source.CheckInDate.ToString("yyyy-MM-dd"),
                DepartureDate = source.CheckOutDate.ToString("yyyy-MM-dd"),
                GuestDetails = new BookingDotComModels.Guest
                {
                    FirstName = firstName,
                    LastName = lastName,
                    EmailAddress = string.Empty // No email data in Internal.Reservation
                },
                AdultCount = source.NumberOfGuests,
                RoomTypeId = int.TryParse(source.RoomId, out var roomId) ? roomId : 0,
                TotalPrice = source.TotalAmount
            };
        }
    }
}