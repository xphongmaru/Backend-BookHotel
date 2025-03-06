using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookHotel.Data;
using BookHotel.Models;
using Microsoft.EntityFrameworkCore;
using BookHotel.Repositories.Admin;

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IControl _control;

        public RoleController(IControl control)
        {
            _control = control;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRole()
        {
            return Ok(await _control.Roles.GetRoles());
        }

        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole(Role role)
        {
            var result = await _control.Roles.CreateRole(role);
            await _control.Save();
            return Ok("tc");
        }
    }
}
