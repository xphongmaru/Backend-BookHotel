using BookHotel.Models;
using System.Text.Json.Serialization;


namespace BookHotel.DTOs
{
    public class RoomTypeDto
    {
        public int TypeRoom_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //lien ket
        [JsonIgnore]
        public List<Room> Rooms { get; set; } = new();
    }
}
