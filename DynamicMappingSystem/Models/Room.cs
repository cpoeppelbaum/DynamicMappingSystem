namespace DynamicMappingSystem.Models
{
    public class Room
    {
        public string Id { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public List<string> Amenities { get; set; } = new List<string>();
    }
}