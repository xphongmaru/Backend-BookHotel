using Microsoft.AspNetCore.Mvc;
using BookHotel.DTOs;
using BookHotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookHotel.Repositories.Admin;
using BookHotel.Responsee;
using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using BookHotel.Constant;
using AutoMapper;

[ApiController]
[Route("api/amenity")]
public class AmenityController : ControllerBase
{
    private readonly IAmenityRepository _repository;
    private readonly IMapper _mapper;

    public AmenityController(IAmenityRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repository.GetAllAsync();
        var result = _mapper.Map<IEnumerable<AmenityDto>>(data);
        return Ok(new BaseResponse<IEnumerable<AmenityDto>>(result));
    }

    [HttpGet("get-amenities-by-room/{roomId}")]
    public async Task<IActionResult> GetAmenitiesByRoomId(int roomId)
    {
        if (roomId <= 0)
            return BadRequest(new BaseResponse<string>("ID phòng phải là số nguyên >0!", 400));

        var amenities = await _repository.GetAmenitiesByRoomIdAsync(roomId);

        if (amenities == null)
        {
            return NotFound(new BaseResponse<string>("Phòng không tồn tại!", 404));
        }

        if (!amenities.Any())
        {
            return Ok(new BaseResponse<string>("Phòng không có tiện nghi!"));
        }

        return Ok(new BaseResponse<List<AmenitiesDto>>(amenities));
    }

    [Authorize(Roles = "admin")]
    [HttpGet("admin/get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _repository.GetByIdAsync(id);
        if (id <= 0)
            return BadRequest(new BaseResponse<string>("ID phải là số nguyên >0!", 400));

        if (data == null)
            return NotFound(new BaseResponse<string>("Không tìm thấy tiện nghi!", 404));

        var result = _mapper.Map<AmenityDto>(data);
        return Ok(new BaseResponse<AmenityDto>(result));
    }

    [Authorize(Roles = "admin")]
    [HttpPost("admin/create")]
    public async Task<IActionResult> Create([FromBody] AmenityCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new BaseResponse<string>("Dữ liệu không hợp lệ!", 400));

        try
        {
            var amenity = _mapper.Map<Amenities>(dto);
            var created = await _repository.CreateAsync(amenity);
            return StatusCode(201, new BaseResponse<string>("Tạo tiện nghi thành công!"));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new BaseResponse<string>(ex.Message, 400));
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("admin/update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AmenityCreateDto dto)
    {
        try
        {
            var amenity = _mapper.Map<Amenities>(dto);
            var success = await _repository.UpdateAsync(id, amenity);
            if (id <= 0)
                return BadRequest(new BaseResponse<string>("ID phải là số nguyên >0!", 400));
            if (!success)
                return NotFound(new BaseResponse<string>("Không tìm thấy tiện nghi!", 404));

            return Ok(new BaseResponse<string>("Cập nhật thành công!"));
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
                return NotFound(new BaseResponse<string>("Không tìm thấy tiện nghi!", 404));

            return Ok(new BaseResponse<string>("Xóa thành công"));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new BaseResponse<string>(ex.Message, 400));
        }
    }
}


