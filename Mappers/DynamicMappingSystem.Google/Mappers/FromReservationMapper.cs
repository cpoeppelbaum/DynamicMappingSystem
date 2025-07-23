using DynamicMappingSystem.Core;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class FromReservationMapper : AbstractMapper<GoogleModels.Reservation, Reservation>
    {
        protected override Reservation Map(GoogleModels.Reservation source)
        {
            return new Reservation
            {
                Id = source.ReservationId ?? string.Empty,
                CheckInDate = ConvertUnixTimestampToDateTime(source.CheckInTimestamp),
                CheckOutDate = ConvertUnixTimestampToDateTime(source.CheckOutTimestamp),
                GuestName = source.GuestFullName ?? string.Empty,
                NumberOfGuests = source.PartySize,
                RoomId = source.AccommodationId ?? string.Empty,
                TotalAmount = (decimal)source.TotalCost
            };
        }

        private static DateTime ConvertUnixTimestampToDateTime(long timestamp)
        {
            // Return UTC time for consistent timezone handling
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }
    }
}