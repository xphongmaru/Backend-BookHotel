using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BookHotel.Models
{
    public class User : BaseEntity
    {
        [Key]
        public int User_id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên đăng nhập không dài hơn 100.")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MaxLength(20, ErrorMessage = "Mật khẩu không dài hơn 20 ký tự.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên user không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên user không dài hơn 100 ký tự.")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email role không được để trống.")]
        [MaxLength(100, ErrorMessage = "Email không dài hơn 100 ký tự.")]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        [ForeignKey("Role")]
        public int Role_id { get; set; }
        [JsonIgnore]
        public Role Role { get; set; }
    }
}