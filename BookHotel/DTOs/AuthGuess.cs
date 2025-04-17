using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BookHotel.DTOs;

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

    public class AuthGuessRespone
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public GetUser User { get; set; } = new GetUser();
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

        [Required(ErrorMessage = "Giới tính không được để trống.")]
        public bool Gender { get; set; }

    }

    public class GuessVeryfyRequest
    {
        public string Email { get; set; }
        public int OTP { get; set; }
    }
    public class RestPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }

    public class GetUserRequest
    {
        public int Guess_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public bool Gender { get; set; }
        public int Role { get; set; } = 0;
        public bool EmailVerify { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Bod { get; set; } = string.Empty;
    }
    public class GetUser
    {
        public int Guess_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public bool Gender { get; set; }
        public int Role { get; set; } = 0;
        public bool EmailVerify { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string Bod { get; set; } = string.Empty;
    }

    public class GetAllUserRequest
    {
        public int Guess_id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CCCD { get; set; } = string.Empty;
        public int Role { get; set; } = 0;
        public bool EmailVerify { get; set; }

    }

    public class EditUserRequest()
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        public string Bod { get; set; }

        [Required(ErrorMessage = "CCCD không được để trống.")]
        public string CCCD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giới tính không được để trống.")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Role không được để trống.")]
        public int Role { get; set; }
    }


    public class ChangePassword()
    {
        [Required(ErrorMessage = "Mật khẩu cũ không được để trống.")]
        public string oldPassword { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        public string newPassword { get; set; }
    }

    public class ChangeInforGuess()
    {
        [Required(ErrorMessage = "Tên không được để trống.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        public string Bod { get; set; } 

        [Required(ErrorMessage = "CCCD không được để trống.")]
        public string CCCD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giới tính không được để trống.")]
        public bool Gender { get; set; }

        public IFormFile? Thumbnail { get; set; } = null;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
