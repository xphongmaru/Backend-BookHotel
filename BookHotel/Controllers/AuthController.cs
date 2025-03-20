using Azure.Core;
using BookHotel.Data;
using BookHotel.DTOs;
using BookHotel.Models;
using BookHotel.Services.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public readonly AppDbContext _context;
        public readonly IConfiguration _configuration;
        public readonly IEmailService _emailService;

        public AuthController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] AuthGuessRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.Password == request.Password);
            if (guess == null)
                return BadRequest("Email hoặc mật khẩu không đúng.");

            var emailVerify = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.EmailVerify == true);
            if (emailVerify == null)
                return BadRequest("Email chưa được xác thực.");
            var role ="";
            if (guess.Role == 0) role = "admin";
            else role = "user";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, guess.Name),
                new Claim(ClaimTypes.Email, guess.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );


            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] GuessRegisterRequest request)
        {
            var _OTP = new Random().Next(100000, 999999);
            var emailVerify = _context.Guess.FirstOrDefault(g => g.Email == request.Email);
            if (emailVerify != null)
            {
                return BadRequest("Email đã tồn tại.");
            }

            var guess = new Guess
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                CCCD = request.CCCD,
                Gender = request.Gender,
                OTP = _OTP
            };
            _context.Guess.Add(guess);
            await _context.SaveChangesAsync();

            // Gửi email xác thực
            await _emailService.SendEmailAsync(guess.Email, "Xác thực tài khoản", $"{_OTP}");

            return Ok("Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.");


        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromBody] GuessVeryfyRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.OTP == request.OTP);
            if (guess == null)
                return BadRequest("OTP không đúng.");
            if(guess.EmailVerify == true)
                return BadRequest("Email đã được xác thực.");
            if(guess != null)
            {
                guess.EmailVerify = true;
                guess.OTP = 0;
                await _context.SaveChangesAsync();
                return Ok(1);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("forgot-password")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == email);
            if (guess == null)
                return BadRequest("Email không tồn tại.");
            if (guess.EmailVerify == false)
                return BadRequest("Tài khoản chưa xác thực.");

            var _OTP = new Random().Next(100000, 999999);
            guess.OTP = _OTP;
            await _context.SaveChangesAsync();

            // Gửi email xác thực
            await _emailService.SendEmailForgotPassword(guess.Email, "Quên mật khẩu", $"{_OTP}");

            return Ok("Vui lòng kiểm tra email để lấy lại mật khẩu.");
        }

        [HttpPost("check-otp")]
        public async Task<ActionResult> CheckOTP([FromBody] GuessVeryfyRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.OTP == request.OTP);
            if (guess == null)
                return BadRequest("OTP không đúng.");
            else
            {
                guess.OTP = 1;
                await _context.SaveChangesAsync();
                return Ok(1);
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] RestPasswordRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.OTP == 1);
            if (guess == null)
                return BadRequest(0);
            else
            {
                guess.Password = request.NewPassword;
                guess.OTP = 0;
                await _context.SaveChangesAsync();
                return Ok("Thay đổi mật khẩu mới thành công.");
            }


            
        }
    }
}
