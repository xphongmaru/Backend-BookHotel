using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Role_Permission :BaseEntity
    {
        public int Role_id { get; set; }
        [JsonIgnore]
        public Role Role { get; set; } = new();
        public int Permision_id { get; set; }
        [JsonIgnore]
        public Permission Permission { get; set; } = new();
    }

}

