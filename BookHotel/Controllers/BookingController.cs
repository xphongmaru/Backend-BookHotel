using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookHotel.Data;

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

        [HttpGet("admin")]
        public async Task<ActionResult> getBooking()
        {
            var bookingList = await _bookingRepository.GetBooking();
            return Ok(new ApiResponse(true,bookingList,null));
        }

        [HttpPost("user")]
        public async Task<ActionResult> addBooking(int id, [FromBody] BookingRoomCreateRequest request)
        {
            try
            {
                var booking = _context.Bookings.FirstOrDefault(b => b.Guess_id == id && b.Status == "Open");

                if (booking == null)
                {
                    booking = new Booking
                    {
                        Status = "Open",
                        Guess_id = id,
                        Check_in = request.Check_in,
                        Check_out = request.Check_out
                    };
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();
                }

                if (booking != null)
                {
                    if (request.Check_out < request.Check_in) {
                        throw new Exception("O am ngay");
                    }

                    booking.Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == id).ToListAsync().Result;

                    if (booking.Check_in != request.Check_in) {
                        throw new Exception("ngay check in k.o dc khac");
                    }

                    if (booking.Check_out != request.Check_out) {
                        throw new Exception("ngay check out k.o dc khac");
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
                    Price = request.Price,
                    Name_Guess = request.Name_Guess,
                    Phone_Guess = request.Phone_Guess,
                };

                _context.Booking_Rooms.Add(bookingRoom);
                await _context.SaveChangesAsync();


                var respone = new BookingRespone
                {
                    Booking_id = booking.Booking_id,
                    Check_in = booking.Check_in,
                    Check_out = booking.Check_out,
                    Status = booking.Status,
                    Description = booking.Description,
                    Request = booking.Request,
                    Guess_id = booking.Guess_id,
                    Booking_Rooms = booking.Booking_Rooms,
                    total = _bookingRepository.getTotal(booking.Booking_id),
                };

                return Ok(new ApiResponse(true, respone, null));
            }
            catch (Exception ex) {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }
        }

        [HttpPut("user")]
        public async Task<ActionResult> processPayment(int id, [FromBody] PaymentRequest request)
        {
            try
            {

                var booking = _context.Bookings.FirstOrDefault(b => b.Guess_id == id && b.Status == "open");
                if (booking == null)
                {
                    throw new Exception("booking khong ton tai");
                }
                var discount = _context.Discounts.FirstOrDefault(d => d.Code == request.DiscountCode);
                 
                if (discount == null) {
                    throw new Exception("giam gia khong ton tai");
                }

                if (discount.End_date <= DateTime.Now) {
                    throw new Exception("giam gia ended");
                }

                if (discount.Price_applies > (float)request.total)
                {
                    throw new Exception("khong dat dieu kien");
                }

                var discount_value = discount.Discount_percentage * (float)request.total;

                if (discount.Max_discount != 0)
                {
                    if (discount_value > discount.Max_discount)
                    {
                        discount_value = discount.Max_discount;
                    }
                }

                booking.Status = "Paid";
                _context.Entry(booking).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                var respone = new BookingRespone
                {
                    Booking_id = booking.Booking_id,
                    Check_in = booking.Check_in,
                    Check_out = booking.Check_out,
                    Status = booking.Status,
                    Description = booking.Description,
                    Request = booking.Request,
                    Guess_id = booking.Guess_id,
                    Booking_Rooms = booking.Booking_Rooms,
                    total = request.total-(decimal)discount_value,
                };

                return Ok(new ApiResponse(true,(float) request.total-discount_value,null));
            }
            catch (Exception ex) {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }
        }
        [HttpPut("admin")]
        public  async Task<ActionResult> updateBooking([FromBody] BookingRespone request)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Booking_id ==request.Booking_id );

            booking.Check_in=request.Check_in;
            booking.Check_out=request.Check_out;
            booking.Status=request.Status;
            booking.Description=request.Description;
            booking.Request=request.Request;

            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(true, booking, null));
            

        }
            
    } 
} 

