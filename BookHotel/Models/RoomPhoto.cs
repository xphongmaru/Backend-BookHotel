using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class RoomPhoto : BaseEntity
    {
        [Key]
        public int RoomPhoto_id { get; set; }
        public string Image_url { get; set; } = string.Empty;

        [ForeignKey("Room")]
        public int Room_id { get; set; }

        [JsonIgnore]
        public Room Room { get; set; }
    }

}

