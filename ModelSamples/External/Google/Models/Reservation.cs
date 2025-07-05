namespace Google
{
    public class Reservation
    {
        public string ReservationId { get; set; } = string.Empty;
        public long CheckInTimestamp { get; set; }
        public long CheckOutTimestamp { get; set; }
        public string GuestFullName { get; set; } = string.Empty;
        public int PartySize { get; set; }
        public string AccommodationId { get; set; } = string.Empty;
        public double TotalCost { get; set; }
    }
}