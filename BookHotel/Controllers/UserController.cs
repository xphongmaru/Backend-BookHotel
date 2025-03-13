using Microsoft.AspNetCore.Mvc;
using BookHotel.Models;
using BookHotel.Repositories.Admin;
using BookHotel.DTOs;
using BookHotel.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IControl _control;
        public readonly AppDbContext _context;

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("API is working!");
        }
        
        public UserController(IControl control, AppDbContext context)
        {
            _control = control;
            _context = context;
        } 

        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _control.User.GetUsers());
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGetItem>> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) { return NotFound(); }
            else { return await _control.User.GetUserById(id); }
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] UserCreateRequest request)
        {
            //if (!ModelState.IsValid) return BadRequest();
            var existingRole = await _context.Roles.FindAsync(request.Role_id);
            if (existingRole == null) {
                return BadRequest();
            }
            else
            {
                var result = await _control.User.CreateUser(request);
                await _control.Save();
                return Ok("Thêm mới user thành công");
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserEditRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) { return NotFound(); }

            var existingRole = await _context.Roles.FindAsync(request.Role_id);
            if (existingRole== null)
            {
                return BadRequest();
            }
            await _control.User.UpdateUser(user, request);
            await _control.Save();
            return Ok("Cập nhật thông tin user thành công.");
        }

        // DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
