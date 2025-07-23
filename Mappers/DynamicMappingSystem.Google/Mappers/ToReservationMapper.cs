using DynamicMappingSystem.Core;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class ToReservationMapper : AbstractMapper<Reservation, GoogleModels.Reservation>
    {
        protected override GoogleModels.Reservation Map(Reservation source)
        {
            return new GoogleModels.Reservation
            {
                ReservationId = source.Id ?? string.Empty,
                CheckInTimestamp = ConvertDateTimeToUnixTimestamp(source.CheckInDate),
                CheckOutTimestamp = ConvertDateTimeToUnixTimestamp(source.CheckOutDate),
                GuestFullName = source.GuestName ?? string.Empty,
                PartySize = source.NumberOfGuests,
                AccommodationId = source.RoomId ?? string.Empty,
                TotalCost = (double)source.TotalAmount
            };
        }

        private static long ConvertDateTimeToUnixTimestamp(DateTime dateTime)
        {
            // Treat DateTime as UTC for consistent conversion
            DateTime utcDateTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : 
                                 dateTime.Kind == DateTimeKind.Local ? dateTime.ToUniversalTime() :
                                 DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            return ((DateTimeOffset)utcDateTime).ToUnixTimeSeconds();
        }
    }
}