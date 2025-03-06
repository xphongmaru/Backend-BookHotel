using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Permission : BaseEntity
    {
        [Key]
        public int Permission_id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Role_Permission> Role_Permissions { get; set; } = new();
    }
}


