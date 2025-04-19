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

    private bool IsValidImage(IFormFile file)
    {
        var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            return false;

        return true;
    }

    [HttpGet("room-details")]
    public async Task<IActionResult> GetRoomDetails(int roomId)
    {
        var room = await _roomRepository.GetRoomDetailsAsync(roomId, User);

        if (roomId <= 0)
            return BadRequest(new BaseResponse<string>("ID phải là số nguyên >0!", 400));

        if (room == null)
            return NotFound(new BaseResponse<string>("Không tìm thấy phòng!", 404));

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var rating = room.Reviews.Any() ? room.Reviews.Average(r => r.Rating) : 0;

        var roomDto = new RoomDto
        {
            Room_id = room.Room_id,
            Name = room.Name,
            Price = room.Price,
            Max_occupancy = room.Max_occupancy,
            Description = room.Description,
            Status = room.Status,
            Thumbnail = baseUrl + room.Thumbnail, // Ghép đầy đủ đường dẫn
            RoomPhotos = room.RoomPhotos
                .Select(p => baseUrl + p.Image_url)
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
                    ReviewerName = r.Anonymous ? "Ẩn danh" : (r.Guess?.Name ?? "Ẩn danh"),
                    ReviewerThumbnail = r.Anonymous ? "" :
                        (!string.IsNullOrEmpty(r.Guess?.Thumbnail) ? baseUrl + r.Guess.Thumbnail : ""),
                    CreatedAt = r.CreatedAt.ToString("dd/MM/yyyy")
                })
                .ToList()
        };

        return Ok(new BaseResponse<RoomDto>(roomDto));
    }

    [HttpGet("listRoom-pagination")]
    public async Task<IActionResult> GetRooms(int pageNumber = 1, int pageSize = 10)
    {
        (List<Room> rooms, int totalRooms) = await _roomRepository.GetRoomsAsync(pageNumber, pageSize, User);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

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
            Thumbnail = baseUrl + r.Thumbnail,
            RoomPhotos = r.RoomPhotos.Select(p => baseUrl + p.Image_url).ToList(),
            Status = r.Status,
            Max_occupancy = r.Max_occupancy,
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
            throw new BadRequestException("Số lượng phòng topN phải lớn hơn 0.");

        var rooms = await _roomRepository.GetBestSellingRoomsAsync(User);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

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
            Thumbnail = baseUrl + r.Room.Thumbnail,
            RoomPhotos = r.Room.RoomPhotos.Select(p => baseUrl + p.Image_url).ToList(),
            Status = r.Room.Status,
            Max_occupancy = r.Room.Max_occupancy,
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
    public async Task<IActionResult> FilterRooms([FromQuery] FilterRoomDto filterDto)
    {
        var name = filterDto.Name;
        var maxOccupancy = filterDto.MaxOccupancy;
        var typeRoomId = filterDto.TypeRoomId;
        var minPrice = filterDto.MinPrice;
        var maxPrice = filterDto.MaxPrice;
        var status = filterDto.Status;
        var minRating = filterDto.MinRating;
        var amenityIds = filterDto.AmenityIds;

        // Validate tên phòng
        if (!string.IsNullOrWhiteSpace(name))
        {
            if (name.Length > 100)
                throw new BadRequestException("Tên phòng quá dài.");
            if (name.Any(c => "!@#$%^&*()+=[]{}<>?/\\|~`".Contains(c)))
                throw new BadRequestException("Tên phòng không được chứa ký tự đặc biệt.");
        }

        // Validate sức chứa
        if (maxOccupancy.HasValue && maxOccupancy <= 0)
            throw new BadRequestException("Sức chứa tối đa phải lớn hơn 0.");

        // Validate giá
        if (minPrice < 0 || maxPrice < 0)
            throw new BadRequestException("Giá không được âm.");
        if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            throw new BadRequestException("Giá tối thiểu không được lớn hơn giá tối đa.");

        // Validate rating
        if (minRating < 0 || minRating > 5)
            throw new BadRequestException("Đánh giá trung bình phải từ 0 đến 5.");

        // Validate status
        var allowedStatuses = new[] { RoomStatus.Available, RoomStatus.Unavailable, RoomStatus.Hidden };
        if (!string.IsNullOrWhiteSpace(status) && !allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            throw new BadRequestException($"Trạng thái không hợp lệ. Chỉ chấp nhận: {string.Join(", ", allowedStatuses)}");

        // Validate kiểu phòng
        if (typeRoomId.HasValue)
        {
            var exists = await _context.TypeRooms.AnyAsync(t => t.TypeRoom_id == typeRoomId.Value);
            if (!exists)
                throw new BadRequestException("Kiểu phòng không tồn tại.");
        }

        // Validate tiện nghi
        if (amenityIds != null && amenityIds.Any())
        {
            if (amenityIds.Any(id => id <= 0))
                throw new BadRequestException("Tất cả ID tiện nghi phải là số nguyên dương.");

            var validAmenityIds = await _context.Amenities
                .Where(a => amenityIds.Contains(a.Amenities_id))
                .Select(a => a.Amenities_id)
                .ToListAsync();

            var invalidIds = amenityIds.Except(validAmenityIds).ToList();
            if (invalidIds.Any())
                throw new BadRequestException($"Các tiện nghi không tồn tại: {string.Join(", ", invalidIds)}");
        }

        // Lọc phòng
        var rooms = await _roomRepository.FilterRoomsAsync(filterDto, User);

        if (!rooms.Any())
        {
            if (!string.IsNullOrWhiteSpace(name))
                throw new NotFoundException($"Không tìm thấy phòng với tên chứa '{name}'.");
            throw new NotFoundException("Không tìm thấy phòng phù hợp với điều kiện lọc.");
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        // Map DTO
        var roomDtos = rooms.Select(r => new RoomListDto
        {
            Room_id = r.Room_id,
            Name = r.Name,
            Price = r.Price,
            Thumbnail = baseUrl + r.Thumbnail,
            RoomPhotos = r.RoomPhotos.Select(p => baseUrl + p.Image_url).ToList(),
            Status = r.Status,
            Max_occupancy = r.Max_occupancy,
            TypeRoom = r.TypeRoom == null ? null : new TypeRoomDto
            {
                TypeRoom_id = r.TypeRoom.TypeRoom_id,
                Name = r.TypeRoom.Name,
                Description = r.TypeRoom.Description
            },
            TotalBookings = r.Booking_Rooms.Count,
            TotalRevenue = r.Booking_Rooms.Sum(b => b.Price * b.Quantity),
            AvgRating = r.Reviews?.Any() == true ? r.Reviews.Average(rev => rev.Rating) : 0,
            RoomAmenities = r.Room_Amenities.Select(ra => new AmenitiesDto
            {
                Amenities_id = ra.Amenities.Amenities_id,
                Name = ra.Amenities.Name,
                Description = ra.Amenities.Description
            }).ToList()
        }).ToList();

        return Ok(new BaseResponse<List<RoomListDto>>(roomDtos));
    }


    [Authorize(Roles = "admin")]
    [HttpPost("admin/create")]
    public async Task<IActionResult> CreateRoom([FromForm] CreateRoomDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new BaseResponse<string>("Dữ liệu không hợp lệ", 400));

        var isDuplicate = await _context.Rooms
            .AnyAsync(r => r.Name.ToLower() == dto.Name.ToLower());
        if (isDuplicate)
            return BadRequest(new BaseResponse<string>("Tên phòng đã tồn tại", 400));

        var typeRoom = await _context.TypeRooms.FindAsync(dto.TypeRoom_id);
        if (typeRoom == null)
            return NotFound(new BaseResponse<string>("Không tìm thấy loại phòng", 404));

        var amenities = await _context.Amenities
            .Where(a => dto.AmenityIds.Contains(a.Amenities_id))
            .ToListAsync();
        if (amenities.Count != dto.AmenityIds.Count)
            return BadRequest(new BaseResponse<string>("Một hoặc nhiều tiện nghi không hợp lệ", 400));

        var validStatuses = new[] { RoomStatus.Available, RoomStatus.Hidden, RoomStatus.Unavailable };
        if (!validStatuses.Contains(dto.Status))
            return BadRequest(new BaseResponse<string>("Trạng thái phòng không hợp lệ", 400));

        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/rooms");
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Kiểm tra thumbnail
        if (!IsValidImage(dto.Thumbnail))
            return BadRequest(new BaseResponse<string>("Ảnh đại diện không hợp lệ (phải là ảnh JPG/PNG, <5MB)", 400));

        // Lưu ảnh thumbnail
        var thumbnailFileName = Guid.NewGuid() + Path.GetExtension(dto.Thumbnail.FileName);
        var thumbnailPath = Path.Combine(uploadPath, thumbnailFileName);
        using (var stream = new FileStream(thumbnailPath, FileMode.Create))
        {
            await dto.Thumbnail.CopyToAsync(stream);
        }
        var thumbnailRelativeUrl = "/uploads/rooms/" + thumbnailFileName;

        // Lưu ảnh mô tả (chỉ lưu đường dẫn tương đối)
        var roomPhotos = new List<RoomPhoto>();
        if (dto.RoomPhotos != null)
        {
            foreach (var photo in dto.RoomPhotos)
            {
                if (!IsValidImage(photo))
                    return BadRequest(new BaseResponse<string>($"Ảnh mô tả không hợp lệ: {photo.FileName}", 400));

                var fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }
                var relativeUrl = "/uploads/rooms/" + fileName;

                roomPhotos.Add(new RoomPhoto
                {
                    Image_url = relativeUrl 
                });
            }
        }

        var room = new Room
        {
            Name = dto.Name,
            Price = dto.Price,
            Max_occupancy = dto.Max_occupancy,
            Description = dto.Description,
            Status = dto.Status,
            Thumbnail = thumbnailRelativeUrl,
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
    [HttpPut("admin/update-room/{roomId}")]
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
    [HttpDelete("admin/{roomId}")]
    public async Task<IActionResult> DeleteRoom(int roomId)
    {
        await _roomRepository.DeleteRoomAsync(roomId);
        return Ok(new BaseResponse<string>("Xóa phòng thành công"));
    }

    [Authorize(Roles = "admin")]
    [HttpPut("admin/{id}/hide")]
    public async Task<IActionResult> HideRoom(int id)
    {
        var (success, message) = await _roomRepository.HideRoomAsync(id);
        if (!success)
            return NotFound(new BaseResponse<string>(message, 404));

        return Ok(new BaseResponse<string>(message, 200));
    }

    [Authorize(Roles = "admin")]
    [HttpGet("admin/room-statuses")]
    public IActionResult GetRoomStatuses()
    {
        var statuses = RoomStatus.GetAll();
        return Ok(new BaseResponse<List<string>>(statuses));
    }

}
