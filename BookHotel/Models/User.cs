using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class User : BaseEntity
    {
        [Key]
        public int User_id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [ForeignKey("Role")]
        public int Role_id { get; set; }
        [JsonIgnore]
        public Role Role { get; set; } = new();
    }
}