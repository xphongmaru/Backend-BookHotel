using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Amenities : BaseEntity
    {
        [Key]
        public int Amenities_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Room_Amenities> Room_Amenities { get; set; } = new();
    }
}


