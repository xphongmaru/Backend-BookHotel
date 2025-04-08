using Microsoft.AspNetCore.Mvc;
using BookHotel.DTOs;
using BookHotel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookHotel.Repositories.Admin;
using BookHotel.Response;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;

    public RoomController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    [HttpGet("room-details")]
    public async Task<IActionResult> GetRoomDetails(int roomId)
    {
        var room = await _roomRepository.GetRoomDetailsAsync(roomId);
        if (room == null)
        {
            return NotFound(new BaseResponse<string>("Không tìm thấy phòng!", 404));
        }

        var roomDto = new RoomDto
        {
            Room_id = room.Room_id,
            Name = room.Name,
            Price = room.Price,
            Max_occupancy = room.Max_occupancy,
            Description = room.Description,
            Status = room.Status,
            Thumbnail = room.Thumbnail,
            RoomPhotos = room.RoomPhotos.Select(p => p.Image_url).ToList(),
            TypeRoom = new
            {
                room.TypeRoom.TypeRoom_id,
                room.TypeRoom.Name,
                room.TypeRoom.Description
            },
            Rating = room.Reviews.Any() ? room.Reviews.Average(r => r.Rating) : 0,
            TotalReviews = room.Reviews.Count,
            RoomAmenities = room.Room_Amenities
                .Where(a => a.Amenities != null)
                .Select(a => new AmenitiesDto
                {
                    Amenities_id = a.Amenities.Amenities_id,
                    Name = a.Amenities.Name,
                    Description = a.Amenities.Description
                }).ToList(),
            Reviews = room.Reviews.Select(r => new ReviewDto
            {
                Review_id = r.Review_id,
                Content = r.Comment,
                Rating = r.Rating,
                ReviewerName = r.Guess?.Name ?? "Ẩn danh",
                CreatedAt = r.CreatedAt
            }).ToList()
        };

        return Ok(new BaseResponse<RoomDto>(roomDto));
    }

    [HttpGet("listRoom-pagination")]
    public async Task<IActionResult> GetRooms(int pageNumber = 1, int pageSize = 10)
    {
        (List<Room> rooms, int totalRooms) = await _roomRepository.GetRoomsAsync(pageNumber, pageSize);

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
            TotalBookings = r.Bookings?.Count ?? 0,
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
        var rooms = await _roomRepository.GetBestSellingRoomsAsync();

        var roomDtos = rooms
            .Select(r => new
            {
                Room = r,
                TotalBookings = r.Booking_Rooms.Count,
                AvgRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0,
                TotalRevenue = r.Booking_Rooms.Sum(b => b.Price)
            })
            .OrderByDescending(r => r.TotalBookings)
            .ThenByDescending(r => r.AvgRating)
            .Take(topN)
            .Select(r => new RoomListDto
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
        var rooms = await _roomRepository.FilterRoomsAsync(name, maxOccupancy, typeRoomId, minPrice, maxPrice, status, minRating);

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
            TotalRevenue = r.Booking_Rooms.Sum(b => b.Price),
            AvgRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0
        }).ToList();

        return Ok(new BaseResponse<List<RoomListDto>>(roomDtos));
    }
}
