namespace BookingDotCom
{
    public class Booking
    {
        public int BookingId { get; set; }
        public string ArrivalDate { get; set; } = string.Empty; // ISO date string
        public string DepartureDate { get; set; } = string.Empty; // ISO date string
        public Guest GuestDetails { get; set; } = new Guest();
        public int AdultCount { get; set; }
        public int RoomTypeId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}