using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class Permission : BaseEntity
    {
        [Key]
        public int Permission_id { get; set; }

        [Required(ErrorMessage = "Tên role không được để trống.")]
        [MaxLength(255, ErrorMessage ="Trường tên đã vượt quá giới hạn 255 ký tự.")]
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Role_Permission> Role_Permissions { get; set; } = new();
    }
}


