using BookHotel.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BookHotel.Constant;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookHotel.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {

        private readonly AppDbContext _context;
        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/<ReviewController>/5
        [Authorize]
        [HttpGet("admin")]
        public async Task<ActionResult<IEnumerable<GetAllReview>>> GetAllReview(string? search, int? rating, int page = 1, int pageSize = 10)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            // Lấy Guess_id từ Email thay vì từ request (để an toàn hơn)
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền xóa đánh giá!", 400)));

            var query = _context.Reviews
                .Include(r => r.Guess)
                .Include(r => r.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search) && !rating.HasValue)
            {
                query = query.Where(r => r.Room.Name.Contains(search) || r.Comment.Contains(search));
            }

            if (rating.HasValue && string.IsNullOrEmpty(search))
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            if (!string.IsNullOrEmpty(search) && rating.HasValue)
            {
                query = query.Where(r => (r.Room.Name.Contains(search) || r.Comment.Contains(search)) && r.Rating == rating.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new GetAllReview
                {
                    Review_id = r.Review_id,
                    Room_id = r.Room_id,
                    Room_name = r.Room.Name,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Guess_name = r.Guess.Name,
                    Guess_avatar = $"{Request.Scheme}://{Request.Host}/uploads/users/{r.Guess.Thumbnail}",
                    CreatedAt = r.CreatedAt,
                })
                .ToListAsync();
                
                var response = new
                {
                    currentPage = page,
                    pageSize = pageSize,
                    totalPages = totalPages,
                    totalItems = totalCount,
                    items = reviews
                };
            if (reviews.Count == 0)
            {
                return NotFound(new ApiResponse(true, "Không tìm thấy đánh giá nào.",null));
            }
            else
                return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllReviewRoom>>> GetAllReviewRoom(int? rating, int room_id, int page = 1, int pageSize = 10)
        {
            var query = _context.Reviews
                .Include(r => r.Guess)
                .Include(r => r.Room)
                .AsQueryable();
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Room_id == room_id);
            if (room == null)
                return NotFound(new ApiResponse(false, null, new ErrorResponse("Không tìm thấy phòng này!", 404)));
            if (rating.HasValue)
            {
                query = query.Where(r => r.Rating == rating.Value && r.Room_id == room_id);
            }
            else
            {
                query = query.Where(r => r.Room_id == room_id);
            }
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new GetAllReviewRoom
                {
                    Review_id = r.Review_id,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Guess_name = r.Anonymous ? "Ẩn danh" : r.Guess.Name,
                    Guess_avatar = r.Anonymous ? $"{Request.Scheme}://{Request.Host}/uploads/users/default-user.png" : $"{Request.Scheme}://{Request.Host}/uploads/users/{r.Guess.Thumbnail}",
                    CreatedAt = r.CreatedAt,
                })
                .ToListAsync();

            if (reviews.Count == 0)
            {
                return NotFound(new ApiResponse(true, "Phòng này chưa có đánh giá.", null));
            }
            else
                return Ok(reviews);
        }


        // POST api/<ReviewController>
        [Authorize]
        [HttpPost("{id}")]
        public async Task<ActionResult> CreateReview(int id, [FromBody] CreateReview request)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            // Lấy Guess_id từ Email thay vì từ request (để an toàn hơn)
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            var isBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.Guess_id == guess.Guess_id && b.Booking_id==id);
            if (isBooking == null)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn chưa đặt phòng!", 400)));

            //kiểm tra người dùng đã đặt phòng chưa
            var isRoomBooking = await _context.Booking_Rooms.FirstOrDefaultAsync(br => br.Room_id == request.Room_id);
            if(isRoomBooking == null)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn chưa đặt phòng này!", 400)));
            }
            var isStatusBooking = await _context.Bookings.FirstOrDefaultAsync(b => b.Booking_id == isRoomBooking.Booking_id && b.Status == Constant.BookingConstant.CHECKED_OUT);
            if(isStatusBooking != null)
            {
                var review = new Review
                {
                    Guess_id = guess.Guess_id,
                    Comment = request.Comment,
                    Rating = request.Rating,
                    Room_id = request.Room_id,
                    Anonymous = request.Anonymous,
                };

                _context.Reviews.Add(review);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    // Cập nhật lại trạng thái của phòng
                    isStatusBooking.Status = Constant.BookingConstant.RATED;
                    _context.Bookings.Update(isStatusBooking);
                    await _context.SaveChangesAsync();
                    return Ok(new ApiResponse(true, "Thêm đánh giá thành công!", null));
                }
                else
                {
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Thêm đánh giá thất bại!", 400)));
                }
            }
            else
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn chưa hoàn thành quá trình đặt phòng!", 400)));
        }


        // DELETE api/<ReviewController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            // Lấy Guess_id từ Email thay vì từ request (để an toàn hơn)
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền xóa đánh giá!", 400)));

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Review_id == id);
            if (review == null)
                return NotFound(new ApiResponse(false, null, new ErrorResponse("Không tìm thấy đánh giá này!", 404)));

            _context.Reviews.Remove(review);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok(new ApiResponse(true, "Xóa đánh giá thành công!", null));
            }
            else
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Xóa đánh giá thất bại!", 400)));
            }
        }
    }
}
