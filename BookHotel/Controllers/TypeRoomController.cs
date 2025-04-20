using Microsoft.AspNetCore.Mvc;
using BookHotel.DTOs;
using BookHotel.Models;
using BookHotel.Repositories.Admin;
using BookHotel.Responsee;
using BookHotel.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestDto pagingDto)
        {
            var data = await _repository.GetAllAsync(pagingDto.PageNumber, pagingDto.PageSize);
            var totalItems = await _repository.CountAllAsync();

            var result = _mapper.Map<IEnumerable<TypeRoomDto>>(data);

            return Ok(new BaseResponse<object>(new
            {
                Items = result,
                TotalItems = totalItems,
                PageNumber = pagingDto.PageNumber,
                PageSize = pagingDto.PageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pagingDto.PageSize)
            }));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/get-by-id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (id <= 0)
                return BadRequest(new BaseResponse<string>("ID phải là số nguyên >0!", 400));
            if (data == null)
                return NotFound(new BaseResponse<string>("Không tìm thấy kiểu phòng", 404));

            var result = _mapper.Map<TypeRoomDto>(data);
            return Ok(new BaseResponse<TypeRoomDto>(result));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("admin/create")]
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

        [Authorize(Roles = "admin")]
        [HttpPut("admin/update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TypeRoomCreateDto dto)
        {
            try
            {
                var typeRoom = _mapper.Map<TypeRoom>(dto);
                var success = await _repository.UpdateAsync(id, typeRoom);
                if (id <= 0)
                    return BadRequest(new BaseResponse<string>("ID phải là số nguyên >0!", 400));
                if (!success)
                    return NotFound(new BaseResponse<string>("Không tìm thấy loại phòng", 404));

                return Ok(new BaseResponse<string>("Cập nhật thành công"));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new BaseResponse<string>(ex.Message, 400));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("admin/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _repository.DeleteAsync(id);
                if (id <= 0)
                    return BadRequest(new BaseResponse<string>("ID phải là số nguyên >0!", 400));
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
