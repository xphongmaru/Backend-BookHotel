using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using BookHotel.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        public readonly IControl _control;
        public readonly AppDbContext _appdbContext;

        public PermissionController(IControl control, AppDbContext appDbContext)
        {
            _control = control;
            _appdbContext = appDbContext;
        }

        // GET: api/<PermissionController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Permission>>> GetPermissons()
        {
            return Ok(await _control.Permissions.GetPermissions());
        }

        // GET api/<PermissionController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionGetRequest>> GetPermissionById(int id)
        {
            var permisson = await _control.Permissions.GetPermissionByID(id);
            if (permisson == null) { return NotFound(); }
            return permisson;
        }

        // POST api/<PermissionController>
        [HttpPost]
        public async Task<ActionResult> CreatePermison([FromBody] PermissionCreateRequest request)
        {
            await _control.Permissions.CreatePermission(request);
            await _control.Save();
            return Ok("Thêm permission mới thành công.");
        }

        // PUT api/<PermissionController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePermission(int id, [FromBody] PermissionCreateRequest request)
        {
            var permission = await _appdbContext.Permissions.FindAsync(id);
            if (permission == null) {
                return BadRequest();
            }
            await _control.Permissions.UpdatePermission(permission, request);
            await _control.Save();
            return Ok("Cập nhật permission thành công.");
        }

        // DELETE api/<PermissionController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePermission(int id)
        {
            var permison = await _control.Permissions.GetPermissionByID(id);
            if (permison == null)
            {
                return BadRequest();
            }
            await _control.Permissions.DeletePermission(id);
            await _control.Save();
            return Ok("Xóa permission thành công.");
        }
    }
}
