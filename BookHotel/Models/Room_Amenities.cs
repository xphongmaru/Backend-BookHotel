using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Room_Amenities:BaseEntity
    {
        public int  Room_id{ get; set; }
        [JsonIgnore]
        public Room Room { get; set; }
        public int Amenities_id { get; set; }
        [JsonIgnore]
        public Amenities Amenities { get; set; }
    }
}


