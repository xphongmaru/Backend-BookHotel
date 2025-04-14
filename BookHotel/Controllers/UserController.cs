using BookHotel.DTOs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using BookHotel.Services.Mail;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookHotel.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        public UserController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        // GET: api/<UserController>
        [Authorize]
        [HttpGet("api/admin/user")]
        public async Task<ActionResult<IEnumerable<GetAllUserRequest>>> GetUsers()
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));
            
            //return await _context.Guess.ToListAsync();
            return await _context.Guess.Select(g => new GetAllUserRequest
            {
                Guess_id = g.Guess_id,
                Name = g.Name,
                PhoneNumber = g.PhoneNumber,
                Email = g.Email,
                CCCD = g.CCCD,
                Role = g.Role,
                EmailVerify = g.EmailVerify
            }).ToListAsync();
        }
        
        //GET api/< UserController >/
        [Authorize]
        [HttpGet("api/admin/user/{id}")]
        public async Task<ActionResult<GetUserRequest>> GetUser(int id)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            var user = await _context.Guess.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse(false, null, new ErrorResponse("Không tìm thấy người dùng", 400)));

            }
            return new GetUserRequest
            {
                Guess_id = user.Guess_id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                CCCD = user.CCCD,
                Gender = user.Gender,
                Role = user.Role,
                EmailVerify = user.EmailVerify
            };
        }

        // POST api/<UserController>
        [Authorize]
        [HttpPost("api/admin/user")]
        public async Task<ActionResult> CreateUser([FromBody] GuessRegisterRequest request)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (user == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (user.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

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
                OTP = _OTP
            };
            _context.Guess.Add(guess);
            await _context.SaveChangesAsync();

            // Gửi email xác thực
            await _emailService.SendEmailAsync(guess.Email, "Xác thực tài khoản", $"{_OTP}");

            return Ok(new ApiResponse(true, "Tạo tài khoản thành công. Vui lòng kiểm tra email để xác thực tài khoản.", null));

        }

        // PUT api/<UserController>/5
        [Authorize]
        [HttpPut("api/admin/user/{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] EditUserRequest request) 
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            var user = await _context.Guess.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse(false, null, new ErrorResponse("Email đã tồn tại.", 400)));
            }
            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.CCCD = request.CCCD;
            user.Bod = DateTime.Parse(request.Bod);
            user.Gender = request.Gender;
            if(request.Role != 0 && request.Role != 1) request.Role = 1;
            user.Role = request.Role;
            
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(true, "Cập nhật thông tin thành công.", null));
        }

        [Authorize]
        [HttpPut("api/user/change-password/{id}")]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] ChangePassword request)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            if(guess.Guess_id != id)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Lỗi xác thực người dùng.", 400)));

            if(request.oldPassword != guess.Password)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mật khẩu cũ không chính xác.", 400)));
            else if(request.oldPassword == guess.Password)
            {
                guess.Password = request.newPassword;
                _context.Entry(guess).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse(true, "Đổi mật khẩu thành công.", null));
            }
            else
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mật khẩu không chính xác.", 400)));
            }  
        }

        [Authorize]
        [HttpPut("api/user/change-infor/{id}")]
        public async Task<ActionResult> ChangeInfor(int id, [FromBody] ChangeInforGuess request)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            if (guess.Guess_id != id)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Lỗi xác thực người dùng.", 400)));
            
            guess.Name = request.Name;
            guess.PhoneNumber = request.PhoneNumber;
            guess.Address = request.Address;
            guess.CCCD = request.CCCD;
            guess.Gender = request.Gender;
            guess.Bod = DateTime.Parse(request.Bod);
            _context.Entry(guess).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(true, "Cập nhật thông tin thành công.", null));
        }
    }
}
