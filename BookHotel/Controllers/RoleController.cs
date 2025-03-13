using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookHotel.Data;
using BookHotel.Models;
using Microsoft.EntityFrameworkCore;
using BookHotel.Repositories.Admin;
using System.Security.AccessControl;
using BookHotel.DTOs;

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IControl _control;
        private readonly AppDbContext _appDbContext;

        public RoleController(IControl control, AppDbContext appDbContext)
        {
            _control = control;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return Ok(await _control.Roles.GetRoles());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleGetItem>> GetItemRole(int id)
        {
            var role = await _control.Roles.GetRole(id);
            if (role == null) { return BadRequest(); }
            else return role;
        }

        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] RoleCreateRequest request)
        {
            var existingPermissions = await _appDbContext.Permissions
                .Where(p => request.PermissonIds.Contains(p.Permission_id))
                .Select(p => p.Permission_id)
                .ToListAsync();

            var invalidPermissions = request.PermissonIds.Except(existingPermissions).ToList();

            if (invalidPermissions.Any())
            {
                return BadRequest();
            }
            else
            {
                var result = await _control.Roles.CreateRole(request);
                await _control.Save();
                return Ok("Thêm role thành công.");
            }

            
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(int id, [FromBody] RoleCreateRequest request)
        {
            var role = await _appDbContext.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound("Role không tồn tại.");
            }

            var existingPermission = await _appDbContext.Permissions
                .Where(p => request.PermissonIds.Contains(p.Permission_id))
                .Select(p => p.Permission_id)
                .ToListAsync();
            var validatePermission = request.PermissonIds.Except(existingPermission).ToList();
            if (validatePermission.Any())
            {
                return BadRequest();
            }

            await _control.Roles.UpdateRole(role, request);
            await _control.Save();
            return Ok("Cập nhật thông tin thành công.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            var role= await _appDbContext.Roles.FindAsync(id);
            if (role == null) { return BadRequest(); }
            else
            {
                await _control.Roles.DeleteRole(id);
                await _control.Save();
                return Ok("Xóa role thành công");
            }
            
        }
    }
}
