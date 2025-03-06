using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Role:BaseEntity
    {
        [Key]
        public int Role_id { get; set; }
        [Required(ErrorMessage ="Tên role không được để trống.")]
        [MaxLength(100, ErrorMessage ="Tên không dài hơn 100.")]
        public string Name { get; set; } = string.Empty;

        // Quan hệ 1-N: Một Role có nhiều Users
        [JsonIgnore]
        public List<User> Users { get; set; } = new();
        [JsonIgnore]
        public List<Role_Permission> Role_Permissions { get; set; } = new();
    }
}