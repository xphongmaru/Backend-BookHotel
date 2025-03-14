using System.ComponentModel.DataAnnotations;

namespace BookHotel.DTOs
{
    public class AuthGuessRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; } = string.Empty;
    }

    public class GuessRegisterRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên không được để trống.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "CCCD không được để trống.")]
        public string CCCD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        public bool Gender { get; set; }
        
    }
}
