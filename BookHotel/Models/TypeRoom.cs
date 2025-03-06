using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class TypeRoom : BaseEntity
    {
        [Key]
        public int TypeRoom_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        //lien ket
        [JsonIgnore]
        public List<Room> Rooms { get; set; } = new();
    }
}


