using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace BookHotel.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        public readonly AppDbContext _context;

        public BookingController(AppDbContext context, IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
            _context = context;
        }

        [Authorize]
        [HttpGet("admin")]
        public async Task<ActionResult> geAlltBooking()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
         if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            var bookingList = await _bookingRepository.GetBooking();
            return Ok(new ApiResponse(true,bookingList,null));
        }

        [Authorize]
        [HttpPost("user")]
        public async Task<ActionResult> addBooking ([FromBody] BookingRoomCreateRequest request)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
                if (guess == null)
                    return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
  

                var booking = _context.Bookings.FirstOrDefault(b => b.Guess_id == guess.Guess_id && b.Status == Constant.BookingConstant.OPEN);

                if (booking == null)
                {
                    booking = new Booking
                    {
                        Status = Constant.BookingConstant.OPEN,
                        Guess_id = guess.Guess_id,
                        Check_in = DateTime.ParseExact(request.Check_in,"dd/MM/yyyy",null),
                        Check_out = DateTime.ParseExact(request.Check_out, "dd/MM/yyyy", null)
                    };
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();
                }

                if (booking != null)
                {
                    booking.Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == booking.Booking_id).ToListAsync().Result;
                    
                    if (DateTime.ParseExact(request.Check_out, "dd/MM/yyy", null) <= DateTime.ParseExact(request.Check_in,"dd/MM/yyy",null)) {
                        throw new Exception("check out phải >= ngày check in");
                    }   

                    if (booking.Check_in != DateTime.ParseExact(request.Check_in,"dd/MM/yyy",null)) {
                        throw new Exception("không được đổi check in");
                    }

                    if (booking.Check_out != DateTime.ParseExact(request.Check_out,"dd/MM/yyy",null)) {
                        throw new Exception("không được đổi check out");
                    }

                    if (request.Quantity > _context.Rooms.FirstOrDefault(d => d.Room_id == request.Room_id).Max_occupancy)
                    {
                        throw new Exception("so nguoi > suc chua cua phong");
                    }
                }

                var bookingRoom = new Booking_Room
                {
                    Booking_id = booking.Booking_id,
                    Room_id = request.Room_id,
                    Quantity = request.Quantity,
                    Name_Guess = request.Name_Guess,
                    Phone_Guess = request.Phone_Guess,
                };

                _context.Booking_Rooms.Add(bookingRoom);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true, "thêm phòng thành công", null));
            }
            catch (Exception ex) {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }
        }

        [Authorize]
        [HttpGet("user")]
        public async  Task<ActionResult> getBooking()
        {
            try {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
                if (guess == null)
                    return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
              
                var booking = _context.Bookings.FirstOrDefault(b => b.Guess_id == guess.Guess_id && b.Status == Constant.BookingConstant.OPEN);

                if (booking == null)
                {
                    throw new Exception("chưa đặt phòng");                
                }
                

                booking.Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == booking.Booking_id).ToListAsync().Result;

                var respone = new BookingRespone
                {
                    Booking_id = booking.Booking_id,
                    Check_in = booking.Check_in.ToString("dd/MM/yyy"),
                    Check_out = booking.Check_out.ToString("dd/MM/yyy"),
                    Status = booking.Status,
                    Description = booking.Description,
                    Request = booking.Request,
                    Guess_id = booking.Guess_id,
                    Booking_Rooms = booking.Booking_Rooms,
                    total = _bookingRepository.getTotal(booking.Booking_id) 
                };

                return Ok(new ApiResponse(true, respone, null));

            }
            catch(Exception ex)
            {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }
        }

        [Authorize]
        [HttpPut("user")]
        public async Task<ActionResult> processBooking( [FromBody] DiscountRequest request)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
                if (guess == null)
                    return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
                

                var booking = _context.Bookings.FirstOrDefault(b => b.Guess_id == guess.Guess_id && b.Status == Constant.BookingConstant.OPEN);
                
                if (booking == null)
                {
                    throw new Exception("booking khong ton tai");
                }
 
               var discount = _context.Discounts.FirstOrDefault(ds => ds.Code == request.DiscountCode);

                if (discount==null)
                {
                    throw new Exception("discount khong ton tai");                   
                }

                if (discount.End_date < DateTime.Now)
                {
                    throw new Exception("discount da het han");
                }

                decimal discount_value = (decimal) discount.Discount_percentage*request.total;
                
                if(discount_value>(decimal)discount.Max_discount)
                {
                    discount_value = (decimal)discount.Max_discount;
                }    

                booking.Status =Constant.BookingConstant.PENDDING;
                _context.Entry(booking).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                BookingRespone respone = new BookingRespone
                {
                    Booking_id = booking.Booking_id,
                    Check_in = booking.Check_in.ToString("dd/MM/yyy"),
                    Check_out = booking.Check_out.ToString("dd/MM/yyy"),
                    Status = booking.Status,
                    Description = booking.Description,
                    Request = booking.Request,
                    Guess_id = booking.Guess_id,
                    Booking_Rooms = booking.Booking_Rooms,
                    total = request.total-discount_value
                };


                return Ok(new ApiResponse(true,respone,null));
            }
            catch (Exception ex) {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }
        }

        //[Authorize]
        [HttpDelete("user")]
        public async Task <ActionResult> deleteBookingItem([FromBody] BookingRoomDeleteRequest request)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
                if (guess == null)
                    return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

                var booking = _context.Bookings.FirstOrDefault(b => b.Guess_id == guess.Guess_id && b.Status == Constant.BookingConstant.OPEN);

                if (booking == null)
                {
                    throw new Exception("Khong ton tai booking");
                }

                if (booking.Guess_id != guess.Guess_id)
                {
                    throw new Exception("Khong duoc xoa");
                }

                var bookingRoom = _context.Booking_Rooms.FirstOrDefault(br => br.Booking_id == booking.Booking_id&&br.Room_id==request.RoomId);

                if (bookingRoom == null) {
                    throw new Exception("Khong ton tai bookingRoom");
                }
                
                _context.Booking_Rooms.Remove(bookingRoom);
                 await _context.SaveChangesAsync();

                return Ok(new ApiResponse(true,bookingRoom,null));
            }
            catch (Exception ex)
            {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }
        }

        [Authorize]
        [HttpPut("admin")]
        public  async Task<ActionResult> updateBooking([FromBody] BookingRespone request)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));


            var booking = _context.Bookings.FirstOrDefault(b => b.Booking_id ==request.Booking_id );   

            booking.Check_in = DateTime.ParseExact(request.Check_in, "dd/MM/yyy", null);
            booking.Check_out = DateTime.ParseExact(request.Check_out, "dd/MM/yyy", null);
            booking.Status=request.Status;
            booking.Description=request.Description;
            booking.Request=request.Request;

            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(true, booking, null));
        }

    } 
} 

