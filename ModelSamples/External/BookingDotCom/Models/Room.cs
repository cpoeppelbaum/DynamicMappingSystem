namespace BookingDotCom
{
    public class Room
    {
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public int MaxGuests { get; set; }
        public decimal BasePrice { get; set; }
        public List<string> Amenities { get; set; } = new List<string>();
    }
}