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
    public class AuthGuessController : ControllerBase
    {
        public readonly AppDbContext _context;
        public readonly IConfiguration _configuration;
        public readonly IEmailService _emailService;

        public AuthGuessController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("loign")]
        public async Task<ActionResult> Login([FromBody] AuthGuessRequest request)
        {
            var guess = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.Password == request.Password);
            if(guess == null)
                return BadRequest("Email hoặc mật khẩu không đúng.");

            var emailVerify = _context.Guess.FirstOrDefault(g => g.Email == request.Email && g.EmailVerify == true);
            if(emailVerify == null)
                return BadRequest("Email chưa được xác thực.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, guess.Name),
                new Claim(ClaimTypes.Email, guess.Email),
            };

            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );


            return Ok(new {token = new JwtSecurityTokenHandler().WriteToken(token)});
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] GuessRegisterRequest request)
        {
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
                Gender = request.Gender
            };
            _context.Guess.Add(guess);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, guess.Name),
                new Claim(ClaimTypes.Email, guess.Email),
            };

            var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var verificationLink = Url.Action(nameof(VerifyEmail), "AuthGuess", new { token = tokenString }, Request.Scheme);

            // Gửi email xác thực
            await _emailService.SendEmailAsync(guess.Email, "Xác thực tài khoản", $"{verificationLink}");

            return Ok("Đăng ký thành công. Vui lòng kiểm tra email để xác thực tài khoản.");


        }

        [HttpGet("verify-email")]
        public async Task<ActionResult> VerifyEmail(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                }, out SecurityToken validatedToken);

                var emailClaim = principal.FindFirst(ClaimTypes.Email);
                if (emailClaim == null)
                {
                    return BadRequest("Token không hợp lệ.");
                }

                var guess = _context.Guess.FirstOrDefault(g => g.Email == emailClaim.Value);
                if (guess == null)
                {
                    return BadRequest("Người dùng không tồn tại.");
                }

                //Xác thực email thành công
                guess.EmailVerify = true;
                await _context.SaveChangesAsync();

                return Ok(1);
            }
            catch
            {
                return BadRequest(0);
            }
        }
    }
}
