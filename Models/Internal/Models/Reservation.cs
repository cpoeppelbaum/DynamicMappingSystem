namespace Internal.Models
{
    public class Reservation
    {
        public string Id { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public int NumberOfGuests { get; set; }
        public string RoomId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}