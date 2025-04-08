using BookHotel.DTOs;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using BookHotel.Services.Mail;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllUserRequest>>> GetUsers()
        {
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
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserRequest>> GetUser(int id)
        {
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
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] GuessRegisterRequest request)
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
                OTP = _OTP
            };
            _context.Guess.Add(guess);
            await _context.SaveChangesAsync();

            // Gửi email xác thực
            await _emailService.SendEmailAsync(guess.Email, "Xác thực tài khoản", $"{_OTP}");

            return Ok(new ApiResponse(true, "Tạo tài khoản thành công. Vui lòng kiểm tra email để xác thực tài khoản.", null));

        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] EditUserRequest request) 
        {
            var user = await _context.Guess.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse(false, null, new ErrorResponse("Email đã tồn tại.", 400)));
            }
            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Address = request.Address;
            user.CCCD = request.CCCD;
            user.Gender = request.Gender;
            if(request.Role != 0 && request.Role != 1) request.Role = 1;
            user.Role = request.Role;
            
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(true, "Cập nhật thông tin thành công.", null));
        }
    }
}
