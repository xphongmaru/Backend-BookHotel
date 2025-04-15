using Microsoft.AspNetCore.Mvc;
using BookHotel.DTOs;
using BookHotel.Models;
using BookHotel.Repositories.Admin;
using BookHotel.Responsee;
using BookHotel.Exceptions;
using AutoMapper;

namespace BookHotel.Controllers
{
    [ApiController]
    [Route("api/type-room")]
    public class TypeRoomController : ControllerBase
    {
        private readonly ITypeRoomRepository _repository;
        private readonly IMapper _mapper;

        public TypeRoomController(ITypeRoomRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _repository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<TypeRoomDto>>(data);
            return Ok(new BaseResponse<IEnumerable<TypeRoomDto>>(result));
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null)
                return NotFound(new BaseResponse<string>("Không tìm thấy kiểu phòng", 404));

            var result = _mapper.Map<TypeRoomDto>(data);
            return Ok(new BaseResponse<TypeRoomDto>(result));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] TypeRoomCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new BaseResponse<string>("Dữ liệu không hợp lệ", 400));

            try
            {
                var typeRoom = _mapper.Map<TypeRoom>(dto);
                var created = await _repository.CreateAsync(typeRoom);
                var result = _mapper.Map<TypeRoomDto>(created);
                return StatusCode(201, new BaseResponse<string>("Tạo kiểu phòng thành công"));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new BaseResponse<string>(ex.Message, 400));
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TypeRoomCreateDto dto)
        {
            try
            {
                var typeRoom = _mapper.Map<TypeRoom>(dto);
                var success = await _repository.UpdateAsync(id, typeRoom);
                if (!success)
                    return NotFound(new BaseResponse<string>("Không tìm thấy loại phòng", 404));

                return Ok(new BaseResponse<string>("Cập nhật thành công"));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new BaseResponse<string>(ex.Message, 400));
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _repository.DeleteAsync(id);
                if (!success)
                    return NotFound(new BaseResponse<string>("Không tìm thấy loại phòng", 404));

                return Ok(new BaseResponse<string>("Xóa thành công"));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new BaseResponse<string>(ex.Message, 400));
            }
        }
    }
}
