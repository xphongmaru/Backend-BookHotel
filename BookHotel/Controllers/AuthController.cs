using Azure.Core;
using BookHotel.Data;
using BookHotel.DTOs;
using BookHotel.Models;
using BookHotel.Services.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BookHotel.Controllers
{
    [Route("api/auth")]
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
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Email hoặc mật khẩu không đúng.", 400)));

            var emailVerify = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.EmailVerify == true);
            if (emailVerify == null)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Email chưa được xác thực.", 400)));

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
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new ApiResponse(true, new { token = new JwtSecurityTokenHandler().WriteToken(token) }, null));
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] GuessRegisterRequest request)
        {
            var _OTP = new Random().Next(100000, 999999);
            var emailVerify = _context.Guess.FirstOrDefault(g => g.Email == request.Email);
            if (emailVerify != null)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Email đã tồn tại.", 400)));
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
                OTP = _OTP,
                Thumbnail = "default-user.png",
            };
            _context.Guess.Add(guess);
            await _context.SaveChangesAsync();

            // Gửi email xác thực
            await _emailService.SendEmailAsync(guess.Email, "Xác thực tài khoản", $"{_OTP}");

            return Ok(new ApiResponse(true, "Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.", null));
        }

        [HttpPost("verify-email")]
        public async Task<ActionResult> VerifyEmail([FromBody] GuessVeryfyRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.OTP == request.OTP);

            if (guess == null)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("OTP không đúng.", 400)));
            }

            if (guess.EmailVerify)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Email đã được xác thực.", 400)));
            }

            // Xác thực email thành công
            guess.EmailVerify = true;
            guess.OTP = 0;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse(true, "Xác thực email thành công.", null));
        }


        [HttpGet("forgot-password")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == email);
            if (guess == null)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Email không tồn tại.", 400)));

            if (guess.EmailVerify == false)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Tài khoản chưa xác thực.", 400)));

            var _OTP = new Random().Next(100000, 999999);
            guess.OTP = _OTP;
            await _context.SaveChangesAsync();

            // Gửi email xác thực
            await _emailService.SendEmailForgotPassword(guess.Email, "Quên mật khẩu", $"{_OTP}");

            return Ok(new ApiResponse(true, "Vui lòng kiểm tra email để lấy lại mật khẩu.", null));
        }

        [HttpPost("check-otp")]
        public async Task<ActionResult> CheckOTP([FromBody] GuessVeryfyRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.OTP == request.OTP);
            if (guess == null)
            //return BadRequest("OTP không đúng.");
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("OTP không đúng.",400)));
            }    
            else
            {
                guess.OTP = 1;
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse(true, 1, null));
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] RestPasswordRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.OTP == 1);
            if (guess == null)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Thay đổi mật khẩu mới không thành công.", 400)));
            }    
            else
            {
                guess.Password = request.NewPassword;
                guess.OTP = 0;
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse(true, "Thay đổi mật khẩu mới thành công.", null));
            }
            
        }

        //GET api/< UserController >/
        [Authorize]
        [HttpGet("api/user/{id}")]
        public async Task<ActionResult<GetUser>> GetUser(int id)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            
            if (guess.Guess_id != id)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            var user = await _context.Guess.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse(false, null, new ErrorResponse("Không tìm thấy người dùng", 400)));

            }
            return new GetUser
            {
                Guess_id = user.Guess_id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                CCCD = user.CCCD,
                Gender = user.Gender,
                Thumbnail = $"{Request.Scheme}://{Request.Host}/uploads/users/{user.Thumbnail}",
                Bod = user.Bod.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
            };
        }
    }
}
