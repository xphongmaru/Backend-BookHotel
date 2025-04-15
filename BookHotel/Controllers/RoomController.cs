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


[ApiController]
[Route("api/room")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    private readonly AppDbContext _context;

    public RoomController(IRoomRepository roomRepository, AppDbContext context)
    {
        _roomRepository = roomRepository;
        _context = context;
    }


    [HttpGet("room-details")]
    public async Task<IActionResult> GetRoomDetails(int roomId)
    {
        var room = await _roomRepository.GetRoomDetailsAsync(roomId, User);

        if (room == null)
        {
            return NotFound(new BaseResponse<string>("Không tìm thấy phòng!", 404));
        }

        var rating = room.Reviews.Any() ? room.Reviews.Average(r => r.Rating) : 0;

        var roomDto = new RoomDto
        {
            Room_id = room.Room_id,
            Name = room.Name,
            Price = room.Price,
            Max_occupancy = room.Max_occupancy,
            Description = room.Description,
            Status = room.Status, 
            Thumbnail = room.Thumbnail,
            RoomPhotos = room.RoomPhotos
                .Select(p => p.Image_url)
                .ToList(),
            TypeRoom = new
            {
                room.TypeRoom.TypeRoom_id,
                room.TypeRoom.Name,
                room.TypeRoom.Description
            },
            Rating = rating,
            TotalReviews = room.Reviews.Count,
            RoomAmenities = room.Room_Amenities
                .Where(a => a.Amenities != null)
                .Select(a => new AmenitiesDto
                {
                    Amenities_id = a.Amenities.Amenities_id,
                    Name = a.Amenities.Name,
                    Description = a.Amenities.Description
                })
                .ToList(),
            Reviews = room.Reviews
                .Select(r => new ReviewDto
                {
                    Review_id = r.Review_id,
                    Content = r.Comment,
                    Rating = r.Rating,
                    ReviewerName = r.Guess?.Name ?? "Ẩn danh",
                    CreatedAt = r.CreatedAt
                })
                .ToList()
        };

        return Ok(new BaseResponse<RoomDto>(roomDto));
    }


    [HttpGet("listRoom-pagination")]
    public async Task<IActionResult> GetRooms(int pageNumber = 1, int pageSize = 10)
    {
        (List<Room> rooms, int totalRooms) = await _roomRepository.GetRoomsAsync(pageNumber, pageSize, User);

        var roomIds = rooms.Select(r => r.Room_id).ToList();

        var bookingRoomCounts = await _context.Booking_Rooms
            .Where(br => roomIds.Contains(br.Room_id))
            .GroupBy(br => br.Room_id)
            .Select(g => new { RoomId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.RoomId, g => g.Count);

        var bookingRoomRevenues = await _context.Booking_Rooms
            .Where(br => roomIds.Contains(br.Room_id))
            .GroupBy(br => br.Room_id)
            .Select(g => new { RoomId = g.Key, Revenue = g.Sum(br => br.Price * br.Quantity) })
            .ToDictionaryAsync(g => g.RoomId, g => g.Revenue);

        var roomDtos = rooms.Select(r => new RoomListDto
        {
            Room_id = r.Room_id,
            Name = r.Name,
            Price = r.Price,
            Thumbnail = r.Thumbnail,
            RoomPhotos = r.RoomPhotos.Select(p => p.Image_url).ToList(),
            Status = r.Status,
            TypeRoom = r.TypeRoom == null ? null : new TypeRoomDto
            {
                TypeRoom_id = r.TypeRoom.TypeRoom_id,
                Name = r.TypeRoom.Name,
                Description = r.TypeRoom.Description
            },
            TotalBookings = bookingRoomCounts.TryGetValue(r.Room_id, out var count) ? count : 0,
            TotalRevenue = bookingRoomRevenues.TryGetValue(r.Room_id, out var revenue) ? revenue : 0,
            AvgRating = r.Reviews?.Any() == true ? r.Reviews.Average(rv => rv.Rating) : 0
        }).ToList();

        var pagedResult = new
        {
            totalRooms,
            totalPages = (int)Math.Ceiling((double)totalRooms / pageSize),
            currentPage = pageNumber,
            pageSize,
            rooms = roomDtos
        };

        return Ok(new BaseResponse<object>(pagedResult));
    }

    [HttpGet("best-selling")]
    public async Task<IActionResult> GetBestSellingRooms(int topN = 5)
    {
        if (topN <= 0)
        {
            throw new BadRequestException("Số lượng phòng topN phải lớn hơn 0.");
        }

        var rooms = await _roomRepository.GetBestSellingRoomsAsync(User);

        var rankedRooms = rooms
            .Select(room => new
            {
                Room = room,
                TotalBookings = room.Booking_Rooms.Count,
                AvgRating = room.Reviews.Any() ? room.Reviews.Average(r => r.Rating) : 0,
                TotalRevenue = room.Booking_Rooms.Sum(b => b.Price)
            })
            .OrderByDescending(r => r.TotalBookings)
            .ThenByDescending(r => r.AvgRating)
            .Take(topN);

        var roomDtos = rankedRooms.Select(r => new RoomListDto
        {
            Room_id = r.Room.Room_id,
            Name = r.Room.Name,
            Price = r.Room.Price,
            Thumbnail = r.Room.Thumbnail,
            RoomPhotos = r.Room.RoomPhotos.Select(p => p.Image_url).ToList(),
            Status = r.Room.Status, 
            TypeRoom = r.Room.TypeRoom == null ? null : new TypeRoomDto
            {
                TypeRoom_id = r.Room.TypeRoom.TypeRoom_id,
                Name = r.Room.TypeRoom.Name,
                Description = r.Room.TypeRoom.Description
            },
            TotalBookings = r.TotalBookings,
            TotalRevenue = r.TotalRevenue,
            AvgRating = r.AvgRating
        }).ToList();

        return Ok(new BaseResponse<List<RoomListDto>>(roomDtos));
    }


    [HttpGet("filter-rooms")]
    public async Task<IActionResult> FilterRooms(
        string? name = null,
        int? maxOccupancy = null,
        int? typeRoomId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? status = null,
        double? minRating = null)
    {
        var rooms = await _roomRepository.FilterRoomsAsync(
            name, maxOccupancy, typeRoomId, minPrice, maxPrice, status, minRating, User);

        var roomDtos = rooms.Select(r => new RoomListDto
        {
            Room_id = r.Room_id,
            Name = r.Name,
            Price = r.Price,
            Thumbnail = r.Thumbnail,
            RoomPhotos = r.RoomPhotos.Select(p => p.Image_url).ToList(),
            Status = r.Status,
            TypeRoom = r.TypeRoom == null ? null : new TypeRoomDto
            {
                TypeRoom_id = r.TypeRoom.TypeRoom_id,
                Name = r.TypeRoom.Name,
                Description = r.TypeRoom.Description
            },
            TotalBookings = r.Booking_Rooms.Count,
            TotalRevenue = r.Booking_Rooms.Sum(b => b.Price * b.Quantity), 
            AvgRating = r.Reviews?.Any() == true ? r.Reviews.Average(rev => rev.Rating) : 0 
        }).ToList();

        return Ok(new BaseResponse<List<RoomListDto>>(roomDtos));
    }

    [Authorize(Roles = "admin")]
    [HttpPost("create")]
    public async Task<IActionResult> CreateRoom([FromForm] CreateRoomDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new BaseResponse<string>("Dữ liệu không hợp lệ", 400));

        // Validate loại phòng
        var typeRoom = await _context.TypeRooms.FindAsync(dto.TypeRoom_id);
        if (typeRoom == null)
            return NotFound(new BaseResponse<string>("Không tìm thấy loại phòng", 404));

        // Validate tiện nghi
        var amenities = await _context.Amenities
            .Where(a => dto.AmenityIds.Contains(a.Amenities_id))
            .ToListAsync();

        if (amenities.Count != dto.AmenityIds.Count)
            return BadRequest(new BaseResponse<string>("Một hoặc nhiều tiện nghi không hợp lệ", 400));

        // Validate Status hợp lệ
        var validStatuses = new[] { RoomStatus.Available, RoomStatus.Hidden, RoomStatus.Unavailable };
        if (!validStatuses.Contains(dto.Status))
            return BadRequest(new BaseResponse<string>("Trạng thái phòng không hợp lệ", 400));

        // Tạo thư mục lưu ảnh nếu chưa có
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/rooms");
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Lưu ảnh thumbnail
        var thumbnailFileName = Guid.NewGuid() + Path.GetExtension(dto.Thumbnail.FileName);
        var thumbnailPath = Path.Combine(uploadPath, thumbnailFileName);
        using (var stream = new FileStream(thumbnailPath, FileMode.Create))
        {
            await dto.Thumbnail.CopyToAsync(stream);
        }

        // Lưu danh sách ảnh mô tả
        var roomPhotos = new List<RoomPhoto>();
        if (dto.RoomPhotos != null)
        {
            foreach (var photo in dto.RoomPhotos)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
                roomPhotos.Add(new RoomPhoto { Image_url = "/uploads/rooms/" + fileName });
            }
        }

        // Tạo entity phòng
        var room = new Room
        {
            Name = dto.Name,
            Price = dto.Price,
            Max_occupancy = dto.Max_occupancy,
            Description = dto.Description,
            Status = dto.Status, 
            Thumbnail = "/uploads/rooms/" + thumbnailFileName,
            TypeRoom_id = dto.TypeRoom_id,
            RoomPhotos = roomPhotos,
            Room_Amenities = dto.AmenityIds
                .Select(id => new Room_Amenities { Amenities_id = id })
                .ToList()
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return Ok(new BaseResponse<string>("Thêm phòng thành công"));
    }



    [Authorize(Roles = "admin")]
    [HttpPut("update-room/{roomId}")]
    public async Task<IActionResult> UpdateRoom(int roomId, [FromForm] UpdateRoomDto dto)
    {
        if (!ModelState.IsValid)
            throw new BadRequestException("Dữ liệu không hợp lệ");

        var validStatuses = new[] { RoomStatus.Available, RoomStatus.Hidden, RoomStatus.Unavailable };
        if (!validStatuses.Contains(dto.Status))
            throw new BadRequestException("Trạng thái phòng không hợp lệ");

        var result = await _roomRepository.UpdateRoomAsync(roomId, dto);

        if (!result.Success)
            throw new NotFoundException(result.Message);

        return Ok(new BaseResponse<string>("Cập nhật phòng thành công"));
    }


    [Authorize(Roles = "admin")]
    [HttpDelete("{roomId}")]
    public async Task<IActionResult> DeleteRoom(int roomId)
    {
        await _roomRepository.DeleteRoomAsync(roomId);
        return Ok(new BaseResponse<string>("Xóa phòng thành công"));
    }


    [Authorize(Roles = "admin")]
    [HttpPut("{id}/hide")]
    public async Task<IActionResult> HideRoom(int id)
    {
        var (success, message) = await _roomRepository.HideRoomAsync(id);
        if (!success)
            return NotFound(new BaseResponse<string>(message, 404));

        return Ok(new BaseResponse<string>(message, 200));
    }

}
