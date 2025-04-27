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
using Azure.Core;

namespace BookHotel.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        public readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize]
        [HttpPost("checkout")]
        public async Task<ActionResult> CheckoutBooking([FromBody] CheckoutCreateRequest request)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            //kiểm tra thông tin đặt phòng
            var rooms = request.Rooms;
            //check trạng thái phòng
            decimal total = 0;
            foreach (var room in rooms)
            {
                var roomCheck = _context.Rooms.FirstOrDefault(r => r.Room_id == room.Room_id);
                if (roomCheck == null)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Phòng không tồn tại", 400)));
                if (roomCheck.Status == Constant.RoomStatus.Unavailable)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Phòng đã được đặt", 400)));
                //tính tiền
                total += roomCheck.Price;

            }

            //tính số ngày đặt
            var checkInDate = DateTime.Parse(request.Check_in);
            var checkOutDate = DateTime.Parse(request.Check_out);
            var totalDays = (checkOutDate - checkInDate).TotalDays;
            if (totalDays <= 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Ngày đặt không hợp lệ", 400)));

            total = total * (decimal)totalDays;
            decimal discount_value = 0;
            //check discount
            if (request.Discount != string.Empty)
            {
                var discount = _context.Discounts.FirstOrDefault(d => d.Code == request.Discount);
                if (discount == null)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mã giảm giá không tồn tại", 400)));
                //check thời gian
                if (discount.Start_date > DateTime.Now || discount.End_date < DateTime.Now)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mã giảm giá không hợp lệ", 400)));

                //check số tiền
                if (discount.Status == false)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mã giảm giá không hợp lệ", 400)));

                if (total < (decimal)discount.Price_applies)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mã giảm giá không hợp lệ", 400)));

                ////check số lượng
                if (discount.Quantity <= 0)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mã giảm giá đã hết lượt sử dụng.", 400)));

                //check người dùng
                var bookingDiscount = await _context.Bookings.FirstOrDefaultAsync(b => b.Guess_id == guess.Guess_id && b.DiscountCode == request.Discount);
                if (bookingDiscount != null)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Mã giảm giá đã được sử dụng.", 400)));

                discount_value = (decimal)discount.Discount_percentage * total;

                if (discount_value > (decimal)discount.Max_discount)
                {
                    discount_value = (decimal)discount.Max_discount;
                }
               
                await _context.SaveChangesAsync();
            }

            return Ok(new ApiResponse(true, new GetBookingRequest
            {
                Check_in = request.Check_in,
                Check_out = request.Check_out,
                Discount = request.Discount,
                total = total,
                totalDays = (int)totalDays,
                discount_value = discount_value,
                totalDiscount = total - discount_value,
                Rooms = rooms,

            }, null));

        }

        [Authorize]
        [HttpPost("booking")]
        public async Task<ActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            //lưu thông tin đặt phòng
            var booking = new Booking
            {
                Check_in = DateTime.Parse(request.Check_in),
                Check_out = DateTime.Parse(request.Check_out),
                Request = request.Request,
                Status = Constant.BookingConstant.PENDDING,
                Total = request.Total,
                Guess_id = guess.Guess_id, //lấy id của người dùng đang đăng nhập
                DiscountCode=request.Discount
            };
            _context.Bookings.Add(booking);
            int result = await _context.SaveChangesAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Đặt phòng không thành công", 400)));
            //lưu phòng
            foreach (var room in request.Rooms)
            {
                var bookingRoom = new Booking_Room
                {
                    Booking_id = booking.Booking_id,
                    Room_id = room.Room_id,
                    Quantity = room.Quantity,
                    Name_Guess = room.Name_Guess,
                    Phone_Guess = room.Phone_Guess,
                };
                _context.Booking_Rooms.Add(bookingRoom);
            }
            result = await _context.SaveChangesAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Đặt phòng không thành công", 400)));
            else
                return Ok(new ApiResponse(true, booking.Booking_id, null));
        }

            
        [Authorize]
        [HttpPut("admin/update-status-booking")]
        public async Task<ActionResult> updateStatus([FromBody] StatusUpdateRequest request)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            //tìm booking
            var booking = _context.Bookings.FirstOrDefault(b => b.Booking_id == request.Booking_id);
            var discount = _context.Discounts.FirstOrDefault(d => d.Code == booking.DiscountCode);
            
            if (discount != null &&request.Status==Constant.BookingConstant.APPROVED&&booking.Status==Constant.BookingConstant.PENDDING)
            {
                //check số lượng
                discount.Quantity -= 1;
                _context.Discounts.Update(discount);
                await _context.AddRangeAsync();
            }

            //check null
            if (booking == null)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Không tồn tại booking", 400)));
            }

            //update và lưu vào db
            booking.Status = request.Status;
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(true, booking, null));
        }

        [Authorize]
        [HttpGet("get-booking")]
        public async Task<ActionResult> getCart(int id)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            //tìm booking
            var booking = _context.Bookings.FirstOrDefault(b => b.Booking_id == id);

            //check null
            if (booking == null)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Không tồn tại booking", 400)));
            }

            if (booking.Guess_id != guess.Guess_id)
            {
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không có quyền truy câp", 401)));
            }

            //tim booking room
            booking.Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == booking.Booking_id).ToListAsync().Result;

            var respone = new BookingRespone
            {
                Booking_id = booking.Booking_id,
                Check_in = booking.Check_in.ToString("dd/MM/yyyy"),
                Check_out = booking.Check_out.ToString("dd/MM/yyyy"),
                Status = booking.Status,
                //Description = booking.Description,
                Request = booking.Request,
                //Guess_id = booking.Guess_id,
                Booking_Rooms = booking.Booking_Rooms,
                total = booking.Total,
            };

            return Ok(new ApiResponse(true, respone, null));
        }

        [Authorize]
        [HttpGet("admin/get-all-booking")]
        public async Task<ActionResult<List<getAllBookingRespone>>> geAlltBooking(int Guess_id, string startdate, string enddate)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));


            try
            {
                DateTime.Parse(startdate);
            }
            catch (Exception ex)
            {
                startdate = string.Empty;
            }

            try
            {
                DateTime.Parse(enddate);
            }
            catch (Exception ex)
            { enddate = string.Empty; }



            if (Guess_id != 0 && startdate != string.Empty && enddate != string.Empty)
            {
                var bookingList = await _context.Bookings
                .Where(b => b.Guess_id == Guess_id && b.CreatedAt >= DateTime.Parse(startdate) && b.CreatedAt <= DateTime.Parse(enddate))
                .Select(b =>
                    new getAllBookingRespone
                    {
                        Booking_id = b.Booking_id,
                        guess_id=guess.Guess_id,
                        guess_ava=guess.Thumbnail,
                        guess_name = guess.Name,
                        guess_num = guess.PhoneNumber,
                        Status = b.Status,
                        Request = b.Request,
                        Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                        Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                        Total = b.Total,
                        Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                        Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                    }
                ).ToListAsync();
                return Ok(new ApiResponse(true, bookingList, null));

            }
            else if (Guess_id == 0 && startdate != string.Empty && enddate != string.Empty)
            {
                var bookingList = await _context.Bookings
                .Where(b => b.CreatedAt >= DateTime.Parse(startdate) && b.CreatedAt <= DateTime.Parse(enddate))
                .Select(b =>
                    new getAllBookingRespone
                    {
                        Booking_id = b.Booking_id,
                        guess_id=guess.Guess_id,
                        guess_ava=guess.Thumbnail,
                        guess_name = guess.Name,
                        guess_num = guess.PhoneNumber,
                        Status = b.Status,
                        Request = b.Request,
                        Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                        Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                        Total = b.Total,
                        Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                        Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                    }
                ).ToListAsync();
                return Ok(new ApiResponse(true, bookingList, null));

            }
            else if (Guess_id != 0 && startdate == string.Empty && enddate == string.Empty)
            {
                var bookingList = await _context.Bookings
               .Where(b => b.Guess_id == Guess_id)
               .Select(b =>
                   new getAllBookingRespone
                   {
                       Booking_id = b.Booking_id,
                       guess_id = guess.Guess_id,
                       guess_ava = guess.Thumbnail,
                       guess_name = guess.Name,
                       guess_num = guess.PhoneNumber,
                       Status = b.Status,
                       Request = b.Request,
                       Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                       Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                       Total = b.Total,
                       Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                       Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                   }
               ).ToListAsync();
                return Ok(new ApiResponse(true, bookingList, null));

            }
            else if (Guess_id == 0 && (startdate != string.Empty || enddate != string.Empty))
            {
                if (startdate != string.Empty)
                {
                    var bookingList = await _context.Bookings
                   .Where(b => b.CreatedAt.Date == DateTime.Parse(startdate).Date)
                   .Select(b =>
                       new getAllBookingRespone
                       {
                           Booking_id = b.Booking_id,
                           guess_id = guess.Guess_id,
                           guess_ava = guess.Thumbnail,
                           guess_name = guess.Name,
                           guess_num = guess.PhoneNumber,
                           Status = b.Status,
                           Request = b.Request,
                           Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                           Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                           Total = b.Total,
                           Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                           Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                       }
                   ).ToListAsync();
                    return Ok(new ApiResponse(true, bookingList, null));
                }
                else
                {
                    var bookingList = await _context.Bookings
                  .Where(b => b.CreatedAt.Date == DateTime.Parse(enddate).Date)
                  .Select(b =>
                      new getAllBookingRespone
                      {
                          Booking_id = b.Booking_id,
                          guess_id = guess.Guess_id,
                          guess_ava = guess.Thumbnail,
                          guess_name = guess.Name,
                          guess_num = guess.PhoneNumber,
                          Status = b.Status,
                          Request = b.Request,
                          Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                          Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                          Total = b.Total,
                          Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                          Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                      }
                  ).ToListAsync();
                    return Ok(new ApiResponse(true, bookingList, null));
                }

            }
            else if (Guess_id != 0 && (startdate != string.Empty || enddate != string.Empty))
            {
                if (startdate != string.Empty)
                {
                    var bookingList = await _context.Bookings
                   .Where(b => b.Guess_id == b.Guess_id && b.CreatedAt.Date == DateTime.Parse(startdate).Date)
                   .Select(b =>
                       new getAllBookingRespone
                       {
                           Booking_id = b.Booking_id,
                           guess_id = guess.Guess_id,
                           guess_ava = guess.Thumbnail,
                           guess_name = guess.Name,
                           guess_num = guess.PhoneNumber,
                           Status = b.Status,
                           Request = b.Request,
                           Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                           Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                           Total = b.Total,
                           Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                           Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                       }
                   ).ToListAsync();
                    return Ok(new ApiResponse(true, bookingList, null));
                }
                else
                {
                    var bookingList = await _context.Bookings
                  .Where(b => b.Guess_id == Guess_id && b.CreatedAt.Date == DateTime.Parse(enddate).Date)
                  .Select(b =>
                      new getAllBookingRespone
                      {
                          Booking_id = b.Booking_id,
                          guess_id = guess.Guess_id,
                          guess_ava = guess.Thumbnail,
                          guess_name = guess.Name,
                          guess_num = guess.PhoneNumber,
                          Status = b.Status,
                          Request = b.Request,
                          Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                          Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                          Total = b.Total,
                          Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                          Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                      }
                  ).ToListAsync();
                    return Ok(new ApiResponse(true, bookingList, null));
                }

            }
            else
            {
                var bookingList = await _context.Bookings
                .Select(b =>
                    new getAllBookingRespone
                    {
                        Booking_id = b.Booking_id,
                        guess_id = guess.Guess_id,
                        guess_ava = guess.Thumbnail,
                        guess_name = guess.Name,
                        guess_num = guess.PhoneNumber,
                        Status = b.Status,
                        Request = b.Request,
                        Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                        Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                        Total = b.Total,
                        Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                        Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                    }
                ).ToListAsync();
                return Ok(new ApiResponse(true, bookingList, null));
            }
        }

        [Authorize]
        [HttpPut("cancel-booking")]
        public async Task<ActionResult> cancelbooking([FromBody] cancelBookingRequest request)
        {
            //xác thực người dùng
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            //tìm booking
            var booking = _context.Bookings.FirstOrDefault(b => b.Booking_id == request.Booking_id);

            //check null
            if (booking == null)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Không tồn tại đặt phòng", 400)));
            }

            if (booking.Status != Constant.BookingConstant.PENDDING)
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Không thể hủy đặt phòng", 400)));
            }

            //update và lưu vào db
            booking.Status = Constant.BookingConstant.CANCEL;
            booking.Request = request.request;
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse(true, "Hủy đặt phòng thành công ", null));
        }

        [Authorize]
        [HttpGet("admin/get-all-status")]
        public async Task<ActionResult> getAllStatus()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            string[] StatusList = ["Pendding", "Closed", "Cancel", "Rejected", "Approved"];
            return Ok(new ApiResponse(true, StatusList, null));
        }

        [Authorize]
        [HttpGet("user/get-all-booking")]
        public async Task<ActionResult> getAllCurentBooking(string enddate, string startdate)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            
            try
            {
                DateTime.Parse(startdate);
            }
            catch (Exception ex)
            {
                startdate = string.Empty;
            }

            try
            {
                DateTime.Parse(enddate);
            }
            catch (Exception ex)
            { enddate = string.Empty; }


            if (startdate != string.Empty && enddate != string.Empty)
            {
                var bookingList = await _context.Bookings
              .Where(b => b.Guess_id == guess.Guess_id && b.CreatedAt >= DateTime.Parse(startdate) && b.CreatedAt <= DateTime.Parse(enddate))
              .Select(b =>
                  new getAllBookingRespone
                  {
                      Booking_id = b.Booking_id,
                      guess_id = guess.Guess_id,
                      guess_ava = guess.Thumbnail,
                      guess_name = guess.Name,
                      guess_num = guess.PhoneNumber,
                      Status = b.Status,
                      Request = b.Request,
                      Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                      Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                      Total = b.Total,
                      Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                      Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                  }
              ).ToListAsync();

                return Ok(new ApiResponse(true, bookingList, null));

            }
            else if (startdate != string.Empty && enddate == string.Empty)
            {
                var bookingList = await _context.Bookings
             .Where(b => b.Guess_id == guess.Guess_id && b.CreatedAt.Date == DateTime.Parse(startdate).Date)
             .Select(b =>
                 new getAllBookingRespone
                 {
                     Booking_id = b.Booking_id,
                     guess_id = guess.Guess_id,
                     guess_ava = guess.Thumbnail,
                     guess_name = guess.Name,
                     guess_num = guess.PhoneNumber,
                     Status = b.Status,
                     Request = b.Request,
                     Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                     Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                     Total = b.Total,
                     Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                     Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                 }
             ).ToListAsync();

                return Ok(new ApiResponse(true, bookingList, null));
            }
            else if (startdate == string.Empty && enddate != string.Empty)
            {
                var bookingList = await _context.Bookings
             .Where(b => b.Guess_id == guess.Guess_id && b.CreatedAt.Date == DateTime.Parse(enddate).Date)
             .Select(b =>
                 new getAllBookingRespone
                 {
                     Booking_id = b.Booking_id,
                     guess_id = guess.Guess_id,
                     guess_ava = guess.Thumbnail,
                     guess_name = guess.Name,
                     guess_num = guess.PhoneNumber,
                     Status = b.Status,
                     Request = b.Request,
                     Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                     Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                     Total = b.Total,
                     Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                     Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                 }
             ).ToListAsync();

                return Ok(new ApiResponse(true, bookingList, null));
            }
            else
            {
                var bookingList = await _context.Bookings
               .Where(b => b.Guess_id == guess.Guess_id)
               .Select(b =>
                   new getAllBookingRespone
                   {
                       Booking_id = b.Booking_id,
                       guess_id = guess.Guess_id,
                       guess_ava = guess.Thumbnail,
                       guess_name = guess.Name,
                       guess_num = guess.PhoneNumber,
                       Status = b.Status,
                       Request = b.Request,
                       Check_in = b.Check_in.ToString("dd/MM/yyyy"),
                       Check_out = b.Check_out.ToString("dd/MM/yyyy"),
                       Total = b.Total,
                       Booking_Rooms = _context.Booking_Rooms.Where(br => br.Booking_id == b.Booking_id).ToList(),
                       Quantity = b.Booking_Rooms.Sum(r => r.Quantity)
                   }
               ).ToListAsync();

                return Ok(new ApiResponse(true, bookingList, null));
            }

        }
    }
}


