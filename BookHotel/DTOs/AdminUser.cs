using BookHotel.Models;
using System.ComponentModel.DataAnnotations;

namespace BookHotel.DTOs
{
    public class RoleCreateRequest
    {
        [Required(ErrorMessage = "Tên role không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên role không dài hơn 100 ký tự.")]
        public string Name { get; set; } = string.Empty;
        public List<int> PermissonIds { get; set; } = new List<int>();
    }

    public class RoleGetItem
    {
        public int Role_id{ get; set; }
        public string Name{ get; set;} = string.Empty;
        public List<int> PermissonIds { get; set; }
    }

    public class PermissionCreateRequest
    {
        [Required(ErrorMessage = "Tên chức năng không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên chức năng không dài hơn 100 ký tự.")]
        public string Name { get; set; } = string.Empty;
    }
    public class PermissionGetRequest
    {
        public int Permission_id{ get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class UserCreateRequest
    {
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
        public int Role_id { get; set; }
    }

    public class UserEditRequest
    {
        [Required(ErrorMessage = "Tên user không được để trống.")]
        [MaxLength(100, ErrorMessage = "Tên user không dài hơn 100 ký tự.")]
        public string Fullname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email role không được để trống.")]
        [MaxLength(100, ErrorMessage = "Email không dài hơn 100 ký tự.")]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Role_id { get; set; }
    }

    public class UserGetItem
    {
        public int User_id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Role_id { get; set; }
    } 
}
