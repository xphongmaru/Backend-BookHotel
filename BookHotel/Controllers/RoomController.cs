using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BookHotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomDetails(int roomId)
        {
            var room = await _roomRepository.GetRoomDetailsAsync(roomId);
            if (room == null)
            {
                return NotFound(new { message = "Không tìm thấy phòng!" });
            }

            // Chuyển đổi sang DTO
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

                // Gán TypeRoom vào DTO
                TypeRoom = new
                {
                    room.TypeRoom.TypeRoom_id,
                    room.TypeRoom.Name,
                    room.TypeRoom.Description
                },

                //  Tính toán rating trung bình, nếu không có thì = 0
                Rating = room.Reviews.Any() ? room.Reviews.Average(r => r.Rating) : 0,

                //  Thêm danh sách Room_Amenities
                RoomAmenities = room.Room_Amenities
                    .Where(a => a.Amenities != null)
                    .Select(a => new AmenitiesDto // Dùng DTO thay vì anonymous type
                    {
                        Amenities_id = a.Amenities.Amenities_id,
                        Name = a.Amenities.Name,
                        Description = a.Amenities.Description
                        }).ToList()
                    };

            return Ok(roomDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms(int pageNumber = 1, int pageSize = 10)
        {
            (List<Room> rooms, int totalRooms) = await _roomRepository.GetRoomsAsync(pageNumber, pageSize);

            var roomDtos = rooms.Select(r => new RoomListDto
            {
                Room_id = r.Room_id,
                Name = r.Name,
                Price = r.Price,
                Thumbnail = r.Thumbnail,
                RoomPhotos = r.RoomPhotos.Select(p => p.Image_url).ToList() // Lấy danh sách ảnh
            }).ToList();

            return Ok(new
            {
                totalRooms,
                totalPages = (int)Math.Ceiling((double)totalRooms / pageSize),
                currentPage = pageNumber,
                pageSize,
                rooms = roomDtos
            });
        }

        [HttpGet("best-selling")]
        public async Task<IActionResult> GetBestSellingRooms(int topN = 5)
        {
            var rooms = await _roomRepository.GetBestSellingRoomsAsync(topN);

            var roomDtos = rooms.Select(r => new RoomListDto
            {
                Room_id = r.Room_id,
                Name = r.Name,
                Price = r.Price,
                Thumbnail = r.Thumbnail,
                RoomPhotos = r.RoomPhotos.Select(p => p.Image_url).ToList(),
                TotalBookings = r.Booking_Rooms.Count,
                TotalRevenue = r.Booking_Rooms.Sum(b => b.Price),
                AvgRating = r.Reviews.Any() ? r.Reviews.Average(rev => rev.Rating) : 0
            }).ToList();

            return Ok(roomDtos);
        }
    }
}

