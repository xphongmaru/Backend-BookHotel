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

    [HttpGet("get-by-id/{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _repository.GetByIdAsync(id);
        if (data == null)
            return NotFound(new BaseResponse<string>("Không tìm thấy tiện nghi", 404));

        var result = _mapper.Map<AmenityDto>(data);
        return Ok(new BaseResponse<AmenityDto>(result));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] AmenityCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new BaseResponse<string>("Dữ liệu không hợp lệ", 400));

        try
        {
            var amenity = _mapper.Map<Amenities>(dto);
            var created = await _repository.CreateAsync(amenity);
            return StatusCode(201, new BaseResponse<string>("Tạo tiện nghi thành công"));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new BaseResponse<string>(ex.Message, 400));
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AmenityCreateDto dto)
    {
        try
        {
            var amenity = _mapper.Map<Amenities>(dto);
            var success = await _repository.UpdateAsync(id, amenity);
            if (!success)
                return NotFound(new BaseResponse<string>("Không tìm thấy tiện nghi", 404));

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
                return NotFound(new BaseResponse<string>("Không tìm thấy tiện nghi", 404));

            return Ok(new BaseResponse<string>("Xóa thành công"));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new BaseResponse<string>(ex.Message, 400));
        }
    }
}


