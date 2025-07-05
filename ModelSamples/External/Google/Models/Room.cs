namespace Google
{
    public class Room
    {
        public string AccommodationId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double NightlyRate { get; set; }
        public int GuestCapacity { get; set; }
        public string[] Features { get; set; } = Array.Empty<string>();
    }
}