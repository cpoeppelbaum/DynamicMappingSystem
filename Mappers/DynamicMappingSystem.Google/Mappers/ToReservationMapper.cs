using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class ToReservationMapper : IMapper<Reservation, GoogleModels.Reservation>
    {
        protected override GoogleModels.Reservation Map(Reservation source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
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
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Unexpected error during conversion from Models.Reservation to Google.Reservation", ex);
            }
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