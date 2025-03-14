using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookHotel.Data;
using BookHotel.DTOs;
using BookHotel.Repositories.Admin;

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAdminController : ControllerBase
    {
        private readonly IControl _control;

        public LoginAdminController(IControl control)
        {
            _control = control;
        }

        [HttpPost]
        public async Task<ActionResult<UserGetItem>> LoginAdmin([FromBody] AdminLoginRequest request)
        {
            var result= await _control.LoginAdmin.LoginRequest(request);

            if (result != null)
            {
                return Ok(result);
            }
            else { 
                return BadRequest(new { message = "Email hoặc mật khẩu không chính xác. "});
            }
        }
    }
}
