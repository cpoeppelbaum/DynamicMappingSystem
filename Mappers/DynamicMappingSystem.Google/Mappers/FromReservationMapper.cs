using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using Models;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google.Mappers
{
    internal class FromReservationMapper : IMapper<GoogleModels.Reservation, Reservation>
    {
        public Reservation Convert(GoogleModels.Reservation source)
        {
            if (source == null) 
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
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
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Unexpected error during conversion from Google.Reservation to Models.Reservation", ex);
            }
        }

        private static DateTime ConvertUnixTimestampToDateTime(long timestamp)
        {
            // Return UTC time for consistent timezone handling
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }
    }
}